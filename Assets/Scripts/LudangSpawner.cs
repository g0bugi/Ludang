using UnityEngine;
using UnityEngine.InputSystem;

public class LudangSpawner : MonoBehaviour
{

    [Header("References")]
    public Collider2D leftWallCollider;
    public Collider2D rightWallCollider;

    public SpriteRenderer ghostRenderer;

    [Header("Spawn")]
    public float spawnY = 2.5f;

    public float wallPadding = 0.05f;

    [Header("Drop Randomness")]
    [Range(0f, 2f)]
    public float dropXRandomRange = 0.35f;

    [Header("Input")]
    public float dropCooldown = 0.5f;

    private float cooldownTimer = 0f;

    private float minX = -2.6f;
    private float maxX = 2.6f;



    private void Awake()
    {

        RecalculateClamp();

        if (ghostRenderer != null)
        {

            ghostRenderer.enabled = true;

        }

    }

    private void Update()
    {

        cooldownTimer -= Time.deltaTime;

        float x = GetMouseWorldX();
        x = Mathf.Clamp(x, minX, maxX);

        transform.position = new Vector3(x, spawnY, 0f);

        UpdateGhostVisual();

        if (WasLeftClickThisFrame())
        {

            TryDrop();

        }

    }

    public void RecalculateClamp()
    {

        if (leftWallCollider != null)
        {

            minX = leftWallCollider.bounds.max.x + wallPadding;

        }

        if (rightWallCollider != null)
        {

            maxX = rightWallCollider.bounds.min.x - wallPadding;

        }

        if (minX > maxX)
        {

            float mid = (minX + maxX) * 0.5f;
            minX = mid - 0.1f;
            maxX = mid + 0.1f;

        }

    }

    private void UpdateGhostVisual()
    {

        if (ghostRenderer == null)
        {

            Debug.Log("ghostRenderer is null");
            return;

        }

        if (GameManager.Instance == null)
        {

            Debug.Log("GameManager.Instance is null");
            return;

        }

        int level = GameManager.Instance.CurrentDropLevel;
        var data = GameManager.Instance.GetLevelData(level);

        if (data == null)
        {

            Debug.Log("data is null for level: " + level);
            return;

        }

        // sprite (없으면 현재 ghost sprite 유지)
        if (data.sprite != null)
        {

            ghostRenderer.sprite = data.sprite;

        }

        // size
        ghostRenderer.transform.parent.localScale = Vector3.one * data.size;

        // 고스트 위치는 스포너와 동일
        ghostRenderer.transform.parent.position = transform.position;

    }

    private void TryDrop()
    {

        if (cooldownTimer > 0f)
        {

            return;

        }

        cooldownTimer = dropCooldown;

        Vector3 dropPos = GetRandomizedDropPosition(transform.position);

        GameManager.Instance.DropCurrentAt(dropPos);

    }

    private Vector3 GetRandomizedDropPosition(Vector3 basePos)
    {

        float x = basePos.x;

        if (dropXRandomRange > 0f)
        {

            x += Random.Range(-dropXRandomRange, dropXRandomRange);

        }

        x = Mathf.Clamp(x, minX, maxX);

        return new Vector3(x, spawnY, 0f);

    }

    private bool WasLeftClickThisFrame()
    {

        if (Mouse.current == null)
        {

            return false;

        }

        return Mouse.current.leftButton.wasPressedThisFrame;

    }

    private float GetMouseWorldX()
    {

        if (Mouse.current == null)
        {

            return transform.position.x;

        }

        Vector2 screen = Mouse.current.position.ReadValue();
        Vector3 world = Camera.main.ScreenToWorldPoint(new Vector3(screen.x, screen.y, 0f));

        return world.x;

    }

#if UNITY_EDITOR
    private void OnValidate()
    {

        RecalculateClamp();

    }
#endif

}
