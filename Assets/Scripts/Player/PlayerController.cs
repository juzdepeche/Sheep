using InControl;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static PlayerInput;

public abstract class PlayerController : MonoBehaviour
{
    //sprite gameobject
    public GameObject Blood;
    public GameObject Shadow;

    public PlayerInput PlayerInput;

    public bool dead;

    protected SpriteRenderer sprite;
    protected Rigidbody2D rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        sprite = GetComponent<SpriteRenderer>();
        DoStart();
    }

    private void Update() {
        if (PlayerInput.Device.CommandWasPressed || Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            GameData.Instance.SetValue(GameData.ON_START, this);
        }
    }

    //Since Wolf needed Start(), DoStart is an override method to be use in its children to not override Start() 
    public virtual void DoStart()
    {
        //override
    }

    public enum FacingDirection
    {
        Left,
        Right
    }

    public FacingDirection Direction = FacingDirection.Left;

    public virtual void Die()
    {
        transform.localScale = new Vector3(transform.localScale.x, -transform.localScale.y, transform.localScale.z);
        // -0.3f so it is as high as it was before the 180 of localScale.y
        transform.position = new Vector2(transform.position.x, transform.position.y - 0.3f);
        Destroy(GetComponent<Rigidbody2D>());
        GetComponent<CapsuleCollider2D>().isTrigger = true;
        Shadow.SetActive(false);
        dead = true;

        Blood.SetActive(true);
    }

    public float[] GetAxis()
    {
        float[] axis = new float[2];
        axis[0] = 0;
        axis[1] = 0;

        if (PlayerInput.Device != null)
        {
            axis[0] = PlayerInput.Device.LeftStickX;
            axis[1] = PlayerInput.Device.LeftStickY;
        }
        else
        {
            axis[0] = Input.GetAxis(PlayerInput.InputAxeX);
            axis[1] = Input.GetAxis(PlayerInput.InputAxeY);
        }

        return axis;
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
        LayerManager.Instance.SetLayer(new SpriteRenderer[] { sprite }, transform.position.y);
    }

    protected bool Action1WasPressed()
    {
        bool wasPressed = PlayerInput.Action1WasPressed();

        if (wasPressed) GameData.Instance.SetValue(GameData.ON_ACTION_1, this);

        return wasPressed;
    }

    protected bool Action2WasPressed()
    {
		return PlayerInput.Action2WasPressed();
    }

    protected bool Action3WasPressed()
    {
        return  PlayerInput.Action3WasPressed();
    }

    protected void Bleed()
    {
        Blood.transform.localScale = Vector3.Lerp(Blood.transform.localScale, new Vector3(3f, 3f, 3f), Time.deltaTime);
    }

    public abstract void Kill();
}
