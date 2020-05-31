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
        GameObject[] sheeps = new GameObject[sheepNumber];

        for (var i = 0; i < sheepNumber; i++)
        {
            //todo: extract these values into variables
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
            if (Flock.Instance) Flock.Instance.AddAgent(newSheep.GetComponent<FlockAgent>());
        }

        return sheeps;
    }

    public GameObject SpawnWolf(Player player, Nullable<Vector2> position = null)
    {
        //todo: extract these values into variables
        position = position == null ? new Vector2(UnityEngine.Random.Range(-8f, 8f), UnityEngine.Random.Range(-4.5f, 4.5f)) : position;

        GameObject wolf = Instantiate(Wolf, position.GetValueOrDefault(), Quaternion.identity);
        wolf.GetComponent<Wolf>().Player = player;
        return wolf;
    }
    public GameObject SpawnDog(Player player, Nullable<Vector2> position = null)
    {
        //todo: extract these values into variables
        position = position == null ? new Vector2(0, 0) : position;

        GameObject dog = Instantiate(Dog, position.GetValueOrDefault(), Quaternion.identity);
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
