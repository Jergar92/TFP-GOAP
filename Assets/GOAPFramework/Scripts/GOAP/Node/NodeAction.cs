using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GOAP.Helper;
namespace GOAP.Framework
{
    public partial class NodeAction : NodeBase, IFunctionAssignable<ActionBase>
    {

        [SerializeField]
        private ActionBase my_action;

        [SerializeField]
        List<PEParameter> _preconditions = new List<PEParameter>();

        [SerializeField]
        List<PEMethod> _functionPreconditions = new List<PEMethod>();

        [SerializeField]
        List<PEParameter> _effects = new List<PEParameter>();
        [SerializeField]
        List<PEMethod> _functionEffects = new List<PEMethod>();
        //[SerializeField]
        //PEParameter _precondition;//

        [SerializeField]
        PEParameter _effect;//this can be an effect(action) or desire(goal)
        [SerializeField]
        private int action_cost = 1;
        [SerializeField]
        private float fail_frequency = 0.0f;
        [SerializeField]
        private bool interrumpible_action = false;
        [SerializeField]
        private bool action_cost_variable = false;

        public bool isActionCostVariable
        {
            get
            {
                return action_cost_variable;
            }
            set
            {
                action_cost_variable = value;
            }
        }
        public int actionCost
        {
            get
            {
                if (action_cost_variable && myAction != null)
                {

                    return (myAction.cost == -1) ? myAction.cost : action_cost;

                }

                return action_cost;
            }
            set
            {
                if (!action_cost_variable)
                    action_cost = value;
            }
        }
        public float failFrequency
        {
            get
            {
                return fail_frequency;
            }
            set
            {
                fail_frequency = value;
            }
        }
        public bool interrumpible
        {
            get
            {
                return interrumpible_action;
            }
            set
            {
                interrumpible_action = value;
            }
        }
        public List<PEParameter> preconditions
        {
            get { return _preconditions; }
            set { _preconditions = value; }
        }
        public List<PEMethod> functionPreconditions
        {
            get { return _functionPreconditions; }
            set { _functionPreconditions = value; }
        }

        public List<PEParameter> effects
        {
            get { return _effects; }
            set { _effects = value; }
        }
        public List<PEMethod> functionEffects
        {
            get { return _functionEffects; }
            set { _functionEffects = value; }
        }
        /*
        public List<PEParameter> preconditions
        {
            get { return _preconditions; }
            set { _preconditions = value; }
        }
        public List<PEParameter> effects
        {
            get{ return _effects; }
            set{ _effects = value; }
        }
        */
        public ActionBase myAction
        {
            get { return my_action; }
            set { my_action = value; }
        }
        public FunctionBase myFunction
        {
            get { return my_action; }
            set { my_action = (ActionBase)value; }
        }
        override public void OnValidate(GoapGraph goap_graph)
        {
            if (goap_graph.worldState == null)
                return;

            /*
            if(precondition!=null)
            precondition.worldState = goap_graph.worldState;
            if(effect!=null)
            effect.worldState = goap_graph.worldState;
            */

            foreach (PEParameter item in preconditions)
            {
                item.worldState = goap_graph.worldState;
            }

            foreach (PEParameter item in effects)
            {
                item.worldState = goap_graph.worldState;
            }
            foreach (PEMethod item in functionPreconditions)
            {
                item.worldState = goap_graph.worldState;
            }

            foreach (PEMethod item in _functionEffects)
            {
                item.worldState = goap_graph.worldState;
            }


        }  
        public bool Deactivate(Component agent, IWorldState world_state)
        {
            if (myAction == null)
                return false;
            myAction.DeactivateAction();
            return true;
        }
        public bool Interrupt(Component agent, IWorldState world_state)
        {
            if(!interrumpible || myAction==null)
            {
                return false;
            }
            return myAction.IsActionInterruptible();
        }
        public Status Execute(Component agent, IWorldState worldState)
        {

            if (!IsValid(agent, worldState))
            {
                status = Status.FAILURE;
                return status;
            }
            status = OnExecute(agent, worldState);
            // Debug.Log("Execute Exit: " + status.ToString());
            return status;
        }
        protected override Status OnExecute(Component agent, IWorldState worldState)
        {

            if (myAction == null)
            {
                return Status.ERROR;
            }

            if (status == Status.RUNNING || status == Status.START)
            {
            
                status = myAction.ExecuteAction(agent, worldState);
                if (status != Status.RUNNING)
                    return OnActionEnd(agent, worldState,myAction.status == Status.SUCCESS ? true : false);
            }

            return status;
        }
        protected override void OnReset()
        {
            if (myAction == null || myAction.status == Status.START)
            {
                return;
            }
            myAction.DeactivateAction();
            myAction.status = Status.START;
          
        }
        Status OnActionEnd(Component agent, IWorldState worldState, bool success)
        {

            if (success)
            {
           
                if (IsMethodEffectsPosible(agent))
                {
                    
                    SetEffect();
                    if (goapGraph.ContinuePlan())
                    {
                        return Status.RUNNING;
                    }
                    return Status.SUCCESS;
                }
            }
    
            return Status.FAILURE;

        }

        bool IsMethodEffectsPosible(Component agent)
        {          
            foreach(PEMethod method in functionEffects)
            {
                if (!method.IsMethodValid())
                    return false;
            }            
            return true;
        }
        void SetEffect()
        {
            foreach (PEParameter parameter in effects)
            {
                WSVariable variable = worldState.GetVariable(parameter.name);
                variable.value = parameter.value;
            }

        }


        override public bool IsValid(Component agent, IWorldState worldState)
        {
            if (myAction == null)
            {
                Debug.Log("Action Invalid, myAction is null");
                return false;
            }

            return MeetPreconditions(agent,worldState) && myAction.IsActionValid();
        }

        bool MeetPreconditions(Component agent, IWorldState worldState)
        {
            if (preconditions.Count <= 0)
                return true;
            foreach (PEParameter parameter in preconditions)
            {
                WSVariable variable = worldState.GetVariable(parameter.name);
                if (!parameter.value.Equals(variable.value))
                    return false;
            }
            foreach (PEMethod method in functionPreconditions)
            {
                if (!method.IsMethodValid())
                    return false;
            }
            //Debug.Log("Preconditions meet");
            return true;
        }
        override protected void AddPrecondition(WSVariable variable)
        {
            var data_type = typeof(PEParameter<>).RHelperGenericType(new Type[] { variable.myType });
            var newData = (PEParameter)Activator.CreateInstance(data_type);

            newData.worldState = worldState;
            newData.variableReference = variable;

            preconditions.Add(newData);

            //precondition = new_data;

        }
        override protected void RemovePrecondition(PEParameter variable)
        {

            PEParameter remove = variable;
            if (preconditions.Remove(remove))
                Debug.Log(string.Format("Parameter in Preconditions: ({0}) Removed", remove.name));

            // precondition = null;

        }
        override protected void AddEffect(WSVariable variable)
        {
            var data_type = typeof(PEParameter<>).RHelperGenericType(new Type[] { variable.myType });
            var newData = (PEParameter)Activator.CreateInstance(data_type);

            newData.worldState = worldState;
            newData.variableReference = variable;

            effects.Add(newData);

            //effect = new_data;
        }
        protected override void RemoveEffect(PEParameter variable)
        {
            PEParameter remove = variable;
            if (effects.Remove(remove))
                Debug.Log(string.Format("Parameter in Effect: ({0}) Removed", remove.name));

            // effect = null;

        }
        protected override void AddPrecondition(WSFunctionVariable method)
        {
            var newData = new PEMethod();

            newData.methodReference = method;
            newData.worldState = worldState;
            functionPreconditions.Add(newData);
        }
        protected override void RemovePrecondition(PEMethod method)
        {
            PEMethod remove = method;
            if (functionPreconditions.Remove(remove))
                Debug.Log(string.Format("Parameter in Effect: ({0}) Removed", remove.name));
        }
        protected override void AddEffect(WSFunctionVariable method)
        {
            var newData = new PEMethod();

            newData.worldState = worldState;
            newData.methodReference = method;
            functionEffects.Add(newData);
        }
        protected override void RemoveEffect(PEMethod method)
        {
            PEMethod remove = method;
            if (functionEffects.Remove(remove))
                Debug.Log(string.Format("Parameter in Effect: ({0}) Removed", remove.name));
        }

        protected override void AddDesire(WSFunctionVariable variable){ }
        protected override void RemoveDesire(PEMethod variable){ }
        override protected void AddDesire(WSVariable variable) { }
        protected override void RemoveDesire(PEParameter variable) { }
        public override void OnDestroy() { }

    }
}