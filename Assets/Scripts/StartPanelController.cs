
using UnityEngine;

public class StartPanelController : MonoBehaviour
{

    [Header("Panels")]
    public GameObject startPanel;

    [Header("Objects To Enable After Start")]
    public GameObject hud;
    public GameObject spawnerRoot;

    [Header("Optional: Disable On Start (if already active)")]
    public GameObject gameOverPanel;

    private bool hasStarted = false;

    private void Awake()
    {

        if (startPanel != null)
        {

            startPanel.SetActive(true);

        }

        if (hud != null)
        {

            hud.SetActive(false);

        }

        if (spawnerRoot != null)
        {

            spawnerRoot.SetActive(false);

        }

        if (gameOverPanel != null)
        {

            gameOverPanel.SetActive(false);

        }

        hasStarted = false;

    }

    public void OnClickStart()
    {

        if (hasStarted)
        {

            return;

        }

        hasStarted = true;

        if (startPanel != null)
        {

            startPanel.SetActive(false);

        }

        if (hud != null)
        {

            hud.SetActive(true);

        }

        if (spawnerRoot != null)
        {

            spawnerRoot.SetActive(true);

        }

        // GameManager가 이미 Start()에서 UI 초기화는 해두고 있으니
        // 여기서는 굳이 호출 안 해도 되지만,
        // 시작 버튼 눌렀을 때 UI 한 번 더 확실히 갱신하고 싶으면 아래를 켜도 됨.
        //
        // if (GameManager.Instance != null)
        // {
        //
        //     GameManager.Instance.ForceRefreshUI();
        //
        // }

    }

}

