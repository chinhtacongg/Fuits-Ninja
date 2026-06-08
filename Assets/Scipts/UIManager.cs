using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [Header("Score")]
    [SerializeField] private TextMeshProUGUI scoreText;      
    [SerializeField] private TextMeshProUGUI levelText;      

    [Header("Lives")]
    [SerializeField] private GameObject[] lifeIcons;         

    [Header("Panels")]
    public GameObject activePanel;       
    public GameObject gameOverPanel;     

    [Header("Game Over")]
    [SerializeField] private TextMeshProUGUI finalScoreText;

    [Header("Level Up")]
    [SerializeField] private GameObject levelUpPanel;        // Panel hiện khi lên level
    [SerializeField] private TextMeshProUGUI levelUpText;
    [SerializeField] private SoundManger soundManager;

    [Header("Volume")]
    [SerializeField] private Slider volumeSlider;            
    [SerializeField] private Button muteButton;
    [SerializeField] private Sprite muteSprite;
    [SerializeField] private Sprite unmuteSprite;

    public static UIManager Instance;

    private int _score;
    private int _targetScore;
    private bool _isMuted = false;
    private float _lastVolume = 1f;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        // Volume slider setup
        if (volumeSlider != null)
        {
            volumeSlider.minValue = 0f;
            volumeSlider.maxValue = 1f;
            volumeSlider.value = AudioListener.volume;
            volumeSlider.onValueChanged.AddListener(OnVolumeChanged);
        }

        if (muteButton != null)
            muteButton.onClick.AddListener(ToggleMute);
    }

    // ===== SCORE =====
    public void SetScore(int score)
    {
        _score = score;
        RefreshScoreText();
        StartCoroutine(ScorePop());
    }

    public void UpdateLevelDisplay(int level, int targetScore)
    {
        _targetScore = targetScore;
        RefreshScoreText();

        if (levelText != null)
            levelText.text = "Level " + level;
    }

    private void RefreshScoreText()
    {
        if (scoreText != null)
            scoreText.text = _score + " / " + _targetScore;
    }

    IEnumerator ScorePop()
    {
        if (scoreText == null) yield break;
        scoreText.transform.localScale = Vector3.one * 1.3f;
        yield return new WaitForSeconds(0.1f);
        scoreText.transform.localScale = Vector3.one;
    }

    // ===== LIVES =====
    public void UpdateLives(int lives)
    {
        if (lifeIcons == null) return;
        for (int i = 0; i < lifeIcons.Length; i++)
        {
            lifeIcons[i].SetActive(i < lives);
        }
    }

    // ===== LEVEL UP =====
    public void ShowLevelUpMessage(int newLevel)
    {
        if (levelUpPanel == null) return;
        if (levelUpText != null)
            levelUpText.text = "Level " + newLevel + "!";

        levelUpPanel.SetActive(true);
        StartCoroutine(HideLevelUpPanel());

        if (soundManager != null)
            soundManager.PlayLevelUpClip();

        StartCoroutine(HideLevelUpPanel());
    }

    IEnumerator HideLevelUpPanel()
    {
        yield return new WaitForSecondsRealtime(2f);
        if (levelUpPanel != null)
            levelUpPanel.SetActive(false);
    }

    // ===== GAME OVER =====
    public void ShowGameOver(int finalScore)
    {
        if (finalScoreText != null)
            finalScoreText.text = "Score: " + finalScore;

        if (gameOverPanel != null)
            gameOverPanel.SetActive(true);
    }

    // ===== VOLUME =====
    private void OnVolumeChanged(float value)
    {
        AudioListener.volume = value;
        _lastVolume = value;
        _isMuted = (value <= 0f);
        UpdateMuteIcon();
    }

    public void ToggleMute()
    {
        _isMuted = !_isMuted;
        if (_isMuted)
        {
            _lastVolume = AudioListener.volume > 0 ? AudioListener.volume : 1f;
            AudioListener.volume = 0f;
            if (volumeSlider != null) volumeSlider.value = 0f;
        }
        else
        {
            AudioListener.volume = _lastVolume;
            if (volumeSlider != null) volumeSlider.value = _lastVolume;
        }
        UpdateMuteIcon();
    }

    private void UpdateMuteIcon()
    {
        if (muteButton == null) return;
        Image btnImg = muteButton.GetComponent<Image>();
        if (btnImg == null) return;

        if (_isMuted && muteSprite != null)
            btnImg.sprite = muteSprite;
        else if (!_isMuted && unmuteSprite != null)
            btnImg.sprite = unmuteSprite;
    }
}
