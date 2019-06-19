using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GOAP.Framework;

[CategoryAttribute("AlphaTest")]

public class GrabFruit : ActionBase<Transform>
{
    public float fruitSpeed = 1.0f;
    public WSParameter<Transform> item;
    Item instance;

    public int numFruits = 1;
    private float currentTime = 0.0f;

    /*Function: OnActivate()
      * 1 We get the Seed object and we assing our character to the Seed so no other agent can use the Seed
      * 2 We reset the time
    */
    protected override void OnActivate()
    {
        if (instance == null)
        {
            instance = item.value.GetComponent<Item>();
            if (instance == null)
                EndAction(false);
        }
        instance.Use(agent.gameObject); currentTime = 0.0f;
    }
    /*Function: OnUpdate()
    * 1 We update till the timer pass the flag
    * 2 We Add seeds to the inventory
    * 4 We end the action on true
    */
    protected override void OnUpdate()
    {
        if (fruitSpeed < Timer())
        {
            ownerSystem.agent.GetComponent<Inventory>().AddOneSeed();

            EndAction(true);
        }
    }
    /*Function: Timer()
     * 1 We add time to currentTime
    */
    float Timer()
    {
        return currentTime += Time.deltaTime;
    }
    /*Function: OnDeactivate()
        * 1 Stop using the seed item, now other agent can use this seed
    */
    protected override void OnDeactivate()
    {
        instance.Detach();
    }
}

