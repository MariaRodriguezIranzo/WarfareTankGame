using System.Collections;
using UnityEngine;

public class ControllerNPC : MonoBehaviour
{
    [Header("Vida")]
    public int maxHealth = 3;
    private int currentHealth;

    [Header("Componentes")]
    private Animator animator;
    private SpriteRenderer spriteRenderer;
    private NPCInteractuable npcInteractuable;

    [Header("Audios")]
    public AudioSource audioSource;
    public AudioClip damageSound;
    public AudioClip deathSound;

    [Header("Comportamiento")]
    public bool isAlerted = false;
    public float alertDuration = 5f;
    private float alertTimer = 0f;

    [Header("Efectos Visuales")]
    public GameObject particulasMuertePrefab;

    void Start()
    {
        currentHealth = maxHealth;
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        npcInteractuable = GetComponent<NPCInteractuable>();

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

        if (audioSource != null && damageSound != null)
        {
            audioSource.PlayOneShot(damageSound);
        }

        StartCoroutine(FlashDamageEffect());

        ActivarAlerta();

        if (npcInteractuable != null)
        {
            npcInteractuable.RecibirDisparo();
        }

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
    }

    private void Die()
    {
        Debug.Log("NPC ha muerto.");

        if (audioSource != null && deathSound != null)
        {
            audioSource.PlayOneShot(deathSound);
        }

        // Instanciar partículas de muerte si hay prefab asignado
        if (particulasMuertePrefab != null)
        {
            Instantiate(particulasMuertePrefab, transform.position, Quaternion.identity);
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
