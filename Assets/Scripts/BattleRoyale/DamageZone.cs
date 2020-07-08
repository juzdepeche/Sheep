using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageZone : MonoBehaviour
{
    public static DamageZone instance;

    [SerializeField] private Transform targetCircleTransform;

    private Transform circleTransform;
    private Transform topTransform;
    private Transform bottomTransform;
    private Transform leftTransform;
    private Transform rightTransform;

    [SerializeField]
    private float circleShrinkSpeed;
    private float shrinkTimer;

    private Vector3 circleSize;
    private Vector3 circlePosition;

    private Vector3 targetCircleSize;
    private Vector3 targetCirclePosition;

    private void Awake() {
        instance = this;

        circleTransform = transform.Find("circle");
        topTransform = transform.Find("top");
        bottomTransform = transform.Find("bottom");
        leftTransform = transform.Find("left");
        rightTransform = transform.Find("right");

        SetCircleSize(new Vector3(0, 0), new Vector3(20, 20));

        SetTargetCircle(new Vector3(0, 0), new Vector3(15, 15), 3f);
    }

    private void Update() {
        shrinkTimer -= Time.deltaTime;

        if (shrinkTimer < 0) {
            Vector3 sizeChangeVector = (targetCircleSize - circleSize).normalized;
            Vector3 newCircleSize = circleSize + sizeChangeVector * Time.deltaTime * circleShrinkSpeed;

            Vector3 circleMoveDir = (targetCirclePosition - circlePosition).normalized;
            Vector3 newCirclePosition = circlePosition + circleMoveDir * Time.deltaTime * circleShrinkSpeed;

            SetCircleSize(newCirclePosition, newCircleSize);

            float distanceTestAmount = .1f;
            if (Vector3.Distance(newCircleSize, targetCircleSize) < distanceTestAmount && Vector3.Distance(newCirclePosition, targetCirclePosition) < distanceTestAmount) {
                GenerateTargetCircle();
            }
        }
    }

    private void GenerateTargetCircle() {
        float shrinkSizeAmount = Random.Range(.8f, .9f);
        Vector3 generatedTargetCircleSize = circleSize * shrinkSizeAmount;
        float diameter = generatedTargetCircleSize.x;

        Vector3 generatedTargetCirclePosition = circlePosition;// + 
            // new Vector3(Random.Range(-diameter, diameter), Random.Range(-diameter, diameter));

        float shrinkTime = Random.Range(1f, 3f);

        SetTargetCircle(generatedTargetCirclePosition, generatedTargetCircleSize, shrinkTime);
    }

    private void SetCircleSize(Vector3 position, Vector3 size) {
        circlePosition = position;
        circleSize = size;

        transform.position = position;

        circleTransform.localScale = size;

        topTransform.localScale = new Vector3(100, 100);
        topTransform.localPosition = new Vector3(0, topTransform.localScale.y * .5f + size.y * .5f);
        
        bottomTransform.localScale = new Vector3(100, 100);
        bottomTransform.localPosition = new Vector3(0, -topTransform.localScale.y * .5f - size.y * .5f);

        leftTransform.localScale = new Vector3(100, size.y);
        leftTransform.localPosition = new Vector3(-leftTransform.localScale.x * .5f - size.x * .5f, 0f);

        rightTransform.localScale = new Vector3(100, size.y);
        rightTransform.localPosition = new Vector3(+leftTransform.localScale.x * .5f + size.x * .5f, 0f);
    }

    private void SetTargetCircle(Vector3 position, Vector3 size, float shrinkTimer) {
        this.shrinkTimer = shrinkTimer;

        targetCircleTransform.position = position;
        targetCircleTransform.localScale = size;
        
        targetCirclePosition = position;
        targetCircleSize = size;
    }

    public bool IsOutsideCircle(Vector3 position) {
        return Vector3.Distance(position, circlePosition) > circleSize.x * .5f;
    }
}
