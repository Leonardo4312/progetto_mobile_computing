using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement; 
using TMPro; 

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    // --- VARIABILI STATICHE (Persistenza dei dati) ---
    public static bool startFromGameplay = false; 
    public static int highScore = 0; 

    [Header("Gameplay Settings")]
    public int lives = 3;                  
    public int currentLevel = 1; 
    private Vector3 originalSpawnPosition; 
    private int score = 0; 

    [Header("Menu Navigation Panels")]
    public GameObject mainMenuPanel;      // Pannello 1: PLAY, STORY, OPTIONS
    public GameObject optionsMenuPanel;   // Pannello 2: MUSIC VOLUME, LANGUAGE, EXIT
    public GameObject loginPanel;         // Pannello 3: MODULO VIOLA PLAYFAB

    [Header("Dynamic Sub-Menu Elements")]
    public GameObject volumeSlider;       
    public GameObject languageFlags;      

    [Header("Story Cutscene UI")]
    public GameObject storyPanel;        

    [Header("Gameplay UI")]
    public TMP_Text scoreText; 
    public TMP_Text stageText; 
    public TMP_Text comboText;            // Oggetto di testo per la combo
    
    [Tooltip("Espandi a 4 elementi nell'Inspector per includere Cuore_4 dell'Easter Egg!")]
    public GameObject[] heartImages;             

    [Header("Combo System Settings (ELITE HARDCORE)")]
    public float comboDuration = 1.5f;     // Tempo massimo (in secondi) per concatenare le uccisioni
    private int currentMultiplier = 1;     // Il moltiplicatore attuale (1x, 2x, 3x, 5x)
    private int comboCount = 0;            // Contatore degli alieni uccisi di fila
    private Coroutine comboTimerCoroutine;   // Tiene traccia del tempo che scade

    [Header("Game Over UI")]
    public GameObject gameOverPanel; 
    public RectTransform vortexTransform; 
    public TMP_Text mainMenuHighScoreText; 

    [Header("Localization Texts")]
    public TMP_Text playBtnText;       
    public TMP_Text storyBtnText;      
    public TMP_Text optionsBtnText;    
    public TMP_Text volumeBtnText;     
    public TMP_Text languageBtnText;   
    public TMP_Text exitBtnText;       

    private bool isItalian = false;    // Di default il gioco parte in Inglese

    private GameObject playerInstance;

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        playerInstance = GameObject.FindGameObjectWithTag("Player");
        if (playerInstance != null)
        {
            originalSpawnPosition = playerInstance.transform.position;
        }

        ResetHeartsUI(); 
        UpdateScoreUI(); 
        UpdateMainMenuHighScoreUI(); 
        
        // Nascondi il testo delle combo all'avvio
        if (comboText != null) comboText.gameObject.SetActive(false);

        // --- LOGICA DI GESTIONE DEI PANNELLI ALL'AVVIO ---
        if (startFromGameplay)
        {
            Time.timeScale = 1f; 
            SetAllPanelsInactive();
            StartCoroutine(MostraAnnuncioLivello());
        }
        else
        {
            Time.timeScale = 0f; 
            SetAllPanelsInactive();
            if (mainMenuPanel != null) mainMenuPanel.SetActive(true);
        }
    }

    void SetAllPanelsInactive()
    {
        if (mainMenuPanel != null) mainMenuPanel.SetActive(false);
        if (optionsMenuPanel != null) optionsMenuPanel.SetActive(false);
        if (loginPanel != null) loginPanel.SetActive(false);
        if (storyPanel != null) storyPanel.SetActive(false);
        if (gameOverPanel != null) gameOverPanel.SetActive(false);
        if (stageText != null) stageText.gameObject.SetActive(false);
        
        if (volumeSlider != null) volumeSlider.SetActive(false);
        if (languageFlags != null) languageFlags.SetActive(false);
    }

    // --- 🌍 SISTEMA DI TRADUZIONE DINAMICA ---

    public void SetLanguageToItalian()
    {
        isItalian = true;

        if (playBtnText != null) playBtnText.text = "GIOCA";
        if (storyBtnText != null) storyBtnText.text = "STORIA";
        if (optionsBtnText != null) optionsBtnText.text = "OPZIONI";
        if (volumeBtnText != null) volumeBtnText.text = "VOL. MUSICA";
        if (languageBtnText != null) languageBtnText.text = "LINGUA";
        if (exitBtnText != null) exitBtnText.text = "ESCI";

        UpdateScoreUI();
        UpdateMainMenuHighScoreUI();
        Debug.Log("Lingua impostata: ITALIANO");
    }

    public void SetLanguageToEnglish()
    {
        isItalian = false;

        if (playBtnText != null) playBtnText.text = "PLAY";
        if (storyBtnText != null) storyBtnText.text = "STORY";
        if (optionsBtnText != null) optionsBtnText.text = "OPTIONS";
        if (volumeBtnText != null) volumeBtnText.text = "MUSIC VOLUME";
        if (languageBtnText != null) languageBtnText.text = "LANGUAGE";
        if (exitBtnText != null) exitBtnText.text = "EXIT";

        UpdateScoreUI();
        UpdateMainMenuHighScoreUI();
        Debug.Log("Language set to: ENGLISH");
    }

    // --- 🕹️ FLUSSO DI NAVIGAZIONE ---

    public void ClickPlayInMainMenu()
    {
        if (mainMenuPanel != null) mainMenuPanel.SetActive(false);
        if (loginPanel != null) loginPanel.SetActive(true);
    }

    public void BackToMainMenuFromLogin()
    {
        if (loginPanel != null) loginPanel.SetActive(false);
        if (mainMenuPanel != null) mainMenuPanel.SetActive(true);
    }

    public void OnLoginVerified()
    {
        if (loginPanel != null) loginPanel.SetActive(false);
        
        Time.timeScale = 1f; 
        StartCoroutine(MostraAnnuncioLivello()); 
    }

    public void ClickOptionsInMainMenu()
    {
        if (mainMenuPanel != null) mainMenuPanel.SetActive(false);
        if (optionsMenuPanel != null) optionsMenuPanel.SetActive(true);
        
        if (volumeSlider != null) volumeSlider.SetActive(false);
        if (languageFlags != null) languageFlags.SetActive(false);
    }

    public void BackToMainMenuFromOptions()
    {
        if (optionsMenuPanel != null) optionsMenuPanel.SetActive(false);
        if (mainMenuPanel != null) mainMenuPanel.SetActive(true);
    }

    public void ToggleVolume()
    {
        if (volumeSlider != null) volumeSlider.SetActive(!volumeSlider.activeSelf); 
    }

    public void ToggleLanguage()
    {
        if (languageFlags != null) languageFlags.SetActive(!languageFlags.activeSelf);
    }

    public void ClickStoryInMainMenu()
    {
        if (mainMenuPanel != null) mainMenuPanel.SetActive(false);
        if (storyPanel != null) storyPanel.SetActive(true);
    }

    public void BackToMainMenuFromStory()
    {
        if (storyPanel != null) storyPanel.SetActive(false);
        if (mainMenuPanel != null) mainMenuPanel.SetActive(true);
    }

    // --- 🔊 CONTROLLO AUDIO SLIDER ---
    public void SetMasterVolume(float value)
    {
        AudioListener.volume = value;
        Debug.Log("Volume Generale impostato a: " + value);
    }

    // --- 💰 SISTEMA DI PUNTEGGIO CORRENTE ---
    public void AddScore(int points)
    {
        score += points;
        UpdateScoreUI();
        if (score > highScore) { highScore = score; UpdateMainMenuHighScoreUI(); }
    }

    void UpdateScoreUI()
    {
        if (scoreText != null) 
        {
            string label = isItalian ? "PUNTI" : "SCORE";
            // 🟢 AGGIORNATO: Ora mostra 9 zeri sul tabellone!
            scoreText.text = label + "\n" + score.ToString("D9"); 
        }
    }

    void UpdateMainMenuHighScoreUI()
    {
        if (mainMenuHighScoreText != null) 
        {
            string label = isItalian ? "RECORD  " : "HIGH-SCORE  ";
            // 🟢 AGGIORNATO: Ora mostra 9 zeri sul Record del menu principale!
            mainMenuHighScoreText.text = label + highScore.ToString("D9");
        }
    }

    // --- 💥 SISTEMA DI COMBO & MOLTIPLICATORE (VERSIONE ELITE HARDCORE) ---
    public void RegisterEnemyKill(int basePoints)
    {
        comboCount++; 

        // Soglie di difficoltà: X2 a 10 uccisioni, X3 a 20, X5 a 30!
        if (comboCount >= 30) currentMultiplier = 5;       
        else if (comboCount >= 20) currentMultiplier = 3;  
        else if (comboCount >= 10) currentMultiplier = 2;  
        else currentMultiplier = 1;                       

        // Aggiornamento della UI della combo
        if (comboText != null)
        {
            if (currentMultiplier > 1)
            {
                comboText.gameObject.SetActive(true);
                comboText.text = "X" + currentMultiplier + " COMBO!";
                StartCoroutine(FlashComboTextUI());
            }
            else
            {
                if (comboCount >= 3)
                {
                    comboText.gameObject.SetActive(true);
                    comboText.text = "STREAK x" + comboCount; 
                }
                else
                {
                    comboText.gameObject.SetActive(false); 
                }
            }
        }

        // Assegna i punti modificati dal moltiplicatore attivo
        AddScore(basePoints * currentMultiplier);

        // Gestione del Timer: se abbatti un altro alieno il tempo si resetta
        if (comboTimerCoroutine != null) StopCoroutine(comboTimerCoroutine);
        comboTimerCoroutine = StartCoroutine(ComboTimeoutRoutine());
    }

    IEnumerator FlashComboTextUI()
    {
        if (comboText == null) yield break;
        comboText.color = Color.white; 
        yield return new WaitForSeconds(0.08f);
        comboText.color = new Color(0f, 1f, 1f); 
    }

    IEnumerator ComboTimeoutRoutine()
    {
        yield return new WaitForSeconds(comboDuration);

        // Reset totale per tempo scaduto
        comboCount = 0;
        currentMultiplier = 1;
        
        if (comboText != null)
        {
            comboText.text = "";
            comboText.gameObject.SetActive(false);
        }
    }

    // --- 🟢 FUNZIONE EASTER EGG: ACCREDITO VITA EXTRA ---
    public void AddLife()
    {
        if (lives < 4) 
        {
            lives++;
            
            int indiceCuore = lives - 1;
            if (heartImages != null && indiceCuore < heartImages.Length && heartImages[indiceCuore] != null)
            {
                heartImages[indiceCuore].SetActive(true);
            }
            Debug.Log($"[EASTER EGG] Vita aggiunta! Vite totali: {lives}");
        }
    }

    // --- 🚀 GESTIONE DANNI E RESPAWN ---
    public void PlayerHit()
    {
        lives--;
        if (heartImages != null && lives >= 0 && lives < heartImages.Length) StartCoroutine(FlashAndDisableHeart(heartImages[lives]));
        if (lives <= 0) { if (playerInstance != null) Destroy(playerInstance); TriggerGameOver(); }
        else StartCoroutine(RespawnPlayer());
    }

    IEnumerator FlashAndDisableHeart(GameObject heart)
    {
        if (heart == null) yield break;
        for (int i = 0; i < 3; i++) { heart.SetActive(false); yield return new WaitForSecondsRealtime(0.12f); heart.SetActive(true); yield return new WaitForSecondsRealtime(0.12f); }
        heart.SetActive(false);
    }

    IEnumerator RespawnPlayer()
    {
        if (playerInstance != null) { playerInstance.SetActive(false); playerInstance.transform.position = originalSpawnPosition; Rigidbody rb = playerInstance.GetComponent<Rigidbody>(); if (rb != null) { rb.linearVelocity = Vector3.zero; rb.angularVelocity = Vector3.zero; } }
        yield return new WaitForSeconds(1.5f); 
        if (playerInstance != null && lives > 0) playerInstance.SetActive(true); 
    }

    // --- 🌌 PROGRESSIONE LIVELLI & EFFETTI ---
    public void AdvanceLevel() { currentLevel++; StartCoroutine(MostraAnnuncioLivello()); }

    IEnumerator MostraAnnuncioLivello()
    {
        if (stageText == null) yield break;
        stageText.gameObject.SetActive(true);
        
        string prefix = isItalian ? "LIVELLO " : "LEVEL ";
        stageText.text = prefix + currentLevel.ToString("D2");
        
        for (int i = 0; i < 4; i++) { stageText.enabled = !stageText.enabled; yield return new WaitForSeconds(0.2f); }
        stageText.enabled = true; yield return new WaitForSeconds(1.0f); stageText.gameObject.SetActive(false);
    }

    public void TriggerGameOver()
    {
        if (gameOverPanel != null) gameOverPanel.SetActive(true); Time.timeScale = 0f; 
        if (vortexTransform != null) { vortexTransform.gameObject.SetActive(true); StartCoroutine(AnimaVorticeNero()); }
    }

    public void RetryGame() { startFromGameplay = true; Time.timeScale = 1f; SceneManager.LoadScene(SceneManager.GetActiveScene().name); }
    public void GoToMainMenu() { startFromGameplay = false; Time.timeScale = 1f; SceneManager.LoadScene(SceneManager.GetActiveScene().name); }

    IEnumerator AnimaVorticeNero()
    {
        vortexTransform.localScale = Vector3.zero; float tempo = 0f;
        while (tempo < 1.5f) { tempo += Time.unscaledDeltaTime; float p = tempo / 1.5f; vortexTransform.localScale = Vector3.Lerp(Vector3.zero, new Vector3(50f, 50f, 50f), p); vortexTransform.Rotate(0f, 0f, 600f * Time.unscaledDeltaTime); yield return null; }
    }

    void ResetHeartsUI() 
    { 
        if (heartImages == null) return; 
        for (int i = 0; i < heartImages.Length; i++)
        {
            if (heartImages[i] != null)
            {
                heartImages[i].SetActive(i < 3);
            }
        }
    }
}