using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Butterfly : MonoBehaviour
{
    private Vector2 currentTarget;
    public float speed = 3;
    // Start is called before the first frame update
    void Start()
    {
        currentTarget = GetNewRandomPosition();
    }

    // Update is called once per frame
    void Update()
    {
        if (Vector2.Distance(currentTarget, transform.position) > 0)
        {
            GoToPosition();
        }
        else
        {
            currentTarget = GetNewRandomPosition();
        }
    }

    void GoToPosition()
    {
        transform.position = Vector2.MoveTowards(transform.position, currentTarget, speed * Time.deltaTime);
    }

    private Vector2 GetNewRandomPosition()
    {
        float x = UnityEngine.Random.Range(-8f, 8f);
        float y = UnityEngine.Random.Range(-4.5f, 4.5f);

        return new Vector2(x, y);
    }
}
