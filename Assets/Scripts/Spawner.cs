using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public GameObject Sheep;
    public GameObject Wolf;

    public static Spawner Instance;

    private void Awake()
    {
        Instance = this;
    }

    public GameObject[] SpawnSheeps(int sheepNumber)
    {
        float cameraHeight = 2f * Camera.main.orthographicSize;
        float cameraWidth = cameraHeight * Camera.main.aspect;

        GameObject[] sheeps = new GameObject[sheepNumber];

        for (var i = 0; i < sheepNumber; i++)
        {
            float x = UnityEngine.Random.Range(-8f, 8f);
            float y = UnityEngine.Random.Range(-4.5f, 4.5f);

            Vector2 target = new Vector2(x, y);

            GameObject newSheep = Instantiate(Sheep, target, Quaternion.identity);

            int side = UnityEngine.Random.Range(0, 2);

            if (side >= 1)
            {
                newSheep.transform.localScale += new Vector3(0, -1f, 0);
            }

            sheeps[i] = newSheep;
        }

        return sheeps;
    }

    public GameObject SpawnWolf()
    {
        float x = UnityEngine.Random.Range(-8f, 8f);
        float y = UnityEngine.Random.Range(-4.5f, 4.5f);

        Vector2 target = new Vector2(x, y);

        GameObject wolf = Instantiate(Wolf, target, Quaternion.identity);

        return wolf;
    }
}
