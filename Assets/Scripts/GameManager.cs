
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{

    public static GameManager Instance { get; private set; }

    [Header("Data")]
    public LudangLevelTable levelTable;

    [Header("Prefabs")]
    public LudangPiece piecePrefab;

    [Header("UI (TMP)")]
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI mergedCountText;
    public Image nextIcon;

    [Header("GameOver UI")]
    public GameObject gameOverPanel;

    [Header("Win UI")]
    public GameObject winPanel;

    [Header("Spawn Rule")]
    [Range(0, 10)]
    public int maxRandomStartLevel = 3;

    [Header("Audio - Merge")]
    public AudioClip mergeSfx;

    [Range(0f, 1f)]
    public float mergeSfxVolume = 1f;

    [Range(0f, 0.5f)]
    public float mergePitchRandom = 0.05f;

    [Header("Audio - Special Level (Max or PreMax)")]
    public AudioClip specialLevelSfx;

    [Range(0f, 1f)]
    public float specialSfxVolume = 1f;

    [Header("Merge Immunity")]
    [Tooltip("새로 생성된 조각이 합체하지 못하는 시간(초).")]
    public float spawnMergeImmunitySeconds = 0.25f;

    private AudioSource audioSource;

    private int score = 0;
    private int mergedLudangCount = 0;

    private int currentDropLevel = 0;
    private int nextDropLevel = 0;

    private bool isGameOver = false;
    private bool isWin = false;

    public int CurrentDropLevel => currentDropLevel;

    public int MergedLudangCount => mergedLudangCount;

    private void Awake()
    {

        if (Instance != null && Instance != this)
        {

            Destroy(gameObject);
            return;

        }

        Instance = this;

        if (gameOverPanel != null)
        {

            gameOverPanel.SetActive(false);

        }

        if (winPanel != null)
        {

            winPanel.SetActive(false);

        }

        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {

            audioSource = gameObject.AddComponent<AudioSource>();

        }

        audioSource.playOnAwake = false;
        audioSource.spatialBlend = 0f;

    }

    private void Start()
    {

        Time.timeScale = 1f;

        PrepareNext();
        AdvanceCurrentFromNext();

        UpdateUI();

    }

    private void PrepareNext()
    {

        if (levelTable == null || levelTable.Count <= 0)
        {

            nextDropLevel = 0;
            return;

        }

        int max = Mathf.Clamp(maxRandomStartLevel, 0, levelTable.Count - 1);
        nextDropLevel = Random.Range(0, max + 1);

    }

    private void AdvanceCurrentFromNext()
    {

        currentDropLevel = nextDropLevel;
        PrepareNext();

    }

    public void DropCurrentAt(Vector3 worldPos)
    {

        if (isGameOver || isWin)
        {

            return;

        }

        if (piecePrefab == null)
        {

            Debug.LogError("[GameManager] piecePrefab is missing.");
            return;

        }

        var piece = Instantiate(piecePrefab, worldPos, Quaternion.identity);
        piece.ApplyLevel(currentDropLevel, levelTable);

        if (spawnMergeImmunitySeconds > 0f)
        {

            piece.SetMergeImmunity(spawnMergeImmunitySeconds);

        }

        AdvanceCurrentFromNext();
        UpdateUI();

    }

    public void RequestMerge(LudangPiece a, LudangPiece b, Vector2 mergePos)
    {

        if (isGameOver || isWin)
        {

            return;

        }

        if (a == null || b == null)
        {

            return;

        }

        int level = a.LevelIndex;
        int nextLevel = level + 1;

        mergedLudangCount += 2;

        AddScoreForLevel(level);

        PlayMergeSfx();
        PlaySpecialLevelSfx(nextLevel);

        Destroy(a.gameObject);
        Destroy(b.gameObject);

        int maxLevelIndex = -1;

        if (levelTable != null)
        {

            maxLevelIndex = levelTable.Count - 1;

        }

        if (levelTable != null && nextLevel < levelTable.Count)
        {

            var spawned = Instantiate(piecePrefab, mergePos, Quaternion.identity);
            spawned.ApplyLevel(nextLevel, levelTable);

            if (spawnMergeImmunitySeconds > 0f)
            {

                spawned.SetMergeImmunity(spawnMergeImmunitySeconds);

            }

            if (nextLevel == maxLevelIndex)
            {

                TriggerWin();

            }

        }
        else
        {

            TriggerWin();

        }

        UpdateUI();

    }

    private void AddScoreForLevel(int mergedLevel)
    {

        if (levelTable == null)
        {

            score += 10;
            return;

        }

        var data = levelTable.Get(mergedLevel);
        if (data == null)
        {

            score += 10;
            return;

        }

        score += data.mergeScore;

    }

    private void PlayMergeSfx()
    {

        if (mergeSfx == null)
        {

            return;

        }

        if (audioSource == null)
        {

            return;

        }

        float originalPitch = audioSource.pitch;

        if (mergePitchRandom > 0f)
        {

            audioSource.pitch = 1f + Random.Range(-mergePitchRandom, mergePitchRandom);

        }
        else
        {

            audioSource.pitch = 1f;

        }

        audioSource.PlayOneShot(mergeSfx, mergeSfxVolume);

        audioSource.pitch = originalPitch;

    }

    private void PlaySpecialLevelSfx(int reachedLevel)
    {

        if (specialLevelSfx == null)
        {

            return;

        }

        if (audioSource == null)
        {

            return;

        }

        if (levelTable == null || levelTable.Count <= 0)
        {

            return;

        }

        int maxLevelIndex = levelTable.Count - 1;
        int preMaxLevelIndex = levelTable.Count - 2;
        int prepreMaxLevelIndex = levelTable.Count - 3;

        bool isMaxOrPreMax = (reachedLevel == maxLevelIndex) || (reachedLevel == preMaxLevelIndex) || (reachedLevel == prepreMaxLevelIndex);

        if (!isMaxOrPreMax)
        {

            return;

        }

        audioSource.PlayOneShot(specialLevelSfx, specialSfxVolume);

    }

    private void UpdateUI()
    {

        if (scoreText != null)
        {

            scoreText.text = $"Score: {score}";

        }

        if (nextIcon != null && levelTable != null)
        {

            var next = levelTable.Get(nextDropLevel);
            if (next != null)
            {

                nextIcon.sprite = next.sprite;
                nextIcon.enabled = (next.sprite != null);

            }

        }

    }

    public void TriggerGameOver()
    {

        if (isGameOver || isWin)
        {

            return;

        }

        isGameOver = true;

        if (gameOverPanel != null)
        {

            gameOverPanel.SetActive(true);

        }

    }

    private void TriggerWin()
    {

        if (isWin || isGameOver)
        {

            return;

        }

        isWin = true;

        Time.timeScale = 0f;

        if (winPanel != null)
        {

            winPanel.SetActive(true);

        }

    }

    public LudangLevelTable.LudangLevel GetLevelData(int index)
    {

        if (levelTable == null)
        {

            return null;

        }

        return levelTable.Get(index);

    }

    public void Restart()
    {

        Time.timeScale = 1f;

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);

    }

}

