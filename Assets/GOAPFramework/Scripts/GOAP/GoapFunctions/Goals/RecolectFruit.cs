using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GOAP.Framework;

[CategoryAttribute("AlphaTest")]

public class RecolectFruit : GoalBase<Transform>
{
    /*Function: IsGoalValid()
        * 1 if the number of seeds is equal or superior to maxSeeds the goal is not valid
    */
    public override bool IsGoalValid()
    {  
        return !(worldState.GetTValue<int>("Seeds")>= worldState.GetTValue<int>("MaxSeeds"));
    }
    /*Function: CalculePriority(int)
      * 1 if is raining the priority is 0 (0 is not valid)
      * 2 the principal value of priority is the diference between MaxSeeds and Seeds
      * 3 if we don't have the axe equiped we gain +5 of priority
      * 4 also if we are close to the target we gain priority
  */
    public override int CalculePriority(int priority)
    {
        if ((worldState.GetTValue<bool>("isRaining")))
            return 0;
        
        int ret = (worldState.GetTValue<int>("MaxSeeds") - worldState.GetTValue<int>("Seeds"));
        if (!worldState.GetTValue<bool>("hasAxe"))
            ret += 5;
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
     
        Debug.Log("Goal RecolectFruit Complete");

    }
}