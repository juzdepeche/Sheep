using InControl;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static PlayerInput;

public class Dog : PlayerController
{
    public float Speed;

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!dead)
        {
            var axis = GetAxis();
            float x = axis[0];
            float y = axis[1];

            Vector3 movement = new Vector3(x, y, 0f);

            ChangeFacingDirection(x);

            if (Input.GetMouseButtonDown(0))
            {
                GameData.Instance.SetValue(GameData.ON_ACTION_1, this);
            }

            var position = transform.position + movement * Time.deltaTime * Speed;
            rb.MovePosition(position);
        }
        else {
            Bleed();
        }
        ChangeLayer();
    }

    private void Update()
    {
        if (Action1WasPressed())
        {
            GameData.Instance.SetValue(GameData.ON_ACTION_1, this);
        }
        if (Action3WasPressed())
        {
            GameData.Instance.SetValue(GameData.ON_ACTION_3, this);
        }
    }
}