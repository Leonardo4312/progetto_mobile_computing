using UnityEngine;

public class GridManager : MonoBehaviour
{
    [Header("Enemy Prefabs")]
    public GameObject beePrefab;       
    public GameObject butterflyPrefab; 
    public GameObject bossPrefab;      

    [Header("Grid Settings")]
    private int rows = 5;
    private int cols = 8;
    public float spacingX = 1.5f;     
    public float spacingZ = 1.1f;     

    [Header("Movement Settings")]
    public float speed = 2f;          
    public float distance = 1.5f;     
    private Vector3 startPosition;

    [Header("Attack Settings")]
    public float attackInterval = 3f;  // Ogni quanti secondi parte un nemico in picchiata
    private float attackTimer;

    void Start()
    {
        startPosition = transform.position; 
        GenerateGrid();
        attackTimer = attackInterval; // Fa partire il primo attacco quasi subito
    }

    void Update()
    {
        // Movimento di oscillazione della griglia
        float newX = startPosition.x + Mathf.Sin(Time.time * speed) * distance;
        transform.position = new Vector3(newX, transform.position.y, transform.position.z);

        // --- LOGICA DI ATTACCO ---
        attackTimer -= Time.deltaTime;
        if (attackTimer <= 0f)
        {
            LaunchRandomEnemy();
            attackTimer = attackInterval; // Resetta il timer
        }
    }

    void LaunchRandomEnemy()
    {
        // Cerca tutti gli script Enemy3D che sono attualmente figli della griglia (quelli vivi e non in picchiata)
        Enemy3D[] activeEnemies = GetComponentsInChildren<Enemy3D>();

        if (activeEnemies.Length > 0)
        {
            // Scegli un indice a caso dall'elenco dei nemici vivi
            int randomIndex = Random.Range(0, activeEnemies.Length);
            
            // Ordina a quel nemico specifico di partire in picchiata!
            activeEnemies[randomIndex].StartDive();
        }
    }

    void GenerateGrid()
    {
        for (int row = 0; row < rows; row++)
        {
            for (int col = 0; col < cols; col++)
            {
                if (row == 4 && (col < 2 || col > 5)) continue;

                GameObject prefabToSpawn = beePrefab; 

                if (row == 2 || row == 3) prefabToSpawn = butterflyPrefab;  
                else if (row == 4) prefabToSpawn = bossPrefab;       

                float posX = (col - (cols - 1) / 2f) * spacingX;
                float posZ = (row - (rows - 1) / 2f) * spacingZ;

                Vector3 spawnPosition = transform.position + new Vector3(posX, 0f, posZ);

                GameObject newEnemy = Instantiate(prefabToSpawn, spawnPosition, prefabToSpawn.transform.rotation);
                newEnemy.transform.parent = transform;
            }
        }
    }
}
