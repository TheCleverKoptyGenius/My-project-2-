using UnityEngine;

public class ExitDoor : MonoBehaviour
{
    [Header("Assign two colliders: one solid and one trigger")]
    [SerializeField] private Collider2D closedSolidCollider;
    [SerializeField] private Collider2D openTriggerCollider;

    [Header("Optional door graphics")]
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Sprite closedSprite;
    [SerializeField] private Sprite openSprite;

    private bool lastOpenState;

    private void Start()
    {
        ApplyDoorState();
    }

    private void Update()
    {
        if (GameManager.Instance == null)
            return;

        if (lastOpenState != GameManager.Instance.IsExitOpen)
            ApplyDoorState();
    }

    private void ApplyDoorState()
    {
        lastOpenState = GameManager.Instance != null &&
                        GameManager.Instance.IsExitOpen;

        if (closedSolidCollider != null)
            closedSolidCollider.enabled = !lastOpenState;

        if (openTriggerCollider != null)
            openTriggerCollider.enabled = lastOpenState;

        if (spriteRenderer != null)
        {
            Sprite spriteToUse = lastOpenState ? openSprite : closedSprite;
            if (spriteToUse != null)
                spriteRenderer.sprite = spriteToUse;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player"))
            return;

        if (GameManager.Instance != null && GameManager.Instance.IsExitOpen)
            GameManager.Instance.LeaveLevel();
    }
}
