using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GOAP.Framework;

[CategoryAttribute("AlphaTest")]
public class Sleep : ActionBase<Transform>
{
    public int healthRestored = 5;
    public WSParameter<Transform> item;

    Item instance;
    private float currentTime = 0.0f;
    public float sleepTime = 1.0f;

    /*Function: OnActivate()
      * 1 We get the Bed object and we assing our character to the Bed so no other agent can use the Bed
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
    * 2 We recover health
    * 4 We end the action on true
    */
    protected override void OnUpdate()
    {
        if (sleepTime < Timer())
        {
            ownerSystem.agent.GetComponent<Health>().RestoreHealth(healthRestored);

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
        * 1 Stop using the bed item, now other agent can use this bed
    */
    protected override void OnDeactivate()
    {
        instance.Detach();
    }
}
