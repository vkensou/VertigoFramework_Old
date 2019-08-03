using UnityEngine;
using System.Collections;
using UML;

public abstract class EntitasSubProcedure : SimpleProcedure
{
    protected Contexts m_contexts;
    protected Transform m_rootNode;
    protected IEventRoute m_eventRoute;
    protected StateMachine m_procedureStateMachine;
    protected Feature m_systems;
    protected Blackboard m_blackboard;

    public EntitasSubProcedure(string name)
        : base(name, false)
    {
    }

    public override IEventRoute EventRoute => m_eventRoute;

    public override UML.StateMachine ProcedureStateMachine => m_procedureStateMachine;

    public override Blackboard Blackboard => m_blackboard;

    public void InitialSubProcedure(Contexts contexts, Transform rootNode, IEventRoute eventRoute)
    {
        m_contexts = contexts;
        m_rootNode = rootNode;
        m_eventRoute = eventRoute;
    }

    protected virtual StateMachine CreateStateMachine() { return null; }

    protected abstract void CreateSystems(Feature feature);

    protected override void EnterProcedure(StateEventArg arg)
    {
        m_blackboard = new Blackboard();

        m_procedureStateMachine = CreateStateMachine();

        m_systems = new Feature("Systems");
        CreateSystems(m_systems);
        m_systems.Initialize();

        if (m_procedureStateMachine != null)
        {
            m_procedureStateMachine.EnterStateEvent += OnEnterState;
            m_procedureStateMachine.Start();
        }
    }

    protected virtual void OnEnterState(string state, StateEventArg arg)
    {
        EventRoute.SendEvent(EventSendType.OneFrame, new SystemSwitchStateEvent { state = state, arg = arg });
    }

    protected override void LeaveProcedure(StateEventArg arg)
    {
        m_procedureStateMachine?.Destroy();
        m_systems.TearDown();
        EventRoute.Dispose();

#if (!ENTITAS_DISABLE_VISUAL_DEBUGGING && UNITY_EDITOR)
        Object.Destroy(m_systems.gameObject);
#endif

        m_blackboard = null;
        m_procedureStateMachine = null;
        m_systems = null;
        m_contexts = null;
        m_rootNode = null;
        m_eventRoute = null;
    }

    protected override void OnUpdate()
    {
        base.OnUpdate();

        m_systems.Execute();

        m_systems.Cleanup();

        SystemRequireSwitchStateEvent e = null;
        if (m_procedureStateMachine != null)
            e = EventRoute.TakeEvent<SystemRequireSwitchStateEvent>();

        m_procedureStateMachine?.Update();

        EventRoute.ClearOutOfDateEvents();

        if (e != null)
            m_procedureStateMachine.FireEvent(e.transition, e.eventArg);
    }
}
