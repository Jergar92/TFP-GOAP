using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GOAP.Helper;
namespace GOAP.Framework
{
    public partial class NodeGoal : NodeBase, IFunctionAssignable<GoalBase>
    {
        [SerializeField]
        private GoalBase my_goal;

        [SerializeField]
        private int _goalPriority = 1;
        // [SerializeField]
        //PEParameter _desire;//this can be an effect(action) or desire(goal)


        [SerializeField]
        List<PEParameter> _desires = new List<PEParameter>();

        [SerializeField]
        List<PEMethod> _functionDesires = new List<PEMethod>();

        public int goalPriority
        {
            get
            {
                return _goalPriority;
            }
            set
            {
                _goalPriority = value;
            }
        }
        public List<PEParameter> desires
        {
            get
            {
                return _desires;
            }
            set
            {
                _desires = value;
            }
        }
        public List<PEMethod> functionDesires
        {
            get
            {
                return _functionDesires;
            }
            set
            {
                _functionDesires = value;
            }
        }
        /*
        public List<PEParameter> desires
        {
            get
            {
                return _desires;
            }
            set
            {
                _desires = value;
            }
        }
        */

        public GoalBase myGoal
        {
            get { return my_goal; }
            set { my_goal = value; }
        }
        public FunctionBase myFunction
        {
            get { return my_goal; }
            set { my_goal = (GoalBase)value; }
        }

        override public void OnValidate(GoapGraph goap_graph)
        {
            if (goap_graph.worldState == null)
                return;
            // if(desire!=null)
            // desire.worldState = goap_graph.worldState;

            foreach (PEParameter item in desires)
            {
                item.worldState = goap_graph.worldState;
            }
            foreach (PEMethod item in _functionDesires)
            {
                item.worldState = goap_graph.worldState;
            }
        }
        override public bool IsValid(Component agent, IWorldState world_state)
        {
            
            if (myGoal == null && goalPriority!=0)
                return false;

            return myGoal.IsGoalValid();
            
        }
        bool MeetDesire(Component agent, IWorldState worldState)
        {
            foreach (PEParameter parameter in desires)
            {
                WSVariable variable = worldState.GetVariable(parameter.name);
                if (!variable.value.Equals(parameter.value))
                    return false;
            }
            foreach (PEMethod method in functionDesires)
            {
                if (!method.IsMethodValid())
                    return false;
            }
            return true;
        }
        public void CalculePriority()
        {
            if (myGoal == null)
            {
                goalPriority = 0;
            }
            goalPriority = myGoal.CalculePriority( goalPriority);
        }
        protected override Status OnExecute(Component agent, IWorldState world_state)
        {

            if (myGoal == null)
            {
                return Status.ERROR;
            }
            OnGoalEnd(true);
           status = Status.RUNNING;
            return status;
        }
        protected override void OnReset()
        {
        }
        IEnumerator UpdateGoal(Component action_agent)
        {

            yield return null;
        }
        Status OnGoalEnd(bool success)
        {

            if (success)
            {
                //if(!goapGraph.ApplyEffectToWorldState(effect))
                //      {
                //           status = Status.FAILURE;
                //          return;
                //      }
                //      status = Status.SUCCESS;

                myGoal.EndGoal();

                return Status.SUCCESS;

                //      return;
            }
            myGoal.EndGoal();
            //  status = Status.FAILURE;
            //   goapGraph.StopPlan();
            return Status.FAILURE;

        }
        protected override void AddPrecondition(WSVariable variable) { }
        protected override void RemovePrecondition(PEParameter variable) { }
        protected override void AddEffect(WSVariable variable) { }
        protected override void RemoveEffect(PEParameter variable) { }
        protected override void AddPrecondition(WSFunctionVariable variable) { }
        protected override void RemovePrecondition(PEMethod variable) { }
        protected override void AddEffect(WSFunctionVariable variable) { }
        protected override void RemoveEffect(PEMethod variable) { }


        protected override void AddDesire(WSVariable variable)
        {
            var data_type = typeof(PEParameter<>).RHelperGenericType(new Type[] { variable.myType });
            var new_data = (PEParameter)Activator.CreateInstance(data_type);

            new_data.worldState = worldState;
            new_data.variableReference = variable;
            desires.Add(new_data);
            //desire = new_data;
        }
        protected override void RemoveDesire(PEParameter variable)
        {

            PEParameter remove = variable;


            if (desires.Remove(remove))
                Debug.Log(string.Format("Parameter in Desires: ({0}) Removed", remove.name));

            //  desire = null;
        }
        protected override void AddDesire(WSFunctionVariable method)
        {
            var newData = new PEMethod();

            newData.methodReference = method;
            newData.worldState = worldState;
            functionDesires.Add(newData);
        }
        protected override void RemoveDesire(PEMethod method)
        {
            PEMethod remove = method;
            if (functionDesires.Remove(remove))
                Debug.Log(string.Format("Parameter in Effect: ({0}) Removed", remove.name));

        }
        public override void OnDestroy()
        {
        }
    }
}