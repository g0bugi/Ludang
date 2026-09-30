
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameOverWarningUI : MonoBehaviour
{

    [Header("UI")]
    public CanvasGroup canvasGroup;

    public Image fillImage;

    public TextMeshProUGUI warningText;

    private void Awake()
    {

        if (canvasGroup == null)
        {

            canvasGroup = GetComponent<CanvasGroup>();

        }

        SetVisible(false);

    }

    public void SetVisible(bool visible)
    {

        if (canvasGroup == null)
        {

            return;

        }

        canvasGroup.alpha = visible ? 1f : 0f;
        canvasGroup.blocksRaycasts = visible;
        canvasGroup.interactable = visible;

    }

    public void SetDangerRatio(float ratio01)
    {

        ratio01 = Mathf.Clamp01(ratio01);

        if (fillImage != null)
        {

            fillImage.fillAmount = ratio01;

        }

        if (warningText != null)
        {

            if (ratio01 <= 0f)
            {

                warningText.text = "";

            }
            else
            {

                int percent = Mathf.RoundToInt(ratio01 * 100f);
                warningText.text = $"DANGER {percent}%";

            }

        }

    }

}

