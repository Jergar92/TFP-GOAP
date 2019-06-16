using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GOAP.Framework;

[CategoryAttribute("AlphaTest")]
public class ChopTree : ActionBase<Transform>
{
    public float time = 5.0f;
    public WSParameter<Transform> item;
    Item instance;
    private float currentTime = 0.0f;
    /*Function: OnActivate()
      * 1 We get the Tree object and we assing our character to the Tree so no other agent can use the Tree
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
        instance.Use(agent.gameObject);

        currentTime = 0;


        OnUpdate();
    }
    /*Function: OnUpdate()
      * 1 We update till the timer pass the flag
      * 2 We Add wood to the inventory
      * 3 We end the action on true
    */
    protected override void OnUpdate()
    {
        if (time < Timer())
        {
            ownerSystem.agent.GetComponent<Inventory>().AddOneWood();

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
      * 1 Stop using the tree item, now other agent can use this tree
    */
    protected override void OnDeactivate()
    {
        instance.Detach();
    }
}
