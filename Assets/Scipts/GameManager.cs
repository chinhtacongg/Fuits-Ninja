using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [Header("Fruit Prefabs")]
    [SerializeField] private GameObject[] fruits;
    [SerializeField] private GameObject[] bombs; // Bomb prefabs

    public static GameManager instance;

    public bool isGameActive;

    private float xRange = 4f;
    private float yRange = 2.5f;
    private float spawnRate = 1f;
    private int life = 3;

    // ===== LEVEL SYSTEM =====
    private int currentLevel = 1;
    private int score = 0;

    // Điểm mục tiêu cho từng level (index = level-1)
    private int[] levelTargets = { 50, 100, 200, 350, 500 };

    // Spawn rate multiplier theo level
    private float[] levelSpeedMultiplier = { 1f, 1.3f, 1.7f, 2.2f, 2.8f };

    // Số lượng bom tối đa spawn theo level
    private int[] maxBombsPerLevel = { 1, 2, 3, 4, 5 };

    private int activeBombs = 0;
    private Coroutine spawnCoroutine;

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
    }

    void Start()
    {
        isGameActive = false;
    }

    // ===== LIFE / BOMB HIT =====
    /// <summary>
    /// Gọi khi chém trúng bom: mất 1 mạng + trừ điểm
    /// </summary>
    public void HitBomb()
    {
        activeBombs = Mathf.Max(0, activeBombs - 1);
        life--;
        score = Mathf.Max(0, score - 20); // trừ 20 điểm
        UIManager.Instance.UpdateLives(life);
        UIManager.Instance.SetScore(score);

        if (life <= 0)
        {
            GameOver();
        }
    }

    /// <summary>
    /// Gọi khi quả bom rơi xuống mà không bị chém → không phạt
    /// </summary>
    public void BombMissed()
    {
        activeBombs = Mathf.Max(0, activeBombs - 1);
        // Không bị phạt
    }

    /// <summary>
    /// Gọi khi trái cây rơi ra ngoài mà không bị chém → mất mạng
    /// </summary>
    public void FruitMissed()
    {
        life--;
        UIManager.Instance.UpdateLives(life);
        if (life <= 0)
        {
            GameOver();
        }
    }

    // ===== SCORE & LEVEL =====
    public void AddScore(int value)
    {
        score += value;
        UIManager.Instance.SetScore(score);
        CheckLevelUp();
    }

    private void CheckLevelUp()
    {
        if (currentLevel > levelTargets.Length) return;

        int targetScore = levelTargets[currentLevel - 1];
        if (score >= targetScore)
        {
            currentLevel++;
            OnLevelUp();
        }
    }

    private void OnLevelUp()
    {
        // Dừng coroutine cũ
        if (spawnCoroutine != null)
            StopCoroutine(spawnCoroutine);

        // Tính spawn rate mới theo level
        float baseRate = spawnRate;
        int lvlIndex = Mathf.Clamp(currentLevel - 1, 0, levelSpeedMultiplier.Length - 1);
        float newRate = baseRate / levelSpeedMultiplier[lvlIndex];

        UIManager.Instance.ShowLevelUpMessage(currentLevel);
        UIManager.Instance.UpdateLevelDisplay(currentLevel, GetCurrentLevelTarget());

        // Restart spawn với tốc độ mới
        spawnCoroutine = StartCoroutine(TimeToSpawn(newRate));
    }

    public int GetCurrentLevelTarget()
    {
        if (currentLevel - 1 < levelTargets.Length)
            return levelTargets[currentLevel - 1];
        return levelTargets[levelTargets.Length - 1];
    }

    // ===== SPAWN =====
    IEnumerator TimeToSpawn(float rate)
    {
        while (true)
        {
            SpawnObject();
            yield return new WaitForSeconds(rate);
        }
    }

    private void SpawnObject()
    {
        // Quyết định spawn bom hay trái cây
        int maxBombs = maxBombsPerLevel[Mathf.Clamp(currentLevel - 1, 0, maxBombsPerLevel.Length - 1)];
        bool spawnBomb = bombs != null && bombs.Length > 0
                         && activeBombs < maxBombs
                         && Random.value < GetBombChance();

        if (spawnBomb)
        {
            SpawnFromArray(bombs);
            activeBombs++;
        }
        else
        {
            SpawnFromArray(fruits);
        }
    }

    private float GetBombChance()
    {
        // Level 1: 10%, Level 2: 18%, Level 3: 26%...
        return Mathf.Clamp(0.08f + (currentLevel - 1) * 0.08f, 0.08f, 0.35f);
    }

    private void SpawnFromArray(GameObject[] pool)
    {
        if (pool == null || pool.Length == 0) return;

        int index = Random.Range(0, pool.Length);
        GameObject obj = pool[index];

        if (!obj.activeInHierarchy)
        {
            obj.transform.position = new Vector3(
                Random.Range(-xRange, xRange),
                yRange,
                obj.transform.position.z
            );
            obj.SetActive(true);
        }
    }

    // ===== GAME FLOW =====
    public void StartGame(float difficulty)
    {
        isGameActive = true;
        score = 0;
        life = 3;
        currentLevel = 1;
        activeBombs = 0;

        float baseRate = spawnRate / difficulty;
        UIManager.Instance.activePanel.SetActive(false);
        UIManager.Instance.UpdateLives(life);
        UIManager.Instance.SetScore(0);
        UIManager.Instance.UpdateLevelDisplay(currentLevel, GetCurrentLevelTarget());

        spawnCoroutine = StartCoroutine(TimeToSpawn(baseRate));
    }

    private void GameOver()
    {
        isGameActive = false;
        if (spawnCoroutine != null)
            StopCoroutine(spawnCoroutine);

        Time.timeScale = 0;
        UIManager.Instance.ShowGameOver(score);
    }

    public void ReStart()
    {
        isGameActive = false;
        Time.timeScale = 1;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
