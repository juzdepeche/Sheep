using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SheepManager : MonoBehaviour
{
    public static SheepManager Instance;
    public GameObject[] Sheeps;
    // Start is called before the first frame update
    void Start()
    {
        Instance = this;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /*public void FleeSheepsFrom(Vector2 murderPosition, float time)
    {
        foreach(GameObject sheep in Sheeps)
        {
            if(murderReactRange < Vector2.Distance(murderPosition, sheep.transform.position))
            {
                sheep.GetComponent<Sheep>().FleeFrom(murderPosition, time);
            }
        }
    }*/
}
