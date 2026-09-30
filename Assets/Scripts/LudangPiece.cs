
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
public class LudangPiece : MonoBehaviour
{

    [SerializeField]
    private int levelIndex = 0;

    private bool isMerging = false;

    public int LevelIndex => levelIndex;

    public bool IsMerging => isMerging;

    [Header("Render")]
    public SpriteRenderer visualRenderer;

    [Header("Merge")]
    [Tooltip("생성 직후 합체 금지 시간(초). 0이면 즉시 합체 가능.")]
    public float mergeImmunitySeconds = 0.25f;

    private float mergeImmuneUntil = 0f;

    private Rigidbody2D rb;
    private Collider2D col;

    private void Awake()
    {

        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();

        if (visualRenderer == null)
        {

            Transform visual = transform.Find("Visual");
            if (visual != null)
            {

                visualRenderer = visual.GetComponent<SpriteRenderer>();

            }

        }

        if (visualRenderer == null)
        {

            visualRenderer = GetComponent<SpriteRenderer>();

        }

        SetMergeImmunity(mergeImmunitySeconds);

    }

    public void ApplyLevel(int newLevelIndex, LudangLevelTable table)
    {

        levelIndex = newLevelIndex;

        if (table == null)
        {

            return;

        }

        var data = table.Get(levelIndex);
        if (data == null)
        {

            return;

        }

        if (visualRenderer != null && data.sprite != null)
        {

            visualRenderer.sprite = data.sprite;

        }

        transform.localScale = Vector3.one * data.size;

    }

    public void MarkMerging()
    {

        isMerging = true;

    }

    public void SetMergeImmunity(float seconds)
    {

        if (seconds <= 0f)
        {

            mergeImmuneUntil = 0f;
            return;

        }

        mergeImmuneUntil = Time.time + seconds;

    }

    private bool IsMergeImmune()
    {

        return (mergeImmuneUntil > 0f) && (Time.time < mergeImmuneUntil);

    }

    private void OnCollisionEnter2D(Collision2D collision)
    {

        TryMergeFromCollision(collision);

    }

    private void OnCollisionStay2D(Collision2D collision)
    {

        TryMergeFromCollision(collision);

    }

    private void TryMergeFromCollision(Collision2D collision)
    {

        if (isMerging)
        {

            return;

        }

        if (IsMergeImmune())
        {

            return;

        }

        var other = collision.collider.GetComponent<LudangPiece>();
        if (other == null)
        {

            return;

        }

        if (other.isMerging)
        {

            return;

        }

        if (other.IsMergeImmune())
        {

            return;

        }

        if (other.levelIndex != levelIndex)
        {

            return;

        }

        if (GetEntityId() > other.GetEntityId())
        {

            return;

        }

        isMerging = true;
        other.isMerging = true;

        Vector2 mergePos = (transform.position + other.transform.position) * 0.5f;

        GameManager.Instance.RequestMerge(this, other, mergePos);

    }

}

