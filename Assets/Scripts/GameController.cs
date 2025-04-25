using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
    // Singleton
    public static GameController Instance { get; private set; }

    [Header("UI Elements (arraste no Inspector)")]
    public Text       coinText;      // Seu Text de moedas
    public Image      healthBar;     // Sua barra de vida
    public GameObject gameOverPanel; // Painel de Game Over

    [Header("Configurações de Jogo")]
    public int lifeCost  = 5;    // moedas para comprar vida
    public int maxHealth = 10;   // vida máxima

    [HideInInspector] public int totalCoins;
    private Player     player;
    private bool       isGameOver = false;

    void Awake()
    {
        // Configura o singleton
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    void OnEnable()
    {
        // Inscreve para recriar referências ao carregar cena
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void Start()
    {
        // Inicia moedas (só na primeira vez)
        totalCoins = 0;

        player = FindObjectOfType<Player>();
        if (gameOverPanel) gameOverPanel.SetActive(false);

        Time.timeScale = 1f;
        isGameOver     = false;

        UpdateCoinUI();
        if (player != null) UpdateHealthUI(player.health);
    }

    // Chamado toda vez que uma cena é (re)carregada
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Reencontra o player
        player = FindObjectOfType<Player>();

        // Reaponta sempre os objetos de UI na cena nova
        if (coinText == null)
            coinText = GameObject.Find("CoinCountText")?.GetComponent<Text>();

        if (healthBar == null)
            healthBar = GameObject.Find("HealthBar")?.GetComponent<Image>();

        if (gameOverPanel == null)
            gameOverPanel = GameObject.Find("GameOverPanel");

        // Reseta flag e tempo
        isGameOver     = false;
        Time.timeScale = 1f;

        if (gameOverPanel)
            gameOverPanel.SetActive(false);

        // Atualiza as UIs com os valores que persistem
        UpdateCoinUI();
        if (player != null) UpdateHealthUI(player.health);
    }

    void Update()
    {
        // Dispara Game Over uma vez quando vida zera
        if (!isGameOver && healthBar != null && healthBar.fillAmount <= 0f)
            ShowGameOver();
    }

    void ShowGameOver()
    {
        isGameOver = true;
        if (gameOverPanel == null)
            gameOverPanel = GameObject.Find("GameOverPanel");

        if (gameOverPanel)
        {
            Time.timeScale = 0f;
            gameOverPanel.SetActive(true);
        }
    }

    // --- Métodos públicos para pontuação e vida ---

    public void AddCoins(int amount = 1)
    {
        totalCoins += amount;
        UpdateCoinUI();
    }

    public void LoseHealth(int newHealth)
    {
        if (player == null) player = FindObjectOfType<Player>();
        player.health = Mathf.Clamp(newHealth, 0, maxHealth);
        UpdateHealthUI(player.health);
    }

    public void AddHealth(int amount)
    {
        if (player == null) player = FindObjectOfType<Player>();
        player.health = Mathf.Clamp(player.health + amount, 0, maxHealth);
        UpdateHealthUI(player.health);
    }

    void UpdateCoinUI()
    {
        if (coinText != null)
            coinText.text = totalCoins.ToString("D3");
    }

    void UpdateHealthUI(int h)
    {
        if (healthBar != null)
            healthBar.fillAmount = (float)h / maxHealth;
    }

    // --- Métodos de botões ---

    public void OnBuyLife()
    {
        if (!isGameOver) return;
        if (totalCoins >= lifeCost)
        {
            totalCoins -= lifeCost;
            UpdateCoinUI();
            AddHealth(1);
            HideGameOverPanel();
        }
        else Debug.Log("Moedas insuficientes.");
    }

    public void OnRestart()
    {
        // Recarrega a cena mantendo totalCoins
        Time.timeScale = 1f;
        isGameOver = false;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void OnGoToShop()
    {
        Time.timeScale = 1f;
        isGameOver = false;
        SceneManager.LoadScene("ShopScene");
    }

    void HideGameOverPanel()
    {
        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);
        Time.timeScale = 1f;
        isGameOver = false;
    }
}
