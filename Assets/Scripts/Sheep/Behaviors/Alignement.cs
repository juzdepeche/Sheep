using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Flock/Behavior/Alignement")]
public class AlignementBehavior : FlockBehavior
{
    public override Vector2 CalculateMove(FlockAgent agent, List<FlockAgent> context, Flock flock)
    {
        // no neighbor, maintain current rotaiton
        if (context.Count == 0)
            return agent.direction;

        //average point
        Vector2 aligenementMove = Vector2.zero;
        foreach (FlockAgent item in context)
        {
            aligenementMove += (Vector2)item.direction;
        }
        aligenementMove /= context.Count;

        return aligenementMove;
    }
}

