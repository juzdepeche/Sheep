using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Flock/Behavior/Cohesion")]
public class CohesionBehavior : FlockBehavior
{
    Vector2 currentVelocity;
    public float agentSmoothTime = 0.5f;

    public override Vector2 CalculateMove(FlockAgent agent, List<FlockAgent> context, Flock flock)
    {
        // no neighbor
        if (context.Count == 0)
            return Vector2.zero;

        //average point
        Vector2 cohesionMove = Vector2.zero;
        foreach (FlockAgent item in context)
        {
            cohesionMove += (Vector2)item.transform.position;
        }
        cohesionMove /= context.Count;

        //create offset from agent position
        cohesionMove -= (Vector2)agent.transform.position;
        cohesionMove = Vector2.SmoothDamp(agent.direction, cohesionMove, ref currentVelocity, agentSmoothTime);

        return cohesionMove;
    }
}