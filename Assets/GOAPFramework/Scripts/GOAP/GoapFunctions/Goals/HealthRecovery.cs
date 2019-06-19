using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GOAP.Framework;

[CategoryAttribute("AlphaTest")]
public class HealthRecovery : GoalBase<Transform>
{
    /*Function: CalculePriority(int)
        * 1 The value is the diference between maxHealth and health
        * 2 Also we gain an extra 5 points if is night
    */
    public override int CalculePriority(int priority)
    {
        int ret = (worldState.GetTValue<int>("MaxHealth") - worldState.GetTValue<int>("Health"));
        if ((worldState.GetTValue<bool>("isNight")))
            ret += 5;
        return ret;
      
    }
    public override void OnEnd()
    {

        Debug.Log("Goal HealthRecovery Complete");

    }
}