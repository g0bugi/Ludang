
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class GameOverLine : MonoBehaviour
{

    [Header("Rule")]
    public float holdSecondsToGameOver = 2f;

    [Header("Optional UI")]
    public GameOverWarningUI warningUI;

    private Dictionary<LudangPiece, float> stayTimers = new Dictionary<LudangPiece, float>();

    private void Reset()
    {

        var c = GetComponent<Collider2D>();
        c.isTrigger = true;

    }


    private void Update()
    {

        if (stayTimers.Count <= 0)
        {

            if (warningUI != null)
            {

                warningUI.SetVisible(false);

            }

            return;

        }

        float maxRatio = 0f;

        // Dictionary 직접 foreach 금지: 키 목록을 복사해서 안전하게 순회
        var keys = new List<LudangPiece>(stayTimers.Keys);

        for (int i = 0; i < keys.Count; i++)
        {

            var piece = keys[i];

            if (piece == null)
            {

                stayTimers.Remove(piece);
                continue;

            }

            float t = stayTimers[piece] + Time.deltaTime;
            stayTimers[piece] = t;

            float ratio = holdSecondsToGameOver > 0f ? Mathf.Clamp01(t / holdSecondsToGameOver) : 1f;
            if (ratio > maxRatio)
            {

                maxRatio = ratio;

            }

            if (t >= holdSecondsToGameOver)
            {

                GameManager.Instance.TriggerGameOver();
                return;

            }

        }

        if (warningUI != null)
        {

            warningUI.SetVisible(true);
            warningUI.SetDangerRatio(maxRatio);

        }

    }



    private void OnTriggerEnter2D(Collider2D other)
    {

        var piece = other.GetComponent<LudangPiece>();
        if (piece == null)
        {

            return;

        }

        if (stayTimers.ContainsKey(piece))
        {

            return;

        }

        stayTimers.Add(piece, 0f);

    }

    private void OnTriggerExit2D(Collider2D other)
    {

        var piece = other.GetComponent<LudangPiece>();
        if (piece == null)
        {

            return;

        }

        stayTimers.Remove(piece);

    }

}

