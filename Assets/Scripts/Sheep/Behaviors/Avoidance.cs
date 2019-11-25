using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Flock/Behavior/Avoidance")]
public class AvoidanceBehavior : FlockBehavior
{
    public override Vector2 CalculateMove(FlockAgent agent, List<FlockAgent> context, Flock flock)
    {
        // no neighbor
        if (context.Count == 0)
            return Vector2.zero;

        //average point
        Vector2 avoidanceMove = Vector2.zero;
        int nAvoid = 0;
        foreach (FlockAgent item in context)
        {
            if (Vector2.SqrMagnitude(item.transform.position - agent.transform.position) < flock.SquareAvoidanceRadius + Random.Range(-1f,1f))
            {
                avoidanceMove += (Vector2)(agent.transform.position - item.transform.position);
                nAvoid++;
            }
        }
        if (nAvoid > 0)
            avoidanceMove /= nAvoid;

        return avoidanceMove;
    }
}