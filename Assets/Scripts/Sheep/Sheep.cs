using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Sheep : MonoBehaviour, IMortal
{
    public bool isFlocking = false;
    public float timeForNewPath = 1f;
    public float speed;
    public float speedMultiplier = 1.5f;
    public float SpeedBlood = 1f;
    private bool isIdle = false;
    public bool dead = false;

    public bool isFleeing = false;

    private Vector2 currentTarget;

    private Rigidbody2D rb;

    private float cameraWidth;
    private float cameraHeight;

    public GameObject Shadow;
    public GameObject Blood;

    public SpriteRenderer[] spritesRenderer;

    public GameObject[] sprites;

    private float currentTimeLayer;
    public float ChangeLayerDelay = 1f;
    private float currentTimeNotMoving;
    private Vector2 lastPositionNotChanged;
    public float NotMovingDelay;
    
    private Vector2 lastFacingPosition;

    private Animator animator;

    public Sheep Instance;
    public BloodSpatterManager bloodSplatterManager;

    private float xBaseTransform;

    private int health = 5; 

    private void Awake()
    {
        spritesRenderer = new SpriteRenderer[sprites.Length];

        for (int i = 0; i < spritesRenderer.Length; i++)
        {
            spritesRenderer[i] = sprites[i].GetComponent<SpriteRenderer>();
            spritesRenderer[i].sortingOrder = 100;
        }
        bloodSplatterManager = GetComponent<BloodSpatterManager>();
    }

    // Start is called before the first frame update
    void Start()
    {
        Instance = this;
        currentTarget = GetNewRandomPosition();

        rb = GetComponent<Rigidbody2D>();
        rb.mass = UnityEngine.Random.Range(0.2f, 0.5f);
        cameraHeight = 2f * Camera.main.orthographicSize;
        cameraWidth = cameraHeight * Camera.main.aspect;

        lastFacingPosition = Vector2.zero;
        xBaseTransform = Math.Abs(transform.localScale.x);

        if (DamageZone.instance != null)
        {
            FunctionPeriodic.Create(() => {
                if (DamageZone.instance.IsOutsideCircle(transform.position)) {
                    Damage();
                }
            }, .5f);
        }

        animator = GetComponent<Animator>();
    }
    
    void FixedUpdate()
    {
        if (!dead)
        {
            if (!isIdle)
            {
                if (!isFlocking)
                {
                    if (isFleeing || Vector2.Distance(currentTarget, transform.position) > 0)
                    {
                        GoToPosition();
                    }
                    else
                    {
                        StartCoroutine(Idle());
                    }
                }
            }
            if (currentTimeLayer >= ChangeLayerDelay)
            {
                ChangeLayer();
                currentTimeLayer = 0;
            }

            currentTimeLayer += Time.deltaTime;
            SetFacingDirection();
        }
        else
        {
            Blood.transform.localScale = Vector3.Lerp(Blood.transform.localScale, new Vector3(3f, 3f, 3f), Time.deltaTime);
        }
    }

    private void SetFacingDirection() {
        //if(transform.position.x < currentTarget.x)
        //{
        //    transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
        //}
        //else if(transform.position.x > currentTarget.x)
        //{
        //    transform.localScale = new Vector3(Math.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        //}
    }

    private void ChangeLayer()
    {
        LayerManager.Instance.SetLayer(spritesRenderer, transform.position.y);
        LayerManager.Instance.SetLayer(bloodSplatterManager.GetBloodSpattersSpriteRenderers(), transform.position.y);
    }

    Vector2 GetNewRandomPosition(Vector2? setPosition = null)
    {
        float x = 0, y = 0;
        Vector2 target;
        if (setPosition == null)
        {

            x = UnityEngine.Random.Range(-8f, 8f);
            y = UnityEngine.Random.Range(-4.5f, 4.5f);

            target = new Vector2(x, y);
        }
        else
        {
            target = (Vector2)setPosition;
        }

        return new Vector2(x, y);

    }

    IEnumerator Idle()
    {
        isIdle = true;
        yield return new WaitForSeconds(UnityEngine.Random.Range(timeForNewPath, timeForNewPath + 1f));
        if (!dead) currentTarget = GetNewRandomPosition();
        isIdle = false;
    }

    public void FleeFrom(Vector2 murderPosition, float fleeingTime)
    {
        isFleeing = true;

        if (isIdle)
        {
            StopCoroutine(Idle());
            isIdle = false;
        }

        speed *= speedMultiplier;

        Vector2 fleeFrom = new Vector2(transform.position.x, transform.position.y) - murderPosition;

        currentTarget = (new Vector2(transform.position.x, transform.position.y) + fleeFrom) * 100f;

        StartCoroutine(StopFleeingFrom(fleeingTime));
    }

    IEnumerator StopFleeingFrom(float fleeingTime)
    {
        yield return new WaitForSeconds(fleeingTime);
        isFleeing = false;
        speed /= speedMultiplier;
        if (!dead) currentTarget = GetNewRandomPosition();
    }

    void GoToPosition()
    {
        currentTimeNotMoving += Time.deltaTime;
        var position = Vector2.MoveTowards(transform.position, currentTarget, speed * Time.deltaTime);
        rb.MovePosition(position);

        if (position.x > transform.position.x)
        {
            transform.localScale = new Vector3(-xBaseTransform, transform.localScale.y, transform.localScale.z);
        }
        else if (position.x < transform.position.x)
        {
            transform.localScale = new Vector3(xBaseTransform, transform.localScale.y, transform.localScale.z);
        }

        lastFacingPosition = position;

        if (currentTimeNotMoving >= NotMovingDelay)
        {
            if (Vector2.Distance(transform.position, lastPositionNotChanged) <= 0.5f)
            {
                currentTarget = GetNewRandomPosition();
            }
            currentTimeNotMoving = 0;
            lastPositionNotChanged = transform.position;
        }
    }

    public void Die()
    {
        if (dead) return;
        transform.localScale = new Vector3(transform.localScale.x, -transform.localScale.y, transform.localScale.z);
        // -0.3f so it is as high as it was before the 180 of localScale.y
        transform.position = new Vector2(transform.position.x, transform.position.y - 0.2f);
        Destroy(GetComponent<Rigidbody2D>());
        GetComponent<CapsuleCollider2D>().isTrigger = true;
        Shadow.SetActive(false);
        dead = true;

        Blood.SetActive(true);
    }

    public void Damage(int amount = 1)
    {
        Bleed(1);
        health--;
        if (health <= 0) Die();
    }

    public void Bleed(int amount = 1)
    {
        bloodSplatterManager.Splatter(amount);
    }

    public void HitFence()
    {
        currentTarget = GetNewRandomPosition();
    }

    public void Hide()
    {
        for (int i = 0; i < spritesRenderer.Length; i++)
        {
            spritesRenderer[i].enabled = false;
        }
    }

    public void Show()
    {
        for (int i = 0; i < spritesRenderer.Length; i++)
        {
            spritesRenderer[i].enabled = true;
        }
    }

    internal void GoTo(Vector2 doggoMouth, float goToTime)
    {
        isFleeing = true;

        if (isIdle)
        {
            StopCoroutine(Idle());
            isIdle = false;
        }

        speed *= speedMultiplier;

        currentTarget = GetNewRandomPosition(doggoMouth);

        StartCoroutine(StopGointTo(goToTime));
    }

    IEnumerator StopGointTo(float time)
    {
        yield return new WaitForSeconds(time);
        isFleeing = false;
        speed /= speedMultiplier;
        if (!dead) currentTarget = GetNewRandomPosition();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.tag == "Fence")
        {
            Idle();
            HitFence();
        }
    }
}
