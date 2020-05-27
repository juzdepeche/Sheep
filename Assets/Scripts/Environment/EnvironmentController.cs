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

    private void Start()
    {
        CreateWorld();
    }

    public void CreateWorld()
    {
        for (int i = 0; i < GrassQuantity; i++)
        {
            var grass = Instantiate(GrassPrefabs[Random.Range(0, GrassPrefabs.Length - 1)], GetRandomPosition(), Quaternion.identity);
            grass.transform.SetParent(gameObject.transform);

            var spriteRenderer = grass.GetComponent<SpriteRenderer>();
            LayerManager.Instance.SetLayer(new SpriteRenderer[] { spriteRenderer }, grass.transform.position.y);
            
            SetRandomDirection(grass);
        }
        for (int i = 0; i < ButterflyQuantity; i++)
        {
            var butterfly = Instantiate(ButterflyPrefab, GetRandomPosition(), Quaternion.identity);
            butterfly.transform.SetParent(gameObject.transform);
            SetRandomDirection(butterfly);
        }
        for (int i = 0; i < FlowerQuantity; i++)
        {
            var flower = Instantiate(FlowerPrefab, GetRandomPosition(), Quaternion.identity);
            flower.transform.SetParent(gameObject.transform);

            var spriteRenderer = flower.GetComponent<SpriteRenderer>();
            LayerManager.Instance.SetLayer(new SpriteRenderer[] { spriteRenderer }, flower.transform.position.y);

            SetRandomDirection(flower);
        }
    }

    private Vector2 GetRandomPosition()
    {
        float x = Random.Range(-8f, 8f);
        float y = Random.Range(-4.5f, 4.5f);

        return new Vector2(x, y);
    }

    private void SetRandomDirection(GameObject go)
    {
        float x = Random.Range(-1, 1);
        if (x == 0) go.transform.localScale = new Vector3(-go.transform.localScale.x, go.transform.localScale.y, go.transform.localScale.z);
    }
}
