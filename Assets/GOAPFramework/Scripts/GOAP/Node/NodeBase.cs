using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GOAP.Serialization;
using GOAP.Helper;
namespace GOAP.Framework
{
    public abstract partial class NodeBase
    {
        [SerializeField] private string _name = null;
        [SerializeField] private string comment = null;
        [SerializeField] private bool is_breakpoint = false;

        [SerializeField]
        protected List<NodeAction> neighbours = new List<NodeAction>();

        private GoapGraph graph;
        [System.NonSerialized] private string _nodeName;
        [System.NonSerialized] private int ID;
        [System.NonSerialized] private Status _status = Status.START;

        public bool toDelete
        {
            get
            {
                return _toDelete;
            }

        }
        protected bool _toDelete = false;


        public Status status
        {
            get
            {
                return _status;
            }
            set
            {
                // Debug.Log("Status Change to " + value.ToString());
                _status = value;
            }
        }
        private string customName
        {
            get
            {
                return _name;
            }
            set
            {
                _name = value;
            }
        }
        public string nodeComment
        {
            get
            {
                return comment;
            }
            set
            {
                comment = value;
            }
        }
        public bool isBreakpoint
        {
            get
            {
                return is_breakpoint;
            }
            set
            {
                is_breakpoint = value;
            }
        }
        public GoapGraph goapGraph
        {
            get
            {
                return graph;
            }
            set
            {
                graph = value;
            }
        }
        public int nodeID
        {
            get
            {
                return ID;
            }
            set
            {
                ID = value;
            }
        }
        virtual public string name
        {
            get
            {
                if (!string.IsNullOrEmpty(_name))
                    return _name;
                else if (string.IsNullOrEmpty(nodeName))
                {
                    NameAttribute new_name = GetType().RHelperGetAttribute<NameAttribute>(false);
                    _nodeName = new_name != null ? new_name.name : GetType().FriendlyName();
                }
                return _nodeName;
            }
            set
            {
                customName = value;
            }
        }
        public string nodeName
        {
            get
            {
                if (!string.IsNullOrEmpty(_nodeName))
                    return _nodeName;
                else if (string.IsNullOrEmpty(_nodeName))
                {
                    NameAttribute new_name = GetType().RHelperGetAttribute<NameAttribute>(false);
                    _nodeName = new_name != null ? new_name.name : GetType().FriendlyName();
                }
                return _nodeName;
            }
        }
        public Component goapGraphAgent
        {
            get { return goapGraph != null ? goapGraph.agent : null; }
        }
        public IWorldState worldState
        {
            get { return goapGraph != null ? goapGraph.worldState : null; }
        }
        public List<NodeAction> allNeighbours
        {
            get
            {
                return neighbours;
            }
        }
        public NodeBase() { }

        public static NodeBase Create(GoapGraph goap_graph, System.Type nodeType)
        {

            if (goap_graph == null)
            {
                Debug.LogError("Error Can't create Node Graph is NULL");
                return null;
            }
            NodeBase ret = (NodeBase)System.Activator.CreateInstance(nodeType);

#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                UnityEditor.Undo.RecordObject(goap_graph, "Create Node");
            }
#endif
            ret.goapGraph = goap_graph;
            WSParameter.SetWSFields(ret, goap_graph.worldState);
            ret.OnValidate(goap_graph);
            return ret;
        }
        public NodeBase Duplicate(GoapGraph goap_graph)
        {
            if (goap_graph == null)
            {
                return null;
            }

            NodeBase ret = JSONSerializer.Deserialize<NodeBase>(JSONSerializer.Serialize(typeof(NodeBase), this));

            ret.goapGraph = goap_graph;
            WSParameter.SetWSFields(ret, goap_graph.worldState);

            ret.OnValidate(goap_graph);
            return ret;
        }
        public void AddNeighbours(NodeAction neighbour)
        {
            if (!neighbours.Contains(neighbour))
                neighbours.Add(neighbour);
        }
        public void ResetNeighbours()
        {
            neighbours.Clear();
        }


        protected void StopCoroutine(Coroutine coroutine)
        {

        }
        abstract public bool IsValid(Component agent, IWorldState world_state);
        public void Reset()
        {
            if (_status == Status.START)
                return;

            OnReset();
            _status = Status.START;
        }
      
        
        abstract protected Status OnExecute(Component agent, IWorldState worldState);
        abstract protected void OnReset();
        abstract protected void AddPrecondition(WSVariable variable);
        abstract protected void RemovePrecondition(PEParameter variable);

        abstract protected void AddEffect(WSVariable variable);
        abstract protected void RemoveEffect(PEParameter variable);

        abstract protected void AddDesire(WSVariable variable);
        abstract protected void RemoveDesire(PEParameter variable);

        abstract protected void AddPrecondition(WSFunctionVariable variable);
        abstract protected void RemovePrecondition(PEMethod variable);

        abstract protected void AddEffect(WSFunctionVariable variable);
        abstract protected void RemoveEffect(PEMethod variable);

        abstract protected void AddDesire(WSFunctionVariable variable);
        abstract protected void RemoveDesire(PEMethod variable);

        abstract public void OnValidate(GoapGraph goap_graph);

        abstract public void OnDestroy();
    }
}