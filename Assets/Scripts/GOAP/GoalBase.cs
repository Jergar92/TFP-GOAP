using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace GOAP.Framework
{
    public class GoalBase<T> : GoalBase where T : Component
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
    public class GoalBase : FunctionBase
    {
        [SerializeField]
        private float relevance = 1.0f;

        public void EndGoal()
        {

            OnEnd();
        }
        /*Function: OnEnd()
        * Override this function if you want to add functionality that don't make the actions.
        * Demo examples in all goals scripts, but they are only log's to prove that they are executed properly
        * 
        */
        public virtual void OnEnd()
        {

        }
        /*Function: CalculePriority(int)
        * Override this function if you want to make the priority of the goal dinamic (recommended),
        * return .
        * Demo examples in all goals scripts
        * 
        */
        public virtual int CalculePriority(int priority)
        {
            return priority;
        }
        /*Function: IsGoalValid()
         * Override this function if you want to create an specific validation for this goal,
         * not valid goals will be ignored in the priority selection return true if is valid false if not.
         * Demo examples in RemoveHunger.cs, Chop.cs, GetWarm.cs and RecolectFruit.cs
         * 
         */
        public virtual bool IsGoalValid()
        {
            return true;
        }

    }

}