﻿using InControl;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Player;

public abstract class PlayerController : MonoBehaviour
{
    //sprite gameobject
    public GameObject Blood;
    public GameObject Shadow;

    public Player Player;

    public bool dead;

    protected SpriteRenderer sprite;

    public enum FacingDirection
    {
        Left,
        Right
    }

    public FacingDirection Direction = FacingDirection.Left;

    public virtual void Die()
    {
        transform.localScale = new Vector3(transform.localScale.x, -transform.localScale.y, transform.localScale.z);
        Destroy(GetComponent<Rigidbody2D>());
        GetComponent<CapsuleCollider2D>().isTrigger = true;
        Shadow.SetActive(false);
        dead = true;

        Blood.SetActive(true);
    }

    protected void ChangeFacingDirection(float x)
    {
        if (x == 0) return;
        if (x > 0)
        {
            transform.localScale = new Vector3(-0.09f, 0.09f, 0.09f);
            Direction = FacingDirection.Right;
        }
        else
        {
            transform.localScale = new Vector3(0.09f, 0.09f, 0.09f);
            Direction = FacingDirection.Left;
        }
    }

    protected void ChangeLayer()
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

    protected bool KillWasPressed()
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

    protected bool SpecialWasPressed()
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

    protected void Bleed()
    {
        Blood.transform.localScale = Vector3.Lerp(Blood.transform.localScale, new Vector3(3f, 3f, 3f), Time.deltaTime);
    }

    public abstract void Kill();
}