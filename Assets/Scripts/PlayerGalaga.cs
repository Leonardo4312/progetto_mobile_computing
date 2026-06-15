using UnityEngine;

public class PlayerGalaga : MonoBehaviour
{
    public float speed = 12f;      // Velocità di movimento laterale
    public float xLimit = 8f;       // Limite dello schermo per non uscire fuori
    
    public GameObject laserPrefab; // Qui dentro trascineremo il proiettile

    void Update()
    {
        // 1. MOVIMENTO: prende l'input delle frecce o A/D
        float moveInput = Input.GetAxis("Horizontal");
        Vector3 newPosition = transform.position + new Vector3(moveInput * speed * Time.deltaTime, 0, 0);
        
        // Blocca la navicella nei bordi dello schermo
        newPosition.x = Mathf.Clamp(newPosition.x, -xLimit, xLimit);
        transform.position = newPosition;

        // 2. SPARO: se premi Barra Spaziatrice o il Click del mouse
        if (Input.GetButtonDown("Fire1"))
        {
            Shoot();
        }
    }

    void Shoot()
    {
        // Fa apparire il laser poco più avanti del muso della navicella (Z + 1)
        Instantiate(laserPrefab, transform.position + new Vector3(0, 0, 1f), Quaternion.identity);
    }
}
