using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnvironmentController : MonoBehaviour
{
    public int GrassQuantity = 40;
    public GameObject[] GrassPrefabs;

    void Start()
    {
        for(int i = 0; i < GrassQuantity; i++)
        {
            var grass = Instantiate(GrassPrefabs[Random.Range(0, GrassPrefabs.Length - 1)], GetRandomPosition(), Quaternion.identity);
        }
    }

    private Vector2 GetRandomPosition()
    {
        float x = Random.Range(-8f, 8f);
        float y = Random.Range(-4.5f, 4.5f);

        /*if (x > 0)
        {
            transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
        }
        else
        {
            transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        }*/

        return new Vector2(x, y);
    }
}
