using UnityEngine;
using System.Collections;
using UML;

public abstract class EntitasCompositeProcedure : CompositeProcedure
{
    protected Contexts m_contexts;
    protected Transform m_rootNode;
    protected IEventRoute m_eventRoute;
    protected StateMachine m_procedureStateMachine;

    public EntitasCompositeProcedure(string name)
        : base(name)
    {

    }

    public override IEventRoute EventRoute => m_eventRoute;
    public override StateMachine ProcedureStateMachine => m_procedureStateMachine;

    protected virtual string GetRootNodeName() { return "EntitasProcedure Root"; }
    protected virtual StateMachine CreateStateMachine() { return null; }

    protected override void EnterProcedure(StateEventArg userData)
    {
        m_contexts = new Contexts();

        m_eventRoute = new EntitasEventRoute(m_contexts);

        m_rootNode = new GameObject(GetRootNodeName()).transform;
        RootNodeService.RootNode = m_rootNode;

        m_procedureStateMachine = CreateStateMachine();

        foreach (var v in Region.Subvertices)
        {
            EntitasSubProcedure p = v as EntitasSubProcedure;
            if (p == null)
                continue;
            p.InitialSubProcedure(m_contexts, m_rootNode, EventRoute);
        }
    }

    protected override void LeaveProcedure(StateEventArg arg)
    {
        EventRoute.Dispose();
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

        m_procedureStateMachine = null;
        m_contexts = null;
        m_rootNode = null;
        m_eventRoute = null;
    }

    protected override void OnUpdate()
    {
        base.OnUpdate();
    }
}
