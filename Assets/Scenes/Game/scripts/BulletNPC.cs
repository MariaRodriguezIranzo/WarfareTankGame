using UnityEngine;

public class EnemyBullet : MonoBehaviour
{
    public float bulletSpeed = 10f;
    public float lifetime = 2f;
    public int damage = 1;
    private Rigidbody rb;

    [Header("Audio")] 
    public AudioSource audioSource;
    public AudioClip shootingSound;
   

    void Start()
    {
        rb = GetComponent<Rigidbody>();

        if (rb == null)
        {
            Debug.LogError("❌ Falta Rigidbody en la bala del NPC!");
            return;
        }

        // Sonido de disparo
        if (audioSource != null && shootingSound != null)
        {
            audioSource.PlayOneShot(shootingSound);
        }
        else
        {
            Debug.LogWarning("🎧 AudioSource o shootingSound no asignado en " + gameObject.name);
        }

        rb.velocity = transform.forward * bulletSpeed;
        Destroy(gameObject, lifetime);
    }

    void OnTriggerEnter(Collider other)
    {
        Debug.Log("🎯 Bala NPC impacta con: " + other.name);

        // Aplica daño al jugador
        VidaTanque vida = other.GetComponent<VidaTanque>();
        if (vida != null)
        {
            vida.RecibirDaño(damage);
            Destroy(gameObject);
        }
    }
}
