using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GOAP.Framework;
[CategoryAttribute("AlphaTest")]
public class EatFruit : ActionBase<Transform>
{

    public int hungerRestoed = 2;
    public float time = 5.0f;

    private float currentTime = 0.0f;
    /*Function: OnActivate()
    * 1 We Open the inventory (this is equal to activate some sort of animation or state that block the player to do other thing)
    * 2 We reset the time
    */
    protected override void OnActivate()
    {
        ownerSystem.agent.GetComponent<Inventory>().OpenInventory(true);

        currentTime = 0;

        OnUpdate();
    }
    /*Function: OnUpdate()
    * 1 We update till the timer pass the flag
    * 2 We Add remove one seed from the inventory
    * 3 We decrease the hunger of the player
    * 4 We end the action on true
    */
    protected override void OnUpdate()
    {
        if (time < Timer())
        {
            ownerSystem.agent.GetComponent<Inventory>().RemoveOneSeed();

            ownerSystem.agent.GetComponent<Health>().DecreaseHunger(hungerRestoed);

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
        * 1 Close the inventory
    */
    protected override void OnDeactivate()
    {
        ownerSystem.agent.GetComponent<Inventory>().OpenInventory(false);

    }
}
