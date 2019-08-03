using UnityEngine;
using System.Collections;
using Entitas;
using UML;

public abstract class EntitasProcedure : SimpleProcedure
{
    protected Feature m_systems;
    protected Contexts m_contexts;
    protected Transform m_rootNode;
    protected IEventRoute m_eventRoute;
    protected UML.StateMachine m_procedureStateMachine;
    protected Blackboard m_blackboard;
    protected Schedulable m_schedulable;

    public override IEventRoute EventRoute => m_eventRoute;
    public override UML.StateMachine ProcedureStateMachine => m_procedureStateMachine;
    public override Blackboard Blackboard => base.Blackboard;

    public EntitasProcedure(string name, bool suspendable = false)
        :base(name, suspendable)
    {
    }

    protected virtual StateMachine CreateStateMachine() { return null; }
    protected abstract void CreateSystems(Feature feature, EntitasSystemEnvironment parameters);
    protected virtual string GetRootNodeName() { return string.Format("EntitasProcedure {0} Root", Name); }

    protected override void EnterProcedure(StateEventArg arg)
    {
        m_contexts = new Contexts();

        m_eventRoute = new EntitasEventRoute(m_contexts);

        m_rootNode = new GameObject(GetRootNodeName()).transform;
        RootNodeService.RootNode = m_rootNode;

        m_blackboard = new Blackboard();

        m_procedureStateMachine = CreateStateMachine();

        m_schedulable = new Schedulable();

        m_systems = new Feature("Systems");
        EntitasSystemEnvironment parameters = new EntitasSystemEnvironment(m_contexts.game, EventRoute, Blackboard, ProcedureStateMachine, m_schedulable);
        CreateSystems(m_systems, parameters);
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

    protected override void ResumeProcedure(StateEventArg arg)
    {
        m_rootNode.gameObject.SetActive(true);
    }

    protected override void PauseProcedure(ProcedurePauseEvent arg)
    {
        if (arg.rootNodeHide)
            m_rootNode.gameObject.SetActive(false);
    }

    protected override void LeaveProcedure(StateEventArg arg)
    {
        m_procedureStateMachine?.Destroy();
        m_systems.TearDown();
        EventRoute.Dispose();
        m_contexts.game.DestroyAllEntities();
        m_contexts.Reset();
        Object.Destroy(m_rootNode.gameObject);

#if (!ENTITAS_DISABLE_VISUAL_DEBUGGING && UNITY_EDITOR)
        Object.Destroy(m_systems.gameObject);

        var contextObserverBehaviours = Object.FindObjectsOfType<Entitas.VisualDebugging.Unity.ContextObserverBehaviour>();
        var allContexts = m_contexts.allContexts;
        foreach (var ob in contextObserverBehaviours)
        {
            if (System.Array.IndexOf(allContexts, ob.contextObserver.context) != -1)
                Object.Destroy(ob.gameObject);
        }
#endif

        m_blackboard = null;
        m_procedureStateMachine = null;
        m_systems = null;
        m_contexts = null;
        m_rootNode = null;
        m_eventRoute = null;
        m_schedulable = null;
    }

    protected override void OnUpdate()
    {
        base.OnUpdate();

        m_schedulable.ProcessSchedule(Time.deltaTime);
        m_systems.Execute();

        SystemRequireSwitchStateEvent e = null;
        if (m_procedureStateMachine != null)
            e = EventRoute.TakeEvent<SystemRequireSwitchStateEvent>();

        m_procedureStateMachine?.Update();

        EventRoute.ClearOutOfDateEvents();

        if (e != null)
            m_procedureStateMachine.FireEvent(e.transition, e.eventArg);
    }
}
