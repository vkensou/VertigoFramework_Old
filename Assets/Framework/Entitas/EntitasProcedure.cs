using UnityEngine;
using System.Collections;
using Entitas;

public abstract class EntitasProcedure : SimpleProcedure
{
    protected Feature m_systems;
    protected Contexts m_contexts;
    protected Transform m_rootNode;
    protected ISystemEventRoute m_eventRoute;
    protected UML.StateMachine m_procedureStateMachine;
    public override ISystemEventRoute EventRoute => m_eventRoute;
    public override UML.StateMachine ProcedureStateMachine => m_procedureStateMachine;

    public EntitasProcedure(string name)
        :base(name)
    {
    }

    protected override void OnEnter(UML.EnterEventArg userData)
    {
        m_contexts = new Contexts();

        m_eventRoute = new SystemEventRoute();

        m_procedureStateMachine = new UML.StateMachine();
        InitialStateMachine(m_procedureStateMachine);

        m_systems = new Feature("Systems");
        CreateSystems(m_systems);

        m_rootNode = new GameObject(GetRootNodeName()).transform;
        //EntityCreateService.SetRootNode(m_rootNode);

        //m_procedureStateMachine.StateEnterEvent += (name) => { m_eventRoute.SendEvent(new SystemSwitchStateEvent { state = name }); };
        m_procedureStateMachine.Start();

        // call Initialize() on all of the IInitializeSystems
        m_systems.Initialize();

        EventRoute.RemoveEvent<SystemSwitchStateEvent>();
    }

    protected abstract void CreateSystems(Feature feature);
    protected virtual string GetRootNodeName() { return "EntitasProcedure Root"; }
    protected abstract void InitialStateMachine(UML.StateMachine procedureStateMachine);

    protected override void OnLeave()
    {
        m_procedureStateMachine = null;

        EventRoute.Dispose();
        Object.Destroy(m_rootNode.gameObject);
        m_systems.TearDown();
#if (!ENTITAS_DISABLE_VISUAL_DEBUGGING && UNITY_EDITOR)
        Object.Destroy(m_systems.gameObject);
#endif
        m_systems = null;
        m_contexts.game.DestroyAllEntities();
        m_contexts.Reset();
        m_contexts = null;
    }

    protected override void OnUpdate()
    {
        // call Execute() on all the IExecuteSystems and 
        // ReactiveSystems that were triggered last frame
        m_systems.Execute();
        // call cleanup() on all the ICleanupSystems
        m_systems.Cleanup();

        SystemRequireSwitchStateEvent requireSwitchStateEvent;
        if(EventRoute.TryTakeEvent(out requireSwitchStateEvent))
        {
            m_procedureStateMachine.FireEvent(requireSwitchStateEvent.transition);
        }

        m_procedureStateMachine.Update();

        EventRoute.RemoveAll();
    }
}
