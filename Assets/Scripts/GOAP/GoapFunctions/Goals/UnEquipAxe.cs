using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GOAP.Framework;

[CategoryAttribute("AlphaTest")]
public class UnEquipAxe : GoalBase<Transform>
{
    /*Function: CalculePriority(int)
    * 1 if is raining or we don't have the axe equiped the priority is 0 (0 is not valid)
    * 2 the principal value of priority is the diference between (MaxSeeds and Seeds)+1 <--this will have always more priority that RecolectFruit so will be done first
    * 3 also if we are close to the target we gain priority
    */
    public override int CalculePriority(int priority)
    {
        if (worldState.GetTValue<bool>("isRaining") || !worldState.GetTValue<bool>("hasAxe"))
            return 0;

        int ret = (worldState.GetTValue<int>("MaxSeeds") - worldState.GetTValue<int>("Seeds"))+1;

        Transform fruit = worldState.GetTValue<Transform>("Fruit");
        if (fruit != null)
        {
            int distance = Mathf.Min((int)(agent.transform.position - fruit.transform.position).magnitude, 5);
            ret += (5 - distance);
        }
        return ret;

    }
    public override void OnEnd()
    {

        Debug.Log("Goal UnEquipAxe Complete");

    }
}