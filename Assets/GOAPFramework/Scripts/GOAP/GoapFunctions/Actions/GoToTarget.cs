using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GOAP.Framework;

[CategoryAttribute("AlphaTest")]

public class GoToTarget : ActionBase<UnityEngine.AI.NavMeshAgent>
{

    public WSParameter<GameObject> target;
    public WSParameter<float> speed = 5;
    public float minDistance = 0.1f;

    private Vector3 lastPos = Vector3.zero;

    /*Function: OnActivate()
      * 1 We reset the lastPosition
      * 2 We set the speed
      * 3 if the agent is already close enough we exit with EndAction(true)
    */
    protected override void OnActivate()
    {
        lastPos = Vector3.zero;
        agent.speed = speed.value;
        if ((agent.transform.position - target.value.transform.position).magnitude < agent.stoppingDistance + minDistance)
        {
            EndAction(true);
            return;
        }
        MoveTo();
    }
    /*Function: OnUpdate()
        * 1 We move to the target
    */
    protected override void OnUpdate()
    {
        MoveTo();
    }
    /*Function: MoveTo()
     * 1 We get the target position
     * 2 If the target position changes from the last position we set the new destination if the agent can't reach it, we end with EndAction(false)
     * 3 If we are close enough we end with EndAction(true)
    */
    void MoveTo()
    {
        Vector3 position = target.value.transform.position;

        if (lastPos != position)
        {
            if (!agent.SetDestination(position))
            {
                EndAction(false);
                return;
            }
        }
        lastPos = position;
        if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance + minDistance)
        {
            EndAction(true);
        }
    }
}
