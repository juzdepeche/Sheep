using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnvironmentController : MonoBehaviour
{
    public int GrassQuantity = 40;
    public GameObject ButterflyPrefab;

    public int ButterflyQuantity = 6;
    public GameObject[] GrassPrefabs;

    public int FlowerQuantity = 5;
    public GameObject FlowerPrefab;

    void Start()
    {
        for(int i = 0; i < GrassQuantity; i++)
        {
            var grass = Instantiate(GrassPrefabs[Random.Range(0, GrassPrefabs.Length - 1)], GetRandomPosition(), Quaternion.identity);
            grass.transform.SetParent(gameObject.transform);
            float x = Random.Range(-1, 1);
            if(x == 0) grass.transform.localScale = new Vector3(-grass.transform.localScale.x, grass.transform.localScale.y, grass.transform.localScale.z);
        }
        for(int i = 0; i < ButterflyQuantity; i++)
        {
            var butterfly = Instantiate(ButterflyPrefab, GetRandomPosition(), Quaternion.identity);
            butterfly.transform.SetParent(gameObject.transform);
        }
        for (int i = 0; i < FlowerQuantity; i++)
        {
            var flower = Instantiate(FlowerPrefab, GetRandomPosition(), Quaternion.identity);
            flower.transform.SetParent(gameObject.transform);
        }
    }

    private Vector2 GetRandomPosition()
    {
        float x = Random.Range(-8f, 8f);
        float y = Random.Range(-4.5f, 4.5f);

        return new Vector2(x, y);
    }
}
