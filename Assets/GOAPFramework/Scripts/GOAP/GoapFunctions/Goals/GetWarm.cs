using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GOAP.Framework;

[CategoryAttribute("AlphaTest")]
public class GetWarm : GoalBase<Transform>
{
    /*Function: IsGoalValid()
     * 1 if we don't have wood the plan is not valid
    */
    public override bool IsGoalValid()
    {
        return (worldState.GetTValue<int>("Wood") >0);
    }
    /*Function: CalculePriority(int)
    * 1 The value is the cold of the player * 2 (to get higher values and get priority fast)
    */
    public override int CalculePriority(int priority)
    {    
        
        return worldState.GetTValue<int>("Cold")*2;
    }
    public override void OnEnd()
    {

        Debug.Log("Goal GetWarm Complete");

    }
}