using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LayerManager : MonoBehaviour
{
    public static LayerManager Instance;
    public float[] LayersLevels;
    
    public int MinLayer = 8;

    private float cameraHeight;

    [SerializeField]
    private int LayersNumber;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    private void Start() {
        cameraHeight = 2f * Camera.main.orthographicSize;
        
        SplitLevelInLayers();
    }

    public void SetLayer(GameObject go)
    {
        var spriteRenderer = go.GetComponent<SpriteRenderer>();
        if (Instance == null || spriteRenderer) return;
        for (int i = 0; i < Instance.LayersLevels.Length; i++)
        {
            //last layer
            if (i == Instance.LayersLevels.Length - 1)
            {
                spriteRenderer.sortingOrder = i + Instance.MinLayer;
            }
            //between 2 layers
            else if (Instance.LayersLevels[i] >= go.transform.position.y && Instance.LayersLevels[i + 1] < go.transform.position.y)
            {
                spriteRenderer.sortingOrder = i + Instance.MinLayer;
                break;
            }
        }
    }

    private void SplitLevelInLayers()
    {
        LayersLevels = new float[LayersNumber];

        float step = (cameraHeight / 2) / (LayersNumber / 2);

        float currentStep = cameraHeight / 2;

        for (var i = 0; i < LayersLevels.Length; i++)
        {
            if (i != 0)
            {
                currentStep -= step;
            }

            LayersLevels[i] = currentStep;
        }
    }

}
