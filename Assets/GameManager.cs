using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [Header("UI Elements")]
    public GameObject gameOverText; 
    public RectTransform vortexTransform; // Trascineremo qui il Vortice nero

    void Awake()
    {
        instance = this;
    }

    public void TriggerGameOver()
    {
        // 1. Accendiamo la scritta GAME OVER gigante
        if (gameOverText != null)
        {
            gameOverText.SetActive(true); 
        }
        
        // 2. CONGELIAMO IL GIOCO! Blocca fisica, movimenti e timer dei nemici
        Time.timeScale = 0f; 

        // 3. Facciamo partire l'effetto Vortice Nero
        if (vortexTransform != null)
        {
            vortexTransform.gameObject.SetActive(true);
            StartCoroutine(AnimaVorticeNero());
        }
    }

    // Coroutine speciale che se ne frega del tempo congelato del gioco
    IEnumerator AnimaVorticeNero()
    {
        vortexTransform.localScale = Vector3.zero;
        float durataAnimazione = 1.5f; // Il vortice ci mette 1.5 secondi a chiudersi
        float tempoPassato = 0f;

        while (tempoPassato < durataAnimazione)
        {
            // Usiamo unscaledDeltaTime perché Time.deltaTime ora è pari a 0!
            tempoPassato += Time.unscaledDeltaTime; 
            float percentuale = tempoPassato / durataAnimazione;

            // Ingrandisce il quadrato nero fino a coprire tutto lo schermo (scala 4)
            vortexTransform.localScale = Vector3.Lerp(Vector3.zero, new Vector3(4f, 4f, 4f), percentuale);

            // Effetto rotazione vortice: gira velocissimo sull'asse Z
            vortexTransform.Rotate(0f, 0f, 600f * Time.unscaledDeltaTime);

            yield return null; // Aspetta il frame successivo
        }

        // Blocco totale finale a schermo nero
        vortexTransform.localScale = new Vector3(4f, 4f, 4f);
    }
}