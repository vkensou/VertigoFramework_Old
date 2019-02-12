using UnityEngine;
using System.Collections;
using UML;

public abstract class EntitasCompositeProcedure : CompositeProcedure
{
    protected Contexts m_contexts;
    protected Transform m_rootNode;
    protected ISystemEventRoute m_eventRoute;
    protected StateMachine m_stateMachine;

    public EntitasCompositeProcedure(string name)
        : base(name)
    {

    }

    public override ISystemEventRoute EventRoute => m_eventRoute;
    public override StateMachine ProcedureStateMachine => m_stateMachine;

    protected override void OnEnter(StateEventArg userData)
    {
        base.OnEnter(userData);
        m_contexts = new Contexts();

        m_eventRoute = new SystemEventRoute();

        m_stateMachine = CreateStateMachine();

        m_rootNode = new GameObject(GetRootNodeName()).transform;
        RootNodeService.RootNode = m_rootNode;

        foreach (var v in Region.Subvertices)
        {
            EntitasSubProcedure p = v as EntitasSubProcedure;
            if (p == null)
                continue;
            p.InitialSubProcedure(m_contexts, m_rootNode, m_eventRoute);
        }
    }

    protected virtual string GetRootNodeName() { return "EntitasProcedure Root"; }

    protected virtual StateMachine CreateStateMachine() { return null; }

    protected override void OnLeave(StateEventArg arg)
    {
        m_eventRoute.Dispose();
        Object.Destroy(m_rootNode.gameObject);
        m_contexts.game.DestroyAllEntities();
        m_contexts.Reset();
#if (!ENTITAS_DISABLE_VISUAL_DEBUGGING && UNITY_EDITOR)
        var contextObserverBehaviours = Object.FindObjectsOfType<Entitas.VisualDebugging.Unity.ContextObserverBehaviour>();
        var allContexts = m_contexts.allContexts;
        foreach (var ob in contextObserverBehaviours)
        {
            if (System.Array.IndexOf(allContexts, ob.contextObserver.context) != -1)
                Object.Destroy(ob.gameObject);
        }
#endif
        m_contexts = null;
        base.OnLeave(arg);
    }

    protected override void OnUpdate()
    {
        base.OnUpdate();
    }
}
