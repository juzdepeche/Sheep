using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Sheep : MonoBehaviour
{
    public float timeForNewPath = 1f;
    public float speed;
    public float speedMultiplier = 1.5f;
    public float SpeedBlood = 1f;
    private bool isIdle = false;
    public bool dead = false;

    public bool isFleeing = false;

    private Vector2 currentTarget;

    private float cameraWidth;
    private float cameraHeight;

    public GameObject Shadow;
    public GameObject Blood;

    public SpriteRenderer sprite;

    private float currentTime;
    public float ChangeLayerDelay = 1f;

    public Sheep Instance;
    public BloodSpatterManager bloodSplatterManager;
    private void Awake()
    {
        sprite = GetComponent<SpriteRenderer>();
        sprite.sortingOrder = 100;
    }

    // Start is called before the first frame update
    void Start()
    {
        Instance = this;
        bloodSplatterManager = GetComponent<BloodSpatterManager>();
        currentTarget = GetNewRandomPosition();

        cameraHeight = 2f * Camera.main.orthographicSize;
        cameraWidth = cameraHeight * Camera.main.aspect;

        gameObject.layer = 10;
    }

    // Update is called once per frame
    void Update()
    {
        if (!dead)
        {
            if (!isIdle)
            {
                if (isFleeing)
                {
                    GoToPosition();
                }
                else if (Vector2.Distance(currentTarget, transform.position) > 0)
                {
                    GoToPosition();
                }
                else
                {
                    StartCoroutine(Idle());
                }
            }
            if (currentTime >= ChangeLayerDelay)
            {
                ChangeLayer();
                currentTime = 0;
            }

            currentTime += Time.deltaTime;
        }
        else
        {
            Blood.transform.localScale = Vector3.Lerp(Blood.transform.localScale, new Vector3(3f, 3f, 3f), Time.deltaTime);
        }
    }

    private void ChangeLayer()
    {
        for (int i = 0; i < GameController.Instance.LayersLevels.Length; i++)
        {
            //derniere couche
            if (i == GameController.Instance.LayersLevels.Length - 1)
            {
                sprite.sortingOrder = i + GameController.Instance.MinLayer;
                bloodSplatterManager.SetLayer(i + GameController.Instance.MinLayer);
            }
            //entre 2 couches
            else if (GameController.Instance.LayersLevels[i] >= transform.position.y && GameController.Instance.LayersLevels[i + 1] < transform.position.y)
            {
                sprite.sortingOrder = i + GameController.Instance.MinLayer;
                bloodSplatterManager.SetLayer(i + GameController.Instance.MinLayer);
                break;
            }
        }
    }

    Vector2 GetNewRandomPosition()
    {
        Vector2 target;
        Vector2 futurPosition;

        float x = UnityEngine.Random.Range(-8f, 8f);
        float y = UnityEngine.Random.Range(-4.5f, 4.5f);

        target = new Vector2(x, y);

        futurPosition = target + new Vector2(transform.position.x, transform.position.y);

        if (x > 0)
        {
            transform.localScale = new Vector3(-0.09f, 0.09f, 0.09f);
        }
        else
        {
            transform.localScale = new Vector3(0.09f, 0.09f, 0.09f);
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

    public void FleeFrom(Vector2 murderPosition, float fleeingTime, bool bleed)
    {
        if (bloodSplatterManager != null && bleed) bloodSplatterManager.Splatter(1);
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
        transform.position = Vector2.MoveTowards(transform.position, currentTarget, speed * Time.deltaTime);
    }

    public void Kill()
    {
        transform.localScale = new Vector3(transform.localScale.x, -transform.localScale.y, transform.localScale.z);
        Destroy(GetComponent<Rigidbody2D>());
        //Destroy(GetComponent<CapsuleCollider2D>());
        GetComponent<CapsuleCollider2D>().isTrigger = true;
        //GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Static;
        Shadow.SetActive(false);
        dead = true;

        Blood.SetActive(true);
    }

    public void HitFence()
    {
        currentTarget = GetNewRandomPosition();
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
