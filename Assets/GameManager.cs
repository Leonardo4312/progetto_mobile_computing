using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement; 
using TMPro; 

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    // --- VARIABILI STATICHE (Rimangono in memoria tra i caricamenti) ---
    public static bool startFromGameplay = false; 
    public static int highScore = 0; 

    [Header("Gameplay Settings")]
    public int lives = 3;                  
    public int currentLevel = 1; 
    private Vector3 originalSpawnPosition; 
    private int score = 0; 
    private int menuPhase = 0; // -1: Login, 0: Start, 1: Story, 2: Gameplay

    [Header("Login UI (PlayFab)")]
    public GameObject loginPanel;        // Trascina qui il pannello di Login

    [Header("Main Menu UI")]
    public GameObject startMenuPanel;    
    public TMP_Text pressStartText;     
    public TMP_Text mainMenuHighScoreText; 

    [Header("Story Cutscene UI")]
    public GameObject storyPanel;        

    [Header("Gameplay UI")]
    public TMP_Text scoreText; 
    public TMP_Text stageText; 
    public GameObject[] heartImages;             

    [Header("Game Over UI")]
    public GameObject gameOverPanel; 
    public RectTransform vortexTransform; 

    private GameObject playerInstance;

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        // Trova la navicella e salva la posizione di spawn
        playerInstance = GameObject.FindGameObjectWithTag("Player");
        if (playerInstance != null)
        {
            originalSpawnPosition = playerInstance.transform.position;
        }

        ResetHeartsUI(); 
        UpdateScoreUI(); 
        UpdateMainMenuHighScoreUI(); 
        
        // --- LOGICA DI AVVIO ---
        if (startFromGameplay)
        {
            // Se arriviamo da un "Retry", saltiamo tutto e giochiamo
            menuPhase = 2;
            Time.timeScale = 1f; 
            
            if (loginPanel != null) loginPanel.SetActive(false);
            if (startMenuPanel != null) startMenuPanel.SetActive(false);
            if (storyPanel != null) storyPanel.SetActive(false);
            if (gameOverPanel != null) gameOverPanel.SetActive(false);

            startFromGameplay = false; 
            StartCoroutine(MostraAnnuncioLivello());
        }
        else
        {
            // AVVIO NORMALE: Fermo sulla schermata LOGIN (Fase -1)
            Time.timeScale = 0f; 
            menuPhase = -1; 

            if (loginPanel != null) loginPanel.SetActive(true);
            if (startMenuPanel != null) startMenuPanel.SetActive(false);
            if (storyPanel != null) storyPanel.SetActive(false);
            if (gameOverPanel != null) gameOverPanel.SetActive(false);
            if (stageText != null) stageText.gameObject.SetActive(false); 
        }
    }

    void Update()
    {
        // Gestione pressione Spazio per avanzare nei menu
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (menuPhase == 0) // Dal Titolo alla Storia
            {
                MoveToStoryPhase();
            }
            else if (menuPhase == 1) // Dalla Storia al Gioco
            {
                StartGameplay();
            }
        }
    }

    // --- FUNZIONE CHIAMATA DA PLAYFAB DOPO IL LOGIN ---
    public void OnLoginVerified()
    {
        menuPhase = 0; 
        if (loginPanel != null) loginPanel.SetActive(false);
        if (startMenuPanel != null) startMenuPanel.SetActive(true);
        
        StartCoroutine(FlashPressStartText()); // Fa apparire "Press Space"
    }

    void MoveToStoryPhase()
    {
        menuPhase = 1;
        if (startMenuPanel != null) startMenuPanel.SetActive(false); 
        if (storyPanel != null) storyPanel.SetActive(true);         
    }

    void StartGameplay()
    {
        menuPhase = 2;
        if (storyPanel != null) storyPanel.SetActive(false); 
        Time.timeScale = 1f;                                 

        StartCoroutine(MostraAnnuncioLivello());
    }

    IEnumerator FlashPressStartText()
    {
        if (pressStartText == null) yield break;
        while (menuPhase == 0)
        {
            pressStartText.enabled = !pressStartText.enabled;
            yield return new WaitForSecondsRealtime(0.4f); 
        }
    }

    // --- GESTIONE PUNTEGGIO ---
    public void AddScore(int points)
    {
        score += points;
        UpdateScoreUI();

        // Se superiamo il record attuale, aggiorniamo l'High Score
        if (score > highScore)
        {
            highScore = score;
        }
    }

    void UpdateScoreUI()
    {
        if (scoreText != null) scoreText.text = "SCORE\n" + score.ToString("D6"); 
    }

    void UpdateMainMenuHighScoreUI()
    {
        if (mainMenuHighScoreText != null)
        {
            mainMenuHighScoreText.text = "HI-SCORE  " + highScore.ToString("D6");
        }
    }

    // --- GESTIONE VITE E HIT ---
    public void PlayerHit()
    {
        lives--;

        if (heartImages != null && lives >= 0 && lives < heartImages.Length)
        {
            StartCoroutine(FlashAndDisableHeart(heartImages[lives]));
        }

        if (lives <= 0)
        {
            if (playerInstance != null) Destroy(playerInstance);
            TriggerGameOver();
        }
        else
        {
            StartCoroutine(RespawnPlayer());
        }
    }

    IEnumerator FlashAndDisableHeart(GameObject heart)
    {
        if (heart == null) yield break;
        for (int i = 0; i < 3; i++)
        {
            heart.SetActive(false);
            yield return new WaitForSecondsRealtime(0.12f); 
            heart.SetActive(true);
            yield return new WaitForSecondsRealtime(0.12f);
        }
        heart.SetActive(false);
    }

    IEnumerator RespawnPlayer()
    {
        if (playerInstance != null)
        {
            playerInstance.SetActive(false); 
            playerInstance.transform.position = originalSpawnPosition; 
            Rigidbody rb = playerInstance.GetComponent<Rigidbody>();
            if (rb != null) { rb.linearVelocity = Vector3.zero; rb.angularVelocity = Vector3.zero; }
        }
        yield return new WaitForSeconds(1.5f); 
        if (playerInstance != null && lives > 0) playerInstance.SetActive(true); 
    }

    // --- GESTIONE LIVELLI ---
    public void AdvanceLevel()
    {
        currentLevel++;
        StartCoroutine(MostraAnnuncioLivello());
    }

    IEnumerator MostraAnnuncioLivello()
    {
        if (stageText == null) yield break;
        stageText.text = "LEVEL " + currentLevel.ToString("D2");
        stageText.gameObject.SetActive(true);
        for (int i = 0; i < 4; i++) { stageText.enabled = !stageText.enabled; yield return new WaitForSeconds(0.2f); }
        stageText.enabled = true;
        yield return new WaitForSeconds(1.0f);
        stageText.gameObject.SetActive(false);
    }

    // --- GAME OVER E BOTTONI ---
    public void TriggerGameOver()
    {
        if (gameOverPanel != null) gameOverPanel.SetActive(true); 
        Time.timeScale = 0f; 

        if (vortexTransform != null)
        {
            vortexTransform.gameObject.SetActive(true);
            StartCoroutine(AnimaVorticeNero());
        }
    }

    public void RetryGame()
    {
        startFromGameplay = true; 
        Time.timeScale = 1f; 
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void GoToMainMenu()
    {
        startFromGameplay = false;
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    IEnumerator AnimaVorticeNero()
    {
        vortexTransform.localScale = Vector3.zero;
        float tempo = 0f;
        while (tempo < 1.5f)
        {
            tempo += Time.unscaledDeltaTime; 
            float p = tempo / 1.5f;
            vortexTransform.localScale = Vector3.Lerp(Vector3.zero, new Vector3(50f, 50f, 50f), p);
            vortexTransform.Rotate(0f, 0f, 600f * Time.unscaledDeltaTime);
            yield return null; 
        }
    }

    void ResetHeartsUI()
    {
        if (heartImages == null) return;
        foreach (GameObject h in heartImages) { if (h != null) h.SetActive(true); }
    }
}