using InControl;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Player;

public class Dog : MonoBehaviour
{
    public float Speed;
    public Player Player;
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
        float x = 0;
        float y = 0;
        if (Player != null)
        {
            if(Player.Device != null)
            {
                x = Player.Device.LeftStickX;
                y = Player.Device.LeftStickY;
            }
            else
            {
                x = Input.GetAxis(Player.InputAxeX);
                y = Input.GetAxis(Player.InputAxeY);
            }
        }

        Vector3 movement = new Vector3(x, y, 0f);

        ChangeFacingDirection(x);

        transform.position = transform.position + movement * Time.deltaTime * Speed;

        if (KillWasPressed())
        {
            Kill();
        }
        if (AskWoofWoofWasPressed())
        {
            AskWoofWoof();
        }

        ChangeLayer();
    }

    private bool KillWasPressed()
    {
        if (Player.ControllerType == EControllerType.Controller)
        {
            if (Player.Device.GetControl(InputControlType.Action1).WasPressed) return true;
        }
        else
        {
            if (Input.GetKeyDown(Player.Action1)) return true;
        }

        return false;
    }

    private bool AskWoofWoofWasPressed()
    {
        if (Player.ControllerType == EControllerType.Controller)
        {
            if (Player.Device.GetControl(InputControlType.Action3).WasPressed) return true;
        }
        else
        {
            if (Input.GetKeyDown(Player.Action3)) return true;
        }

        return false;
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
