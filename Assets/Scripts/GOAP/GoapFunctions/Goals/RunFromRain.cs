using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GOAP.Framework;

[CategoryAttribute("AlphaTest")]
public class RunFromRain : GoalBase<Transform>
{
    /*Function: CalculePriority(int)
      * 1 if is raining we get a 5 if not a 0
    */
    public override int CalculePriority(int priority)
    {
        return (worldState.GetTValue<bool>("isRaining")) ? 5 : 0;
      
    }
    public override void OnEnd()
    {

        Debug.Log("Goal RunFromRain Complete");

    }
}