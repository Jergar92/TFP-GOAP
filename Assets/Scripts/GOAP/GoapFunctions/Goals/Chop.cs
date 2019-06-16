using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GOAP.Framework;

[CategoryAttribute("AlphaTest")]
public class Chop : GoalBase<Transform>
{
    /*Function: IsGoalValid()
        * 1 if the wood is equal or superior to maxWood the goal is not valid
    */
    public override bool IsGoalValid()
    {
        return !(worldState.GetTValue<int>("Wood") >= worldState.GetTValue<int>("MaxWood"));
    }
    /*Function: CalculePriority(int)
        * 1 if is raining the priority is 0 (0 is not valid)
        * 2 the principal value of priority is the diference between maxWood and wood
        * 3 if we have the axe equiped we gain +5 of priority
        * 4 also if we are close to the target we gain priority
    */
    public override int CalculePriority(int priority)
    {
        if ((worldState.GetTValue<bool>("isRaining")))
            return 0;
        int ret = (worldState.GetTValue<int>("MaxWood") - worldState.GetTValue<int>("Wood"));
        if (worldState.GetTValue<bool>("hasAxe"))
            ret += 5;
        Transform tree = worldState.GetTValue<Transform>("Tree");
        if(tree !=null)
        {
            int distance = Mathf.Min((int)(agent.transform.position - tree.transform.position).magnitude, 5);
            ret += (5 - distance);
        }
        return ret;


    }
    public override void OnEnd()
    {
        Debug.LogWarning("Goal Chop Complete");
    }

}