using UnityEngine;
using System.Collections;
using Entitas;
using UML;

public abstract class EntitasProcedure : SimpleProcedure
{
    protected Feature m_systems;
    protected Contexts m_contexts;
    protected Transform m_rootNode;
    protected ISystemEventRoute m_eventRoute;
    protected UML.StateMachine m_procedureStateMachine;
    private bool m_paused = false;
    private bool m_running = false;

    public override ISystemEventRoute EventRoute => m_eventRoute;
    public override UML.StateMachine ProcedureStateMachine => m_procedureStateMachine;

    public EntitasProcedure(string name)
        :base(name)
    {
    }

    protected virtual StateMachine CreateStateMachine() { return null; }

    protected override void OnEnter(StateEventArg arg)
    {
        base.OnEnter(arg);

        if (m_paused)
            ResumeProcedure();
        else
            EnterProcedure();
    }

    protected virtual void EnterProcedure()
    {
        if (m_running)
            return;
        m_running = true;
        m_paused = false;

        m_contexts = new Contexts();

        m_eventRoute = new SystemEventRoute();

        m_procedureStateMachine = CreateStateMachine();

        m_systems = new Feature("Systems");
        CreateSystems(m_systems);

        m_rootNode = new GameObject(GetRootNodeName()).transform;
        RootNodeService.RootNode = m_rootNode;

        //m_procedureStateMachine.StateEnterEvent += (name) => { m_eventRoute.SendEvent(new SystemSwitchStateEvent { state = name }); };
        m_procedureStateMachine?.Start();

        // call Initialize() on all of the IInitializeSystems
        m_systems.Initialize();

        EventRoute.RemoveEvent<SystemSwitchStateEvent>();
    }

    protected virtual void ResumeProcedure()
    {
        if (m_running && m_paused)
        {
            m_rootNode.gameObject.SetActive(true);
            m_paused = false;
        }
    }

    protected abstract void CreateSystems(Feature feature);
    protected virtual string GetRootNodeName() { return string.Format("EntitasProcedure {0} Root", Name); }

    protected override void OnLeave(StateEventArg arg)
    {
        var p = arg as ProcedurePauseEvent;
        if (p != null)
            PauseProcedure(p);
        else
            LeaveProcedure(arg);

        base.OnLeave(arg);
    }

    public virtual void LeaveProcedure(StateEventArg arg)
    {
        if (!m_running)
            return;

        m_running = false;
        m_paused = false;

        m_procedureStateMachine = null;

        EventRoute.Dispose();
        Object.Destroy(m_rootNode.gameObject);
        m_systems.TearDown();
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
        m_systems = null;
        m_contexts.game.DestroyAllEntities();
        m_contexts.Reset();
        m_contexts = null;
    }

    protected virtual void PauseProcedure(ProcedurePauseEvent arg)
    {
        if(m_running && !m_paused)
        {
            m_paused = true;
            if (arg.rootNodeHide)
                m_rootNode.gameObject.SetActive(false);
        }
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
        if (m_procedureStateMachine != null)
            e = EventRoute.TakeEvent<SystemRequireSwitchStateEvent>();

        m_procedureStateMachine?.Update();

        EventRoute.RemoveAll();

        if (e != null)
            m_procedureStateMachine.FireEvent(e.transition, e.eventArg);
    }
}
