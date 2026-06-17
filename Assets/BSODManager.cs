using System.Collections;
using UnityEngine;

public class BSODManager : MonoBehaviour
{
    [Header("UI Element")]
    public GameObject bsodPanel; // Il pannello blu del crash

    [Header("UI Vite Extra")]
    public GameObject cuoreQuattro; // Trascina qui il Cuore_4 che abbiamo creato!

    [Header("Configurazione Trappola")]
    [Range(0f, 1f)] 
    public float probabilitaAltriLivelli = 0.4f; 

    private int ultimoLivelloControllato = -1; 

    void Start()
    {
        if (bsodPanel != null) bsodPanel.SetActive(false);
    }

    void Update()
    {
        if (GameManager.instance != null)
        {
            int livelloAttuale = GameManager.instance.currentLevel;

            if (livelloAttuale != ultimoLivelloControllato)
            {
                ultimoLivelloControllato = livelloAttuale;
                GestisciTrappolaPerNuovoLivello(livelloAttuale);
            }
        }
    }

    void GestisciTrappolaPerNuovoLivello(int livello)
    {
        CancelInvoke("AttivaIlCrash");

        if (livello == 2)
        {
            float tempoCasuale = Random.Range(4f, 10f); 
            Invoke("AttivaIlCrash", tempoCasuale);
        }
        else if (livello > 2)
        {
            if (Random.value <= probabilitaAltriLivelli)
            {
                float tempoCasuale = Random.Range(8f, 20f);
                Invoke("AttivaIlCrash", tempoCasuale);
            }
        }
    }

    void AttivaIlCrash()
    {
        StartCoroutine(BSODRoutine());
    }

    IEnumerator BSODRoutine()
    {
        AudioSource musica = FindObjectOfType<AudioSource>();

        // 🟢 REGALO VITA & CUORE EXTRA
        if (GameManager.instance != null)
        {
            GameManager.instance.AddLife(); // Aggiunge la vita nel codice
        }

        if (cuoreQuattro != null)
        {
            cuoreQuattro.SetActive(true); // Accende VISIVAMENTE il quarto cuore sullo schermo!
        }

        if (musica != null) musica.Pause();
        if (bsodPanel != null) bsodPanel.SetActive(true);

        // ⏱️ MODIFICA TEMPO: Ora aspetta 3 secondi (1 in più rispetto a prima)
        yield return new WaitForSeconds(3f);

        if (bsodPanel != null) bsodPanel.SetActive(false);
        if (musica != null) musica.UnPause();
    }
}
