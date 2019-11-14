using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BloodSpatterManager : MonoBehaviour
{
    [SerializeField]
    private List<GameObject> BloodSpatters;
    public bool[] BloodSpattersBool;

    // Start is called before the first frame update
    void Start()
    {
        BloodSpattersBool = new bool[BloodSpatters.Count];
        for (var i = 0; i < BloodSpatters.Count; i++)
        {
            BloodSpatters[i].SetActive(false);
            BloodSpattersBool[i] = false;
        }
    }

    public void Splatter(int num)
    {
        bool isFull = true;
        for (int i = 0; i < BloodSpatters.Count; i++)
        {
            if (!BloodSpattersBool[i]) isFull = false;
        }
        if (isFull) return;

        for (int i = 0; i < num; i++)
        {
            int index = 0;
            do
            {
                index = Random.Range(0, BloodSpatters.Count);
            } while (BloodSpattersBool[index]);
            BloodSpatters[index].SetActive(true);
            BloodSpattersBool[index] = true;
        }
    }

    public void RemoveAll()
    {
        for (int i = 0; i < BloodSpatters.Count; i++)
        {
            BloodSpatters[i].SetActive(false);
            BloodSpattersBool[i] = false;
        }
    }

    public void AddSplatterToBody(bool[] splatter)
    {
        BloodSpattersBool = splatter;

        for (int i = 0; i < BloodSpattersBool.Length; i++)
        {
            if (BloodSpattersBool[i])
            {
                BloodSpatters[i].SetActive(true);
            }
        }
    }

    public void HideAll()
    {
        for (int i = 0; i < BloodSpatters.Count; i++)
        {
            BloodSpatters[i].SetActive(false);
        }
    }

    public void ShowAll()
    {
        for (int i = 0; i < BloodSpatters.Count; i++)
        {
            if(BloodSpattersBool[i]) BloodSpatters[i].SetActive(true);
        }
    }

    public void SetLayer(int layer)
    {
        for (int i = 0; i < BloodSpatters.Count; i++)
        {
            BloodSpatters[i].GetComponent<SpriteRenderer>().sortingOrder = layer;
        }
    }
}
