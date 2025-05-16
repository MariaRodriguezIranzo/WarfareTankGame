using System.Collections;
using UnityEngine;

public class ControllerNPC : MonoBehaviour
{
    public int maxHealth = 3;
    private int currentHealth;
    private Animator animator;
    private SpriteRenderer spriteRenderer;
    public AudioClip damageSound;  // Sonido de da�o
    public AudioClip deathSound;    // Sonido de muerte
    private AudioSource audioSource;

    void Start()
    {
        currentHealth = maxHealth;
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        audioSource = GetComponent<AudioSource>(); // Obt�n el componente AudioSource
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        Debug.Log($"NPC recibi� da�o. Vida restante: {currentHealth}");

        // Reproduce sonido de da�o
        if (audioSource != null && damageSound != null)
        {
            audioSource.PlayOneShot(damageSound);  // Reproduce el sonido de da�o
        }

        StartCoroutine(FlashDamageEffect()); // Efecto de cambio de color al recibir da�o

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        Debug.Log("NPC ha muerto.");

        // Reproduce sonido de muerte
        if (audioSource != null && deathSound != null)
        {
            audioSource.PlayOneShot(deathSound);  // Reproduce el sonido de muerte
        }

        StartCoroutine(PlayDeathAnimation());
    }

    private IEnumerator PlayDeathAnimation()
    {
        // Reproduce la animaci�n de muerte
        if (animator != null)
        {
            animator.SetTrigger("Die");
            // Espera la duraci�n de la animaci�n de muerte antes de destruir el objeto
            float deathAnimationDuration = animator.GetCurrentAnimatorStateInfo(0).length;
            yield return new WaitForSeconds(deathAnimationDuration); // Espera que termine la animaci�n
        }

        // Destruye el objeto despu�s de la animaci�n
        Destroy(gameObject);
    }

    private IEnumerator FlashDamageEffect()
    {
        // Aseg�rate de que el SpriteRenderer est� presente
        if (spriteRenderer != null)
        {
            // Se pone rojo intenso para que se vea el da�o
            spriteRenderer.color = Color.red;
            yield return new WaitForSeconds(0.2f); // El rojo durar� 0.2 segundos
            spriteRenderer.color = Color.white; // Vuelve al color original
        }
    }
}