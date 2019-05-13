using InControl;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Player;

public class Dog : PlayerController
{
    public float Speed;
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
        if (SpecialWasPressed())
        {
            AskWoofWoof();
        }

        ChangeLayer();
    }

    public override void Kill()
    {
        GameController.Instance.KillSheepFromDog(Mouth.transform.position);
    }

    private void AskWoofWoof()
    {
        bool hasWoofed = GameController.Instance.AskWoofWoof(Mouth.transform.position);
    }
}
