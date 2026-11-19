using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PatrolEnemy : MonoBehaviour
{
    [SerializeField] private Transform pointA;
    [SerializeField] private Transform pointB;
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private float reachDistance = 0.05f;
    [SerializeField] private float damageInterval = 1f;

    private Rigidbody2D rb;
    private Transform target;
    private float nextDamageTime;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.bodyType = RigidbodyType2D.Kinematic;
        rb.gravityScale = 0f;
        rb.freezeRotation = true;
    }

    private void Start()
    {
        target = pointB != null ? pointB : pointA;
    }

    private void FixedUpdate()
    {
        if (pointA == null || pointB == null || target == null)
            return;

        Vector2 nextPosition = Vector2.MoveTowards(
            rb.position,
            target.position,
            moveSpeed * Time.fixedDeltaTime
        );

        rb.MovePosition(nextPosition);

        if (Vector2.Distance(nextPosition, target.position) <= reachDistance)
            target = target == pointA ? pointB : pointA;
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (!collision.gameObject.CompareTag("Player"))
            return;

        if (GameManager.Instance == null)
            return;

        if (Time.time < nextDamageTime)
            return;

        GameManager.Instance.TakeDamage(1);
        nextDamageTime = Time.time + damageInterval;
    }
}