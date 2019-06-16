
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
using UnityEngine;
using System.Threading;
using GOAP.Helper;
using GOAP.Framework.Internal;

using GOAP;
namespace GOAP.Framework
{

    public class PlannerNode : IComparable<PlannerNode>
    {
        public PlannerNode(NodeBase node, int g = 0, int h = 0, int f = 0, PlannerNode parent = null)
        {
            _node = node;
            _g = g;
            _h = h;
            _f = f;
            _parent = parent;
        }

        private NodeBase _node;
        private WorldStateData _desireState = null;
        private WorldStateData _currentState = null;

        private int _g;
        private int _h;
        private int _f;
        private PlannerNode _parent = null;

        public NodeBase node
        {
            get { return _node; }
        }
        public int CompareTo(PlannerNode other)
        {
            if (g < other.g)
            {
                return 1;
            }
            if (g > other.g)
            {
                return -1;
            }
            return 0;
        }
        public WorldStateData desireState
        {
            get
            {
                if (_desireState == null)
                    _desireState = new WorldStateData();
                return _desireState;
            }
            set { _desireState = value; }

        }
        public WorldStateData currentState
        {
            get
            {
                if (_currentState == null)
                    _currentState = new WorldStateData();
                return _currentState;
            }
            set { _currentState = value; }

        }
        public int g
        {
            get { return _g; }
            set { _g = value; }

        }
        public int h
        {
            get { return _h; }
            set { _h = value; }
        }
        public int f
        {
            get { return _f; }
            set { _f = value; }
        }
        public PlannerNode parent
        {
            get { return _parent; }
            set { _parent = value; }
        }
    }

    public class GoapPlanner
    {


        private readonly int heuristic = 10;
        public class PlannerStorage
        {

            private PriorityQueue _openList = new PriorityQueue();
            private List<PlannerNode> _closedList = new List<PlannerNode>();

            public PriorityQueue openList
            {
                get { return _openList; }
                set { _openList = value; }
            }
            public List<PlannerNode> closedList
            {
                get { return _closedList; }
                set { _closedList = value; }
            }
            public void Clean()
            {
                _openList.Clear();
                _closedList.Clear();
            }

        }


        private Dictionary<string, List<NodeAction>> neighbour_container = new Dictionary<string, List<NodeAction>>();

        private PlanStatus _plannerStatus = PlanStatus.NO_PLAN;
        private NodeGoal _priorityGoal = null;
        private IWorldState _worldState = null;
        WorldStateData _currentWorldState;
        WorldStateData _desiredWorldState;
        List<NodeAction> plan = new List<NodeAction>();

        private PlannerStorage storage = new PlannerStorage();
        private GoapGraph graph;
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
        public PlanStatus plannerStatus
        {
            get
            {
                return _plannerStatus;
            }
        }
        public IWorldState worldState
        {
            get { return _worldState; }
            set { _worldState = value; }
        }
        public bool noPlan
        {
            get
            {
                if (plannerStatus == PlanStatus.NO_PLAN)
                    return true;

                return false;
            }
        }
        public bool lookingPlan
        {
            get
            {
                if (plannerStatus == PlanStatus.LOKING_PLAN)
                    return true;

                return false;
            }
        }
        public bool isPlanning
        {
            get
            {
                if (plannerStatus == PlanStatus.PLAN)
                    return true;

                return false;
            }
        }
        public WorldStateData currentWorldState
        {
            get
            {
                if (_currentWorldState == null)
                    _currentWorldState = new WorldStateData();
                return _currentWorldState;
            }
            set { _currentWorldState = value; }
        }
        public WorldStateData desiredWorldState
        {
            get
            {
                if (_desiredWorldState == null)
                    _desiredWorldState = new WorldStateData();
                return _desiredWorldState;
            }
            set { _desiredWorldState = value; }
        }
        public void AddNeighbour(string key, NodeAction value)
        {
            if (!neighbour_container.ContainsKey(key))
                neighbour_container[key] = new List<NodeAction>();
            neighbour_container[key].Add(value);
        }
        public void ResetNeighbours()
        {
            neighbour_container.Clear();
        }
        public void Finish()
        {
            _plannerStatus = PlanStatus.NO_PLAN;

        }
        public bool GeneratePlan(NodeGoal priorityGoal)
        {
            plan.Clear();
            if (FindPlan(priorityGoal))
            {
                return true;
            }
            return false;
        }
        public bool FindPlan(NodeGoal priorityGoal)
        {
            if (priorityGoal == null)
                return false;
            storage.Clean();
            _plannerStatus = PlanStatus.LOKING_PLAN;

            NodeGoal nodeGoal = priorityGoal;
            PlannerNode firstNode = new PlannerNode(nodeGoal);
            AddParameters(firstNode, nodeGoal.desires);
            AddParameters(firstNode, nodeGoal.functionDesires);

            if (PlannerComplete(firstNode))
            {
                _plannerStatus = PlanStatus.NO_PLAN;
                return false;
            }
            storage.openList.Push(firstNode);
            while (storage.openList.Count > 0)
            {
                PlannerNode currentNode = storage.openList.Pull();
                if (PlannerComplete(currentNode))
                {
                    GeneratePlan(currentNode);
                    _plannerStatus = PlanStatus.PLAN;
                    return true;
                }

                storage.closedList.Add(currentNode);

                foreach (NodeAction neighbour in GetNeighbours(currentNode))
                {
                    int cost = currentNode.g + neighbour.actionCost;
                    int heuristic = CalculeH(currentNode);
                    int f = cost + heuristic;
                    int openIndex = storage.openList.FindIndex(neighbour);
                    int closedIndex = storage.closedList.FindIndex(node => node.node == neighbour);

                    if (openIndex > 0)
                        Debug.Log("new F = " + f + "Old F = " + storage.openList[openIndex].f);
                    if (openIndex > 0 && f < storage.openList[openIndex].f)
                    {

                        storage.openList.RemoveAt(openIndex);
                        openIndex = -1;
                    }

                    if (closedIndex > 0 && f < storage.closedList[closedIndex].f)
                    {
                        storage.closedList.RemoveAt(closedIndex);
                        closedIndex = -1;
                    }


                    if (openIndex == -1 && closedIndex == -1)
                    {
                        PlannerNode plannerNode = new PlannerNode(neighbour);
                        plannerNode.g = cost;
                        plannerNode.h = heuristic;
                        plannerNode.f = f;
                        AddNewState(currentNode, plannerNode);
                        plannerNode.parent = currentNode;
                        storage.openList.Push(plannerNode);
                    }

                }
            }
            return false;
        }

        public void FindPlanTwo()
        {
            storage.Clean();
            _plannerStatus = PlanStatus.LOKING_PLAN;

            NodeGoal nodeGoal = goapGraph.currentGoal;
            PlannerNode firstNode = new PlannerNode(nodeGoal);
            AddParameters(firstNode, nodeGoal.desires);
            storage.openList.Push(firstNode);
            while (storage.openList.Count > 0)
            {
                PlannerNode currentNode = storage.openList.Pull();
                if (PlannerComplete(currentNode))
                {
                    GeneratePlanTwo(currentNode);
                    _plannerStatus = PlanStatus.PLAN;
                    return;
                }

                storage.closedList.Add(currentNode);

                foreach (NodeAction neighbour in GetNeighbours(currentNode))
                {
                    int cost = currentNode.g + neighbour.actionCost;
                    int heuristic = CalculeH(currentNode);
                    int f = cost + heuristic;
                    int openIndex = storage.openList.FindIndex(neighbour);
                    int closedIndex = storage.closedList.FindIndex(node => node.node == neighbour);

                    if (openIndex > 0)
                        Debug.Log("new F = " + f + "Old F = " + storage.openList[openIndex].f);
                    if (openIndex > 0 && f < storage.openList[openIndex].f)
                    {

                        storage.openList.RemoveAt(openIndex);
                        openIndex = -1;
                    }

                    if (closedIndex > 0 && f < storage.closedList[closedIndex].f)
                    {
                        storage.closedList.RemoveAt(closedIndex);
                        closedIndex = -1;
                    }


                    if (openIndex == -1 && closedIndex == -1)
                    {
                        PlannerNode plannerNode = new PlannerNode(neighbour);
                        plannerNode.g = cost;
                        plannerNode.h = heuristic;
                        plannerNode.f = f;
                        AddNewState(currentNode, plannerNode);
                        plannerNode.parent = currentNode;
                        storage.openList.Push(plannerNode);
                    }

                }
            }
        }
        void AddParameters(PlannerNode node, List<PEParameter> parameters)
        {
            foreach (PEParameter parameter in parameters)
            {
                node.currentState.AddVariable(parameter.variableReference);
                node.desireState.AddVariable(parameter.name, parameter.value);
            }

        }
        void AddParameters(PlannerNode node, List<PEMethod> methods)
        {
            foreach (PEMethod method in methods)
            {
                var currentMethod = node.currentState.AddFunctionVariable(method.methodReference, method.InvokeMethod());
                currentMethod.arguments = method.GetArguments();
                var desireMethod = node.desireState.AddFunctionVariable(method.methodReference, method.value);
                desireMethod.arguments = method.GetArguments();

            }
        }
        void ApplyParameters(PlannerNode node, List<PEParameter> parameters)
        {
            foreach (PEParameter parameter in parameters)
            {
                WSVariable variable = node.currentState.GetVariable(parameter.name);
                if (variable == null)
                    continue;
                variable.value = parameter.value;
            }
        }
        void ApplyParameters(PlannerNode node, List<PEMethod> methods)
        {
            foreach (PEMethod method in methods)
            {
                WSFunctionVariable variable = node.currentState.GetFunctionVariable(method.name);
                if (variable == null)
                    continue;
                bool sameMethod = true;
                for(int i =0;i< variable.arguments.Length;i++)
                {
                    if(!variable.arguments[i].Equals(method.GetArguments()[i]))
                    {
                        sameMethod = false;
                    }
                }
                if (!sameMethod)
                    continue;
                variable.value = method.value;
            }
        }
        void AddNewState(PlannerNode currentNode, PlannerNode neightbourNode)
        {
            neightbourNode.currentState = currentNode.currentState;
            neightbourNode.desireState = currentNode.desireState;
            NodeAction action = neightbourNode.node as NodeAction;

            ApplyParameters(neightbourNode, action.effects);
            ApplyParameters(neightbourNode, action.functionEffects);

            AddParameters(neightbourNode, action.preconditions);
            AddParameters(neightbourNode, action.functionPreconditions);


        }
        bool PlannerComplete(PlannerNode plannerNode)
        {
            return LookVariables(plannerNode) && LookMethods(plannerNode);
        }
        bool LookVariables(PlannerNode plannerNode)
        {
            string[] variableNames = plannerNode.currentState.GetVariableNames();
      //      Debug.Log("PlannerComplete, looking = " + plannerNode.node.name + " Count = " + variableNames.Length);

            foreach (string variableName in variableNames)
            {
                var currentVariable = plannerNode.currentState.GetVariable(variableName);
                var desiredVariable = plannerNode.desireState.GetVariable(variableName);
          
          //      Debug.Log("PlannerComplete, variable = " + variableName);
         //       Debug.Log("currentVariable = " + currentVariable.value);
         //       Debug.Log("currentVariable = " + desiredVariable.value);

                if (!currentVariable.value.Equals(desiredVariable.value))
                {
             //       Debug.Log("PlannerComplete, Exit with FALSE");

                    return false;
                }
            }
         //   Debug.Log("PlannerComplete, Exit with TRUE");

            return true;
        }
        bool LookMethods(PlannerNode plannerNode)
        {
            string[] variableNames = plannerNode.currentState.GetFunctionVariableNames();
         //   Debug.Log("PlannerComplete, looking = " + plannerNode.node.name + " Count = " + variableNames.Length);

         
            foreach (string variableName in variableNames)
            {
                var currentVariable = plannerNode.currentState.GetFunctionVariable(variableName);
                var desiredVariable = plannerNode.desireState.GetFunctionVariable(variableName);
         
               // Debug.Log("PlannerComplete, variable = " + variableName);
              //  Debug.Log("currentVariable = " + currentVariable.value);
              //  Debug.Log("currentVariable = " + desiredVariable.value);

                if (!currentVariable.value.Equals(desiredVariable.value))
                {
              //      Debug.Log("PlannerComplete, Exit with FALSE");

                    return false;
                }
            }
           // Debug.Log("PlannerComplete, Exit with TRUE");

            return true;
        }
        void GeneratePlan(PlannerNode firstNode)
        {
            PlannerNode item = null;
            for (item = firstNode; item.parent != null; item = item.parent)
            {
                plan.Add((NodeAction)item.node);

            }
        //    Debug.Log("-------------->Plan Start");

            foreach (NodeBase node in plan)
            {
     //           Debug.Log(node.name);
            }
      //      Debug.Log("-------------->Plan End");

        }
        public List<NodeAction> GetNewPlan()
        {
            List<NodeAction> ret= new List<NodeAction>(plan);
            plan.Clear();
            return ret;
        }
        void GeneratePlanTwo(PlannerNode firstNode)
        {
            List<NodeAction> plan = new List<NodeAction>();
            PlannerNode item = null;
            for (item = firstNode; item.parent != null; item = item.parent)
            {
                plan.Add((NodeAction)item.node);

            }
       //     Debug.Log("-------------->Plan Start");

            foreach (NodeBase node in plan)
            {
    //            Debug.Log(node.name);
            }
        //    Debug.Log("-------------->Plan End");

            graph.currentPlan = plan;

        }
        int CalculeH(PlannerNode plannerNode)
        {
            int count = 0;
            string[] variable_names = plannerNode.currentState.GetVariableNames();
            foreach (string variable_name in variable_names)
            {

                var current_variable = plannerNode.currentState.GetVariable(variable_name);
                var desired_variable = plannerNode.desireState.GetVariable(variable_name);
                if (!current_variable.value.Equals(desired_variable.value))
                {
                    count++;
                }
            }
            return count * heuristic;
        }
        List<NodeAction> GetNeighbours(PlannerNode parent)
        {

            List<NodeAction> actions = new List<NodeAction>();

            string[] variableNames = GetNeededParameters(parent);
            foreach (string variableName in variableNames)
            {
                foreach (var action in neighbour_container[variableName])
                {
                    foreach (PEParameter effect in action.effects)
                    {
                        WSVariable variable = parent.desireState.GetVariable(variableName);
                        if (variable == null)
                            continue;
                        if (effect.value.Equals(variable.value))
                            actions.Add(action);
                    }
                    foreach (PEMethod effect in action.functionEffects)
                    {
                        WSFunctionVariable variable = parent.desireState.GetFunctionVariable(variableName);
                        if (variable == null)
                            continue;
                        bool sameMethod = false;
                        for (int i = 0; i < variable.arguments.Length; i++)
                        {

                            if (variable.arguments[i].Equals(effect.GetArguments()[i]))
                            {
                                if (variable.arguments[i].GetHashCode().Equals(effect.GetArguments()[i].GetHashCode()))
                                {
                                    sameMethod = true;
                                }
                            }
                        }
                        if (!sameMethod)
                            continue;
                        if (effect.value.Equals(variable.value))
                            actions.Add(action);
                    }
                }
            }
            return actions;
        }
        string[] GetNeededParameters(PlannerNode node)
        {
            List<string> parameters = new List<string>();
            string[] variable_names = node.currentState.GetVariableNames();
            foreach (string variable_name in variable_names)
            {

                var current_variable = node.currentState.GetVariable(variable_name);
                var desired_variable = node.desireState.GetVariable(variable_name);
                if (!current_variable.value.Equals(desired_variable.value))
                {
                    parameters.Add(variable_name);
                }
            }

            string[] methodNames = node.currentState.GetFunctionVariableNames();
            foreach (string methodName in methodNames)
            {

                var current_variable = node.currentState.GetFunctionVariable(methodName);
                var desired_variable = node.desireState.GetFunctionVariable(methodName);
                if (!current_variable.value.Equals(desired_variable.value))
                {
                   
                    parameters.Add(methodName);
                    
                }
            }
            return parameters.ToArray();
        }
        bool GetPriorityGoal()
        {
            _plannerStatus = PlanStatus.LOKING_PLAN;

            _priorityGoal = goapGraph.currentGoal;

            return false;
        }

        void DebugState(PlannerNode plannerNode)
        {
            Debug.Log("DebugState");
            Debug.Log("name = " + plannerNode.node.name);
            string[] variableNames = currentWorldState.GetVariableNames();

            foreach (string variableName in variableNames)
            {

                Debug.Log(variableName);
            }
            Debug.Log("Exit Debug State");
        }


    }
}