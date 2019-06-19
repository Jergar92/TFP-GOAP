using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using GOAP.Serialization;

using GOAP.Framework.Internal;
namespace GOAP.Framework
{
    public partial class GoapGraph : ScriptableObject, IFunctionSystem, ISerializationCallbackReceiver
    {

        [System.NonSerialized]
        private List<NodeAction> _currentPlan;
        [HideInInspector]
        [SerializeField]
        private string _serializedGraph;
        [SerializeField]
        private List<Object> _objectRef;
        [SerializeField]
        private GoapPlanner _planner = null;
        [HideInInspector]
        private int plan_position = -1;

        [SerializeField]
        private bool _deserealizationFail = false;
        [System.NonSerialized]
        private bool _isEnabled = false;
        [System.NonSerialized]
        private bool _isDeserealized = false;


        private string _name = string.Empty;
        private string _comments = string.Empty;
        private List<NodeAction> _nodeActions = new List<NodeAction>();
        private List<NodeGoal> _nodeGoals = new List<NodeGoal>();
#if UNITY_EDITOR
        [System.NonSerialized]
        private List<NodeBase> _lastPlan = new List<NodeBase>();
#endif
        private NodeGoal _priorityGoal;
        private NodeGoal _currentGoal = null;
        private NodeAction _currentNode = null;

        [System.NonSerialized]
        private IWorldState _worldState = null;
        [System.NonSerialized]
        private Component _agent = null;
        [System.NonSerialized]
        private bool _isRunning = false;
        new public string name
        {
            get
            {
                return string.IsNullOrEmpty(_name) ? GetType().Name : _name;
            }
            set
            {
                _name = value;
            }
        }
        public string comments
        {
            get
            {
                return string.IsNullOrEmpty(_comments) ? "empty" : _comments;

            }
            set
            {
                _comments = value;
            }
        }
        public bool isRunning
        {
            get
            {
                return _isRunning;
            }
            set
            {
                _isRunning = value;
            }
        }

        public NodeGoal currentGoal
        {
            set
            {
                if (value != null)
                    Debug.Log("Priority Goal is " + value.name);
                _currentGoal = value;
            }
            get
            {
                return _currentGoal;
            }
        }
        public NodeAction currentNode
        {
            set
            {
                Debug.Log(value.name);
                _currentNode = value;
            }
            get
            {
                return _currentNode;
            }
        }
        public NodeGoal priorityGoalRef
        {
            get
            {
                return priorityGoal;
            }
        }
        private NodeGoal priorityGoal
        {
            set
            {
                if (priorityGoal == value)
                    return;
                if(value!=null)
                    Debug.Log("Priority Goal is " + value.name);

                _priorityGoal = value;
                if (planner.noPlan)
                {
                    currentGoal = _priorityGoal;
                }
            }
            get
            {
                return _priorityGoal;
            }
        }
    
        private bool isPriorityGoalChange
        {

            get
            {
                if (currentGoal != _priorityGoal)
                {
                    if (currentGoal != null)
                        Debug.Log("Interrupting current goal " + currentGoal.name);
                    if (_priorityGoal != null)
                        Debug.Log("Interrupting _priorityGoal " + _priorityGoal.name);
                }

                return currentGoal != _priorityGoal;
            }
        }
        private GoapPlanner planner
        {
            get
            {
                if (_planner == null)
                {
                    _planner = new GoapPlanner();
                    planner.goapGraph = this;
                }
                return _planner;
            }
        }
        public List<NodeAction> currentPlan
        {
            get
            {
                if (_currentPlan == null)
                    _currentPlan = new List<NodeAction>();
                return _currentPlan;
            }
            set
            {
                _currentPlan = value;
            }
        }
#if UNITY_EDITOR
        public List<NodeBase> lastPlan
        {
            get
            {
                if (_lastPlan == null)
                    _lastPlan = new List<NodeBase>();
                return _lastPlan;
            }
       
        }
#endif
        public List<NodeAction> allNodeActions
        {
            get { return _nodeActions; }
            private set { _nodeActions = value; }
        }
        public List<NodeGoal> allNodeGoals
        {
            get { return _nodeGoals; }
            private set { _nodeGoals = value; }
        }


        public Component agent
        {
            get { return _agent; }
            set { _agent = value; }
        }

        Object IFunctionSystem.contextObj
        {
            get { return this; }
        }
        public IWorldState worldState
        {
            get
            {

#if UNITY_EDITOR
                if (_worldState == null || _worldState.Equals(null))
                {
                    return null;
                }
#endif

                return _worldState;
            }
            set
            {
                if (_worldState != value)
                {
                    if (isRunning)
                        return;
                    _worldState = value;

                }
            }
        }
        public string priorityGoalName
        {
            get
            {
                return priorityGoal != null ? priorityGoal.name : "NONE";
            }
        }
        public string currentActionName
        {
            get
            {
                return currentNode != null ? currentNode.name : "NONE";
            }
        }
        public void OnBeforeSerialize()
        {
            if (_objectRef != null && _objectRef.Any(o => o != null))
            {
                _isDeserealized = false;
            }
#if UNITY_EDITOR
            if (JSONSerializer.appIsPlaying)
                return;
            _serializedGraph = Serialize(false, _objectRef);
#endif
        }
        public void OnAfterDeserialize()
        {
            if (_isDeserealized && JSONSerializer.appIsPlaying)
                return;

            _isDeserealized = true;
            Deserialize(_serializedGraph, false, _objectRef);
        }

#if UNITY_EDITOR
        protected void OnValidate()
        {
            if (!Application.isPlaying)
                Validate();
        }
#endif
        protected void OnEnable()
        {
            if (!_isEnabled)
            {
                _isEnabled = true;
                Validate();
            }
        }
        public void StartGoap(Component agent, IWorldState worldState)
        {
#if UNITY_EDITOR
            if (UnityEditor.EditorUtility.IsPersistent(this))
            {
                Debug.Log("Error is Persistent");
            }
#endif
            if (isRunning)
            {

            }
            if (agent == null)
            {
                Debug.LogWarning("Owner is null");
            }
            if (worldState == null)
            {
                if (agent != null)
                {
                    worldState = agent.GetComponent(typeof(IWorldState)) as IWorldState;
                }
            }
            this.agent = agent;
            this.worldState = worldState;
            UpdateReferences();
            UpdateNeighbourds();
            UpdateGoalRevelance();
            CreatePlan();
            isRunning = true;

            /*
             * StartNodes
             * 
             */


        }
        bool CreatePlan()
        {
            if (planner.GeneratePlan(priorityGoal))
            {
                SetNewPlan();
                return true;
            }
            return false;
        }
        void CreatePlanTwo()
        {
            if (currentGoal == null)
            {
                Debug.Log("No goal");
                return;
            }
            planner.FindPlanTwo();
            if (planner.isPlanning)
            {
                plan_position = 0;
                currentNode = currentPlan[plan_position++];
            }
        }
        public bool ContinuePlan()
        {
            currentNode.Deactivate(agent, worldState);
            if (plan_position >= currentPlan.Count())
            {
                return false;
            }

            currentNode = currentPlan[plan_position++];
            return true;
        }
        public void UpdateGoap()
        {
            UpdateGoalRevelance();

            if (planner.noPlan && priorityGoal!=null)
            {
                
                CreatePlan();
            }
            if (planner.isPlanning)
            {
                if (isPriorityGoalChange)
                {
                    
                    if (PlanInterrupt(agent, worldState))
                    {
                        if(CreatePlan())
                        {
                            currentGoal = _priorityGoal;
                            return;
                        }
                    }
                    
                }              
                if (PlanUpdate(agent, worldState) != Status.RUNNING)
                {
                    FinishPlan();
                }
                
            }

        }
        void FinishPlan()
        {
            switch (currentNode.status)
            {
                case Status.SUCCESS:
                  //  current_goal.Execute(agent, worldState);
                    Debug.Log("Plan Exits with Succes");
                    break;
                case Status.FAILURE:
                   // current_goal.Execute(agent, worldState);

                    break;
                case Status.ERROR:
                    Debug.Log("Plan Exits with Error");

                    break;
                default:
                    break;
            }
            planner.Finish();
            priorityGoal = null;

        }
        private bool PlanInterrupt(Component agent, IWorldState worldState)
        {           
            return currentNode.Interrupt(agent,worldState);
        }
        private Status PlanUpdate(Component agent, IWorldState worldState)
        {
            if (currentNode.status != Status.RUNNING)
            {
                currentNode.Reset();
            }
          
            return currentNode.Execute(agent, worldState);
        }
        void SetNewPlan()
        {
#if UNITY_EDITOR
            if(currentPlan.Count>0)
            {
                _lastPlan.Clear();
                foreach (NodeAction action in currentPlan)
                {

                    NodeBase node = new NodeAction();
                    node.name = action.name;
                    node.status = action.myAction.status;
                    _lastPlan.Add(node);
                }
            }
#endif
            ResetPlan();
            currentPlan = planner.GetNewPlan();
            plan_position = 0;
            currentNode = currentPlan[plan_position++];

        }

        void ResetPlan()
        {
            foreach(NodeAction action in currentPlan)
            {
                action.Reset();
            }
        }
        public NodeAction AddNodeAction()
        {
            NodeBase node = NodeBase.Create(this, typeof(NodeAction));

            string str_name = node.nodeName;

            while (GetActionByName(str_name) != null)
            {
                str_name += "-";
            }
            node.name = str_name;
            allNodeActions.Add((NodeAction)node);
            return (NodeAction)node;
        }
        public NodeGoal AddNodeGoal()
        {
            NodeBase node = NodeBase.Create(this, typeof(NodeGoal));
            string str_name = node.nodeName;

            while (GetGoalByName(str_name) != null)
            {
                str_name += "-";
            }
            node.name = str_name;
            allNodeGoals.Add((NodeGoal)node);
            return (NodeGoal)node;
        }
        public void RemoveNodeAction(NodeBase node)
        {
            if (!allNodeActions.Contains((NodeAction)node))
            {
                Debug.LogWarning("Node is not found");
                return;
            }
            allNodeActions.Remove((NodeAction)node);
        }
        public void RemoveNodeGoal(NodeBase node)
        {
            if (!allNodeGoals.Contains((NodeGoal)node))
            {
                Debug.LogWarning("Node is not found");
                return;
            }
            allNodeGoals.Remove((NodeGoal)node);
        }
        NodeAction GetActionByName(string name)
        {
            foreach (NodeAction action in allNodeActions)
            {
                if (action.name == name)
                    return action;
            }
            return null;
        }
        NodeGoal GetGoalByName(string name)
        {
            foreach (NodeGoal goal in allNodeGoals)
            {
                if (goal.name == name)
                    return goal;
            }
            return null;
        }
        public string Serialize(bool prety_json, List<UnityEngine.Object> object_ref)
        {
            if (_deserealizationFail)
            {
                _deserealizationFail = false;
                return _serializedGraph;
            }
            if (object_ref == null)
                object_ref = _objectRef = new List<Object>();
            else
                object_ref.Clear();
            return JSONSerializer.Serialize(typeof(GoapGraphSerializationData), new GoapGraphSerializationData(this), prety_json, object_ref);
        }

        public GoapGraphSerializationData Deserialize(string serializedGraph, bool validate, List<UnityEngine.Object> objectRef)
        {
            if (string.IsNullOrEmpty(serializedGraph))
                return null;
            if (objectRef == null)
                objectRef = _objectRef;
            try
            {
                GoapGraphSerializationData data = JSONSerializer.Deserialize<GoapGraphSerializationData>(serializedGraph, objectRef);
                if (LoadGoapGraphSerializedData(data, validate))
                {
                    _deserealizationFail = false;
                    this._serializedGraph = serializedGraph;
                    return data;
                }
                _deserealizationFail = true;
                return null;
            }
            catch
            {
                _deserealizationFail = true;
                return null;
            }
        }
        bool LoadGoapGraphSerializedData(GoapGraphSerializationData data, bool validate)
        {
            if (data == null)
            {
                Debug.LogError("Error GoapGraphSerializationData is null");
            }
            data.SetGraphToNode(this);
            _name = data._name;
            _comments = data._comments;
            _nodeActions = data._nodeAction;
            _nodeGoals = data._nodeGoal;
            if (validate)
            {
                Validate();
            }
            return true;
        }
        private void Validate()
        {
#if UNITY_EDITOR
            if (!Application.isPlaying)
                UpdateReferences();
#endif

            foreach (NodeAction action in _nodeActions)
            {
                action.OnValidate(this);
            }
            foreach (NodeGoal goal in _nodeGoals)
            {
                goal.OnValidate(this);
            }
            foreach (NodeAction action in _nodeActions)
            {
                if (action.myFunction == null)
                    continue;
                action.myFunction.OnValidate(this);
            }
            foreach (NodeGoal goal in _nodeGoals)
            {
                if (goal.myFunction == null)
                    continue;
                goal.myFunction.OnValidate(this);
            }
        }
        private void UpdateGoalRevelance()
        {
            foreach(NodeGoal allGoals in allNodeGoals)
            {
                allGoals.CalculePriority();
            }
            List<NodeGoal> goals = new List<NodeGoal>(allNodeGoals);
            goals.Sort((value1, value2) => value1.goalPriority.CompareTo(value2.goalPriority));
            goals.Reverse();
            foreach (NodeGoal goal in goals)
            {               
                if (!goal.IsValid(agent,worldState))
                    continue;

                priorityGoal = goal;
                return;
            }
            priorityGoal = null;
        }
        public void UpdateReferences()
        {
            UpdateNodeWSField();
            SendFunctionOwnerDefault();
        }
        private void UpdateNodeWSField()
        {
            foreach (NodeAction action in _nodeActions)
            {
                WSParameter.SetWSFields(action, worldState);
              
                action.OnValidate(this);
            }
            foreach (NodeGoal goal in _nodeGoals)
            {
                WSParameter.SetWSFields(goal, worldState);
                goal.OnValidate(this);
            }

        }
        private void SendFunctionOwnerDefault()
        {
            foreach (NodeAction action in _nodeActions)
            {
                if (action.myFunction == null)
                    continue;
                action.myFunction.SetOwnerSystem(this);
            }
            foreach (NodeGoal goal in _nodeGoals)
            {
                if (goal.myFunction == null)
                    continue;
                goal.myFunction.SetOwnerSystem(this);
            }
        }
        private void UpdateNeighbourds()
        {


            foreach (var variable in worldState.myVariables)
            {
                string variable_name = variable.Value.name;
                foreach (NodeAction action in _nodeActions)
                {
  

                    foreach (PEParameter eff_param in action.effects)
                    {
                        if (eff_param.name == variable_name)
                        {
                            planner.AddNeighbour(variable_name, action);
                        }
                    }
               

                }
            }
            foreach(var function in worldState.myFunctionVariables)
            {
                string variable_name = function.Value.name;

                foreach (NodeAction action in _nodeActions)
                {
                    foreach (PEMethod eff_param in action.functionEffects)
                    {
                        if (eff_param.name == variable_name)
                        {
                            planner.AddNeighbour(variable_name, action);
                        }
                    }
                }
            }

        }
        private void ResetNeighbourds()
        {
            planner.ResetNeighbours();

           
        }

    }
}