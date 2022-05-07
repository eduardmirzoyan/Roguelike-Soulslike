using UnityEngine;
using System.Collections;

public class Camera : MonoBehaviour
{
    public static Camera instance;
    [SerializeField] private Transform target;
    [SerializeField] private float smoothSpeed = 0.125f;
    [SerializeField] private Vector3 normalOffset;
    [SerializeField] private float panDownDelay = 3f;
    [SerializeField] private float panUpDelay = 3f;
    [SerializeField] private Vector3 panDownOffset;
    [SerializeField] private Vector3 pandUpOffset;

    private Vector3 currentOffset;
    private float downTimer = 0f;
    private float upTimer = 0f;

    private void Awake() {
        if(GameManager.instance != null)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
    }

    private void Start() {
        target = GameManager.instance.GetPlayer().transform;
        currentOffset = normalOffset;
        downTimer = panDownDelay;
        upTimer = panUpDelay;
    }

    private void FixedUpdate() {
        Vector3 desiredPosition = target.position + currentOffset;
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
        transform.position = smoothedPosition;
    }

    public void checkPanDown() {
        if (downTimer > 0) {
            downTimer -= Time.deltaTime;
        }
        else {
            currentOffset = panDownOffset;
        }
    }

    public void resetPan() {
        downTimer = panDownDelay;
        upTimer = panUpDelay;
        currentOffset = normalOffset;
    }


}