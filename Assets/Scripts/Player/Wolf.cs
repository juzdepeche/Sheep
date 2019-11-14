﻿using InControl;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Player;

public class Wolf : PlayerController
{
    public float Speed;
    public float SpeedMultiplier = 1.5f;
    public Transform Mouth;
    private Rigidbody2D rb;
    private Vector3 movement;

    private bool hasBody = true;
    private bool isRunning = false;
    private float fleeingTime = 2f;

    public Sprite WolfSprite;
    public Sprite WolfWithoutSheepSprite;
    public Sprite SheepSprite;

    public BloodSpatterManager bloodSpatterManager;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        sprite = GetComponent<SpriteRenderer>();
        bloodSpatterManager = GetComponent<BloodSpatterManager>();
        sprite.sortingOrder = 100;
    }

    // Update is called once per frame
    void Update()
    {
        if (!dead)
        {
            float x = 0;
            float y = 0;

            if (Player != null)
            {
                if (Player.Device != null)
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

            movement = new Vector3(x, y, 0f);

            ChangeFacingDirection(x);

            transform.position = transform.position + movement * Time.deltaTime * Speed;

            if (KillWasPressed())
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
            if (SpecialWasPressed())
            {
                AskAhou();
            }
        }
        else {
            Bleed();
        }
        ChangeLayer();
    }

    private bool BodyWasPressed()
    {
        if (Player.ControllerType == EControllerType.Controller)
        {
            if (Player.Device.GetControl(InputControlType.Action4).WasPressed) return true;
        }
        else
        {
            if (Input.GetKeyDown(Player.Action4)) return true;
        }

        return false;
    }

    public override void Kill()
    {
        if (isRunning || !hasBody) return;
        bool hasKilled = GameController.Instance.KillFromWolf(Mouth.transform.position);
        if (hasKilled)
        {
            //sprite.sprite = WolfSprite;
            if (hasBody) bloodSpatterManager.Splatter(1);
            Speed *= SpeedMultiplier;
            isRunning = true;
            StartCoroutine(StopRunning());
        }
    }

    public void ExitBody()
    {
        if (!hasBody) return;
        hasBody = false;
        sprite.sprite = WolfWithoutSheepSprite;
        Speed *= 2.5f;
        GameController.Instance.WolfGetOutFromBody(transform.position, bloodSpatterManager.BloodSpattersBool);
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

    private void AskAhou()
    {
        bool hasAhou = GameController.Instance.AskAhou();
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
        transform.localScale = new Vector3(transform.localScale.x, -transform.localScale.y, transform.localScale.z);
        Destroy(GetComponent<Rigidbody2D>());
        GetComponent<CapsuleCollider2D>().isTrigger = true;
        Shadow.SetActive(false);
        dead = true;
        ExitBody();
        Blood.SetActive(true);
    }
}