using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class FlockAgent : MonoBehaviour
{
    Flock agentFlock;
    public Flock AgentFlock { get { return agentFlock; } }
    Collider2D agentCollider;
    public Collider2D AgentCollider { get { return agentCollider; } }
    Rigidbody2D rb;
    public Vector2 direction;
    void Start()
    {
        agentCollider = GetComponent<Collider2D>();
        rb = GetComponent<Rigidbody2D>();
        direction = Vector2.zero;
    }

    public void Initialize(Flock flock)
    {
        agentFlock = flock;
    }

    public void Move(Vector2 velocity)
    {
        direction = velocity;
        //transform.up = velocity;
        transform.position += (Vector3)velocity * Time.deltaTime;

        //var position = (transform.position + (Vector3)velocity) * Time.deltaTime;
        //rb.MovePosition(position);
    }
}
