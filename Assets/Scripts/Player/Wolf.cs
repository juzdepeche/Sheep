using InControl;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static PlayerInput;

public class Wolf : PlayerController
{
    public float Speed;
    public float SpeedMultiplier = 1.5f;
    private Vector3 movement;

    private bool hasBody = true;
    private bool isRunning = false;
    private float fleeingTime = 2f;

    public Sprite WolfSprite;
    public Sprite WolfWithoutSheepSprite;
    public Sprite SheepSprite;

    public BloodSpatterManager bloodSpatterManager;
    
    //See PlayerController
    public override void DoStart()
    {
        bloodSpatterManager = GetComponent<BloodSpatterManager>();
    }
    
    void FixedUpdate()
    {
        if (!dead)
        {
            var axis  = GetAxis();
            float x = axis[0];
            float y = axis[1];

            movement = new Vector3(x, y, 0f);

            ChangeFacingDirection(x);

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
            Kill();
        }
        if (BodyWasPressed())
        {
            if (hasBody)
            {
                ExitBody();
            }
            else
            {
                TryGetInNewBody();
            }
        }
        if (Action3WasPressed())
        {
            GameData.Instance.SetValue(GameData.ON_ACTION_3, this);
        }
    }

    private bool BodyWasPressed()
    {
        if (PlayerInput.ControllerType == EControllerType.Controller)
        {
            if (PlayerInput.Device.GetControl(InputControlType.Action4).WasPressed) return true;
        }
        else
        {
            if (Input.GetKeyDown(PlayerInput.Action4)) return true;
        }

        return false;
    }

    public override void Kill()
    {
        //todo: check isRunning usage
        if (isRunning || !hasBody) return;
        GameData.Instance.SetValue(GameData.ON_ACTION_1, this);
    }

    public void OnKillConfirmed()
    {
        if (hasBody) bloodSpatterManager.Splatter(1);
        Speed *= SpeedMultiplier;
        isRunning = true;
        StartCoroutine(StopRunning());
    }

    public void ExitBody()
    {
        if (!hasBody) return;
        hasBody = false;
        sprite.sprite = WolfWithoutSheepSprite;
        Speed *= 2.5f;
        GameData.Instance.SetValue(GameData.ON_WOLF_BODY_EXIT, this);
        AudioManager.Instance.Howl();
        bloodSpatterManager.RemoveAll();
    }

    private void TryGetInNewBody()
    {
        GameController.Instance.WolfGetInNewBody(this, Mouth.transform.position);
    }

    public void GetInNewBody(bool[] bloodSpattersBool)
    {
        bloodSpatterManager.AddSplatterToBody(bloodSpattersBool);
        sprite.sprite = SheepSprite;
        hasBody = true;
        isRunning = false;
        Speed /= 2.5f;
    }

    IEnumerator StopRunning()
    {
        yield return new WaitForSeconds(fleeingTime);
        isRunning = false;
        Speed /= SpeedMultiplier;
        if (hasBody) sprite.sprite = SheepSprite;
    }

    public void Hide()
    {
        sprite.enabled = false;
    }

    public void Show()
    {
        sprite.enabled = true;
    }

    public override void Die()
    {
        base.Die();
        ExitBody();
    }
}
