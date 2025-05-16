using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float bulletSpeed = 10f;
    public float lifetime = 2f;
    public int damage = 1;
    private Rigidbody rb;
    public AudioClip shootingSound;  // Sonido del disparo
    private AudioSource audioSource;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();  // Obtener el AudioSource

        if (rb == null)
        {
            Debug.LogError("Falta Rigidbody en la bala!");
            return;
        }

        // Reproducir sonido de disparo
        if (audioSource != null && shootingSound != null)
        {
            audioSource.PlayOneShot(shootingSound);  // Reproduce el sonido del disparo
        }

        rb.velocity = transform.up * bulletSpeed;
        Destroy(gameObject, lifetime);
    }

    void OnTriggerEnter(Collider other)
    {
        Debug.Log("Colisión detectada con: " + other.name);  // Verifica qué objeto está colisionando

        // Verifica si colisiona con el jugador
        ControllerPlayer player = other.GetComponent<ControllerPlayer>();
        if (player != null)
        {
           
            Destroy(gameObject); // Destruye la bala al impactar con el jugador
            return;
        }

        // Verifica si colisiona con un NPC
        ControllerNPC npc = other.GetComponent<ControllerNPC>();
        if (npc != null)
        {
            Debug.Log("Bala impactó al NPC");
            npc.TakeDamage(damage);  // Aplica el daño
            Destroy(gameObject);  // Destruye la bala al impactar con el NPC
        }
    }
}
