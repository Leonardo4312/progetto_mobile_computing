using UnityEngine;

public class Enemy3D : MonoBehaviour
{
    private Rigidbody rb;
    
    [Header("Movement Settings")]
    public float speed = 4f;              // Velocità di discesa
    public float zigZagSpeed = 5f;        // Velocità dello zig-zag
    public float zigZagWidth = 2f;        // Larghezza dello zig-zag

    // Variabili segrete per la picchiata
    private bool isDiving = false;
    private Transform originalParent;
    private Vector3 originalLocalPosition;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        // La velocità iniziale del Rigidbody rimane spenta!
    }

    // Questa funzione viene chiamata dal GridManager per far partire l'attacco
    public void StartDive()
    {
        if (isDiving) return; // Se sta già attaccando, ignora

        isDiving = true;
        
        // Memorizziamo chi era il padre e la posizione esatta nella griglia
        originalParent = transform.parent;
        originalLocalPosition = transform.localPosition;

        // Ci stacchiamo dalla griglia per muoverci liberamente nello spazio globale
        transform.parent = null;
    }

    void Update()
    {
        if (isDiving)
        {
            // 1. Calcoliamo lo zig-zag orizzontale usando il數學 Mathf.Sin
            float xMovement = Mathf.Sin(Time.time * zigZagSpeed) * zigZagWidth;

            // 2. Muoviamo il nemico: va a destra/sinistra (X) e scende verso il basso (Z decrescente)
            Vector3 movement = new Vector3(xMovement, 0f, -speed);
            transform.Translate(movement * Time.deltaTime, Space.World);

            // 3. Se supera il giocatore ed esce dal fondo dello schermo (Z minore di -2)
            if (transform.position.z < -2f)
            {
                ReturnToGrid();
            }
        }
    }

    void ReturnToGrid()
    {
        isDiving = false;
        
        // Torna ad essere figlio del GridManager
        transform.parent = originalParent;
        
        // Si riposiziona istantaneamente nel suo slot perfetto della griglia
        transform.localPosition = originalLocalPosition;
    }

    // Il tuo vecchio codice di collisione con il laser va qui sotto...
    private void OnCollisionEnter(Collision collision)
    {
        // 1. Se il nemico viene colpito dal laser del giocatore
        if (collision.gameObject.CompareTag("Laser")) 
        {
            Destroy(collision.gameObject); // Distrugge il laser
            Destroy(gameObject);           // Distrugge il nemico
        }

        // 2. NUOVO: Se il nemico si scontra con il Player!
        if (collision.gameObject.CompareTag("Player"))
        {
            Destroy(collision.gameObject); // La navicella del giocatore esplode (sparisce)
            Destroy(gameObject);           // Anche il nemico si polverizza nello scontro

            GameManager.instance.TriggerGameOver();
            
            Debug.Log("🔴 GAME OVER!!!");
        }
    }
}