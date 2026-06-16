using UnityEngine;

public class PlayerGalaga : MonoBehaviour
{
    [Header("Movement Settings")]
    public float speed = 8f;             
    public float xLimit = 7.5f;          

    [Header("Shooting Settings")]
    public GameObject laserPrefab;       
    public float fireRate = 0.25f;        
    private float nextFireTime = 0f;

    [Header("Power-Up Status")]
    public bool hasDoubleLaser = false;  

    private float originalZ; // <--- NUOVO: Manterrà la navicella ancorata al suo binario

    void Start()
    {
        // Memorizziamo la Z di partenza per evitare che la fisica ci spinga indietro
        originalZ = transform.position.z;
    }

    void Update()
    {
        // 1. MOVIMENTO ORIZZONTALE
        float horizontalInput = Input.GetAxis("Horizontal");
        Vector3 direction = new Vector3(horizontalInput, 0f, 0f);
        transform.Translate(direction * speed * Time.deltaTime, Space.World);

        // 2. BLOCCO DEI BORDI (X) E ANCORAGGIO FISSO (Z)
        float clampedX = Mathf.Clamp(transform.position.x, -xLimit, xLimit);
        
        // Forza la navicella a stare sulla sua X controllata e SEMPRE sulla Z originale
        transform.position = new Vector3(clampedX, transform.position.y, originalZ);

        // 3. INPUT DI SPARO
        if ((Input.GetButton("Jump") || Input.GetKeyDown(KeyCode.Space)) && Time.time > nextFireTime)
        {
            nextFireTime = Time.time + fireRate; 
            Shoot();
        }
    }

    void Shoot()
    {
        if (laserPrefab != null)
        {
            if (hasDoubleLaser)
            {
                Vector3 leftSpawn = transform.position + new Vector3(-0.3f, 0f, 0.5f);
                Vector3 rightSpawn = transform.position + new Vector3(0.3f, 0f, 0.5f);

                Instantiate(laserPrefab, leftSpawn, Quaternion.identity);
                Instantiate(laserPrefab, rightSpawn, Quaternion.identity);
            }
            else
            {
                Vector3 singleSpawn = transform.position + new Vector3(0f, 0f, 0.5f);
                Instantiate(laserPrefab, singleSpawn, Quaternion.identity);
            }
        }
    }

    public void ActivateDoubleLaser()
    {
        hasDoubleLaser = true;
    }
}
