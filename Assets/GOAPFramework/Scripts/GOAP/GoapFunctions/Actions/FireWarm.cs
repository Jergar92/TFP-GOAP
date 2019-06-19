using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GOAP.Framework;

[CategoryAttribute("AlphaTest")]
public class FireWarm : ActionBase<Transform>
{
    public int coldDecrease = 2;

    public WSParameter<Transform> item;

    Chimney instance;

    private float currentTime = 0.0f;
    public float dropWoodTime = 1.0f;
    /*Function: OnActivate()
        * 1 We get the Chimney object and we assing our character to the Chimney so no other agent can use the Chimney
        * 2 We reset the time
    */
    protected override void OnActivate()
    {
        if (instance == null)
        {
            instance = item.value.GetComponent<Chimney>();
            if (instance == null)
                EndAction(false);
        }
        instance.Use(agent.gameObject);
        currentTime = 0.0f;
    }
    /*Function: OnUpdate()
    * 1 We update till the timer pass the flag
    * 2 We decrease the cold from the player
    * 3 We end the action on true
    */
    protected override void OnUpdate()
    {
        if (dropWoodTime < Timer())
        {
            ownerSystem.agent.GetComponent<Health>().DecreaseCold(coldDecrease);

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
        * 1 Stop using the chimney item, now other agent can use this chimney
    */
    protected override void OnDeactivate()
    {
        instance.Detach();
    }
}
