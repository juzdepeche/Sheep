using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Flock/Behavior/Stay in Zone")]
public class StayInRadiusBehavioer : FlockBehavior
{
    public Vector2 center;
    public float horizontalRadius = 8f;
    public float verticalRadius = 4.5f;

    public override Vector2 CalculateMove(FlockAgent agent, List<FlockAgent> context, Flock flock)
    {
        Vector2 centerOffset = center - (Vector2)agent.transform.position;
        float horizontal = centerOffset.magnitude / horizontalRadius;
        float vertical = centerOffset.magnitude / verticalRadius;

        if (horizontal < 0.9f && vertical < 0.9f)
        {
            return Vector2.zero;
        }
        //vertical nullators are used in case a sheep overlaps y limits and not x limits (used to not return Vector2.zero)
        int horizontalNullator = horizontal < 0.9f ? 0 : 1;
        int verticalNullator = vertical < 0.9f ? 0 : 1;

        return new Vector2(centerOffset.x * horizontal * horizontal * horizontalNullator, centerOffset.y * vertical * horizontal * verticalNullator);
    }
}

