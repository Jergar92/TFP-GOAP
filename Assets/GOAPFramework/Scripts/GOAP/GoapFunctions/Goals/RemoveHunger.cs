using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GOAP.Framework;

[CategoryAttribute("AlphaTest")]

public class RemoveHunger : GoalBase<Transform>
{
    /*Function: IsGoalValid()
        * 1 if we don't have any seeds the plan is not valid
    */
    public override bool IsGoalValid()
    {
        return !(worldState.GetTValue<int>("Seeds") < 0);
    }
    /*Function: CalculePriority(int)
    * 1 The value is the hunger of the player * 2 (to get higher values and get priority fast)
    */
    public override int CalculePriority(int priority)
    {
        return worldState.GetTValue<int>("Hunger")*2;
    }
    public override void OnEnd()
    {

        Debug.Log("Goal RemoveHunger Complete");

    }
}