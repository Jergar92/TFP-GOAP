using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using GOAP;
namespace GOAP.Framework
{
    public class ActionBase<T> : ActionBase where T : Component
    {
        public sealed override Type agentType
        {
            get
            {
                return typeof(T);
            }
        }
        new public T agent
        {
            get
            {
                return base.agent as T;
            }
        }
    }
    public class ActionBase : FunctionBase
    {

        private Status _status = Status.START;

        public Status status
        {
            get { return _status; }
            set { _status = value; }
        }
        //Init
        public int cost
        {
            get
            {
                return ActionCost();
            }
        }

        public virtual void InitAction()
        {

        }
        //Plan Action

     
        //Checks
        public bool IsActionValid()
        {
            return ValidateContextPrecondition();
        }
        public Status ExecuteAction(Component agent, IWorldState world_state)
        {
            if (!IsActionValid())
                return Status.FAILURE;

            if (status == Status.RUNNING)
            {
                OnUpdate();
                return status;
            }
            if (!Set(agent, world_state))
            {
                return Status.FAILURE;
            }
            
            _status = Status.RUNNING;
            OnActivate();
            if (status == Status.RUNNING)
                OnUpdate();

            return status;
        }
        protected void EndAction(bool succes)
        {

            _status = succes ? Status.SUCCESS : Status.FAILURE;
        }

        /*Function: ActionCost()
        * In order to this function been call you need to set in true the checkbox "V" 
        * in the editor windows this make the cost of the action variable
        * Make your calculation and return the new value
        */
        protected virtual int ActionCost()
        {
            return -1;
        }
        //

        public void DeactivateAction()
        {
            OnDeactivate();
        }
        /*Function: OnActivate()
        * If you action needs to be updated this functions needs to be override, 
        * if not the action will end with succes inmediatly, this function is call
        * at the start of the initialization of the action if you need to reset values
        * or maybe prepare other variables this is the place
        * 
        */
        protected virtual void OnActivate()
        {

            EndAction(true);

        }
        /*Function: OnDeactivate()
        * Call after EndAction this is the place to deactivate the thing 
        * that you activate on OnActivate if is needed
        * 
        */
        protected virtual void OnDeactivate()
        {

        }
        /*Function: OnUpdate()
          * Update the action, to finish the action with succes use EndAction(true) to finish with failure EndAction(false)
          * 
          */
        protected virtual void OnUpdate()
        {
            EndAction(true);

        }
        /*Function: ValidateContextPrecondition()
        * If you can't validate an action with the preconditions of the editor windows then you
        * need to override this function and create a piece of specific code that validate this action,
        * return true if is valid and false if not.
        * 
        */
        protected virtual bool ValidateContextPrecondition()
        {
            return true;
        }
        /*Function: IsActionInterruptible()
        * In order to this function been call you need to set in true the checkbox "Interrumpible" in the editor windows
        * If you don't want that this function interrupts inmediatly then you need to override 
        * this function and only return true went is the right moment to interrupt the action
        * 
        */
        public virtual bool IsActionInterruptible()
        {
            return true;
        }
    }
}