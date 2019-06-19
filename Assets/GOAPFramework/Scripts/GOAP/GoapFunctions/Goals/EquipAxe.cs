using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GOAP.Framework;

[CategoryAttribute("AlphaTest")]
public class EquipAxe : GoalBase<Transform>
{
    /*Function: CalculePriority(int)
    * 1 if is raining or we already have the axe equiped the priority is 0 (0 is not valid)
    * 2 the principal value of priority is the diference between (maxWood and wood)+1 <--this will have always more priority that Chop so will be done first
    * 3 also if we are close to the target we gain priority
    */
    public override int CalculePriority(int priority)
    {
        if (worldState.GetTValue<bool>("isRaining")||worldState.GetTValue<bool>("hasAxe"))
            return 0;
        
        int ret = (worldState.GetTValue<int>("MaxWood") - worldState.GetTValue<int>("Wood"))+1;
        Transform tree = worldState.GetTValue<Transform>("Tree");
        if (tree != null)
        {
            int distance = Mathf.Min((int)(agent.transform.position - tree.transform.position).magnitude, 5);
            ret += (5 - distance);
        }
        return ret;

    }
    public override void OnEnd()
    {

        Debug.Log("Goal EquipAxe Complete");

    }
}