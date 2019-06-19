using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GOAP.Framework;

[CategoryAttribute("AlphaTest")]
public class GrapAxe : ActionBase<Transform>
{

    public float grabSpeed = 1.0f;
    private float currentTime = 0.0f;

    /*Function: OnActivate()
      * 2 We reset the time
    */
    protected override void OnActivate()
    {
     
        currentTime = 0.0f;
    }
    /*Function: OnUpdate()
    * 1 We update till the timer pass the flag
    * 4 We end the action on true
    */
    protected override void OnUpdate()
    {
        if (grabSpeed < Timer())
        {

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
}
