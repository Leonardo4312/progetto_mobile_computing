using UnityEngine;

public class Laser3D : MonoBehaviour
{
    public float speed = 30f;       
    public float lifeTime = 3f;     
    private Rigidbody rb;

    void Start()
    {
        // Prendiamo il componente Rigidbody del laser
        rb = GetComponent<Rigidbody>();

        // Diamo una spinta fisica costante in avanti lungo l'asse Z
        rb.linearVelocity = Vector3.forward * speed;

        // Autodistruzione programmata
        Destroy(gameObject, lifeTime);
    }
}