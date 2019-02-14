using UnityEngine;
using System.Collections;
using UML;

public abstract class EntitasSubProcedure : SimpleProcedure
{
    protected Contexts m_contexts;
    protected Transform m_rootNode;
    protected ISystemEventRoute m_eventRoute;
    protected StateMachine m_stateMachine;
    protected Feature m_systems;
    protected Blackboard m_blackboard;
    private bool m_paused = false;

    public EntitasSubProcedure(string name)
        : base(name)
    {
    }

    public override ISystemEventRoute EventRoute => m_eventRoute;

    public override UML.StateMachine ProcedureStateMachine => m_stateMachine;

    public override Blackboard Blackboard => m_blackboard;

    public void InitialSubProcedure(Contexts contexts, Transform rootNode, ISystemEventRoute eventRoute)
    {
        m_contexts = contexts;
        m_rootNode = rootNode;
        m_eventRoute = eventRoute;
        m_blackboard = new Blackboard();
    }

    protected virtual StateMachine CreateStateMachine() { return null; }

    protected override void OnEnter(UML.StateEventArg userData)
    {
        base.OnEnter(userData);

        m_stateMachine = CreateStateMachine();

        m_systems = new Feature("Systems");
        CreateSystems(m_systems);

        m_systems.Initialize();

        if (m_stateMachine != null)
        {
            m_stateMachine.EnterStateEvent += OnEnterState;
            m_stateMachine.Start();
        }
    }

    protected abstract void CreateSystems(Feature feature);

    protected override void OnLeave(StateEventArg arg)
    {
        var p = arg as ProcedurePauseEvent;
        if (p != null)
        {
            m_paused = true;
            if (p.rootNodeHide)
                m_rootNode.gameObject.SetActive(false);
            return;
        }

        m_systems.TearDown();
#if (!ENTITAS_DISABLE_VISUAL_DEBUGGING && UNITY_EDITOR)
        Object.Destroy(m_systems.gameObject);
#endif
        m_systems = null;
        base.OnLeave(arg);
    }

    protected override void OnUpdate()
    {
        base.OnUpdate();
        // call Execute() on all the IExecuteSystems and 
        // ReactiveSystems that were triggered last frame
        m_systems.Execute();
        // call cleanup() on all the ICleanupSystems
        m_systems.Cleanup();

        SystemRequireSwitchStateEvent e = null;
        if (m_stateMachine != null)
            e = EventRoute.TakeEvent<SystemRequireSwitchStateEvent>();

        m_stateMachine?.Update();

        EventRoute.RemoveAll();

        if (e != null)
            m_stateMachine.FireEvent(e.transition, e.eventArg);
    }

    protected virtual void OnEnterState(string state, StateEventArg arg)
    {
        Debug.LogFormat("Enter {0}", state);
        EventRoute.SendEvent(EventSendType.OneFrame, new SystemSwitchStateEvent { state = state, arg = arg });
    }
}
