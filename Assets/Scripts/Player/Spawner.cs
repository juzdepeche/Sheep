using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public GameObject Sheep;
    public GameObject Wolf;
    public GameObject Dog;
    public GameObject[] Boules;

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
                newSheep.transform.localScale = new Vector3(-newSheep.transform.localScale.x, newSheep.transform.localScale.y, newSheep.transform.localScale.z);
            }

            sheeps[i] = newSheep;
            Flock.Instance.AddAgent(newSheep.GetComponent<FlockAgent>());
        }

        return sheeps;
    }

    public GameObject SpawnWolf(Player player)
    {
        float x = UnityEngine.Random.Range(-8f, 8f);
        float y = UnityEngine.Random.Range(-4.5f, 4.5f);

        Vector2 target = new Vector2(x, y);

        GameObject wolf = Instantiate(Wolf, target, Quaternion.identity);
        wolf.GetComponent<Wolf>().Player = player;
        return wolf;
    }
    public GameObject SpawnDog(Player player)
    {
        Vector2 target = new Vector2(0, 0);

        GameObject dog = Instantiate(Dog, target, Quaternion.identity);
        dog.GetComponent<Dog>().Player = player;
        return dog;
    }

    public void SpawnDeadSheep(Vector2 wolfPosition, bool[] blood)
    {
        GameObject newDeadSheep = Instantiate(Sheep, wolfPosition, Quaternion.identity);
        newDeadSheep.GetComponent<Sheep>().Kill(blood);
        newDeadSheep.GetComponent<BloodSpatterManager>().AddSplatterToBody(blood);
    }

    public void SpawnBoule(Vector2 position)
    {
        var boule = Instantiate(Boules[UnityEngine.Random.Range(0, Boules.Length - 1)], position, Quaternion.identity);
    }
}
