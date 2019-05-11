using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dog : MonoBehaviour
{
    public float Speed;
    private SpriteRenderer sprite;
    public GameObject Mouth;
    // Start is called before the first frame update
    void Start()
    {
        sprite = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        float x = Input.GetAxis("Horizontal2");
        float y = Input.GetAxis("Vertical2");

        Vector3 movement = new Vector3(x, y, 0f);

        ChangeFacingDirection(x);

        transform.position = transform.position + movement * Time.deltaTime * Speed;

        if (Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            Kill();
        }
        if (Input.GetKeyDown(KeyCode.KeypadPlus))
        {
            AskWoofWoof();
        }

        ChangeLayer();
    }

    private void Kill()
    {
        GameController.Instance.KillSheepFromDog(Mouth.transform.position);
    }

    private void ChangeFacingDirection(float x)
    {
        if (x == 0) return;
        if (x > 0)
        {
            transform.localScale = new Vector3(-0.09f, 0.09f, 0.09f);
        }
        else
        {
            transform.localScale = new Vector3(0.09f, 0.09f, 0.09f);
        }
    }

    private void AskWoofWoof()
    {
        bool hasWoofed = GameController.Instance.AskWoofWoof(Mouth.transform.position);
    }

    private void ChangeLayer()
    {
        for (int i = 0; i < GameController.Instance.LayersLevels.Length; i++)
        {
            if (i == GameController.Instance.LayersLevels.Length - 1)
            {
                sprite.sortingOrder = i + GameController.Instance.MinLayer;
            }
            else if (GameController.Instance.LayersLevels[i] >= transform.position.y && GameController.Instance.LayersLevels[i + 1] < transform.position.y)
            {
                sprite.sortingOrder = i + GameController.Instance.MinLayer;
                break;
            }
        }
    }
}
