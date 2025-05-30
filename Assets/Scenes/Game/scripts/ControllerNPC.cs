using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ControllerNPC : MonoBehaviour
{
    [Header("Vida")]
    public int maxHealth = 3;
    private int currentHealth;

    [Header("Componentes")]
    private Animator animator;
    private SpriteRenderer spriteRenderer;

    [Header("Audios")]
    public AudioSource audioSource;
    public AudioClip damageSound;
    public AudioClip deathSound;

    [Header("Comportamiento")]
    public bool isAlerted = false; // ← Esto se activa cuando recibe daño
    public float alertDuration = 5f; // ← Tiempo que persigue tras ser atacado
    private float alertTimer = 0f;

    void Start()
    {
        currentHealth = maxHealth;
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        if (audioSource == null)
        {
            Debug.LogWarning("Falta asignar el AudioSource en el Inspector en " + gameObject.name);
        }
    }

    void Update()
    {
        if (isAlerted)
        {
            alertTimer -= Time.deltaTime;

            if (alertTimer <= 0f)
            {
                isAlerted = false;
                Debug.Log("NPC deja de estar alerta.");
            }
        }
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        Debug.Log($"NPC recibió daño. Vida restante: {currentHealth}");

        // Sonido de daño
        if (audioSource != null && damageSound != null)
        {
            audioSource.PlayOneShot(damageSound);
        }

        StartCoroutine(FlashDamageEffect());

        // ⚠️ Activar modo alerta
        ActivarAlerta();

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void ActivarAlerta()
    {
        isAlerted = true;
        alertTimer = alertDuration;
        Debug.Log("NPC entra en modo alerta. ¡Busca al jugador!");
        // Aquí puedes lanzar animaciones, efectos visuales o notificar a otro script
    }

    private void Die()
    {
        Debug.Log("NPC ha muerto.");

        if (audioSource != null && deathSound != null)
        {
            audioSource.PlayOneShot(deathSound);
        }

        StartCoroutine(PlayDeathAnimation());

        if (NPCManager.instance != null)
        {
            NPCManager.instance.NPCMuerto();
        }
    }

    private IEnumerator PlayDeathAnimation()
    {
        if (animator != null)
        {
            animator.SetTrigger("Die");
            float duration = animator.GetCurrentAnimatorStateInfo(0).length;
            yield return new WaitForSeconds(duration);
        }

        Destroy(gameObject);
    }

    private IEnumerator FlashDamageEffect()
    {
        if (spriteRenderer != null)
        {
            spriteRenderer.color = Color.red;
            yield return new WaitForSeconds(0.2f);
            spriteRenderer.color = Color.white;
        }
    }
}
