using InControl;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wolf : MonoBehaviour
{
    public float Speed;
    public float SpeedMultiplier = 1.5f;
    public Transform Mouth;
    private Rigidbody2D rb;
    private Vector3 movement;

    public Player Player;

    private bool hasBody = true;
    private bool isRunning = false;
    private float fleeingTime = 2f;

    private SpriteRenderer sprite;
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
        float x = 0;
        float y = 0;
        if (Player != null)
        {
            x = Player.Device.LeftStickX;
            y = Player.Device.LeftStickY;
        }

        movement = new Vector3(x, y, 0f);

        ChangeFacingDirection(x);

        transform.position = transform.position + movement * Time.deltaTime * Speed;

        if (Player.Device.GetControl(InputControlType.Action1).WasPressed)
        {
            Kill();
        }
        if (Player.Device.GetControl(InputControlType.Action4).WasPressed)
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
        if (Player.Device.GetControl(InputControlType.Action3).WasPressed)
        {
            AskAhou();
        }
        ChangeLayer();
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

    private void Kill()
    {
        if (isRunning || !hasBody) return;
        bool hasKilled = GameController.Instance.KillSheepFromWolf(Mouth.transform.position);
        if (hasKilled)
        {
            //sprite.sprite = WolfSprite;
            if(hasBody) bloodSpatterManager.Splatter(1);
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
        if(hasBody) sprite.sprite = SheepSprite;
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
}
