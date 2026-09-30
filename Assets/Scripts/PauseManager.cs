
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseManager : MonoBehaviour
{

    [Header("Pause UI")]
    public GameObject pausePanel;

    private bool isPaused = false;

    private void Start()
    {

        if (pausePanel != null)
        {

            pausePanel.SetActive(false);

        }

    }

    private void Update()
    {

        if (Input.GetKeyDown(KeyCode.Escape))
        {

            TogglePause();

        }

    }

    public void TogglePause()
    {

        if (isPaused)
        {

            Resume();

        }
        else
        {

            Pause();

        }

    }

    private void Pause()
    {

        isPaused = true;

        Time.timeScale = 0f;

        if (pausePanel != null)
        {

            pausePanel.SetActive(true);

        }

    }

    public void Resume()
    {

        isPaused = false;

        Time.timeScale = 1f;

        if (pausePanel != null)
        {

            pausePanel.SetActive(false);

        }

    }

    public void QuitGame()
    {

        Application.Quit();

    }

}

