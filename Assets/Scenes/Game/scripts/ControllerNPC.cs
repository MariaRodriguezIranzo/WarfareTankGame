using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class ControllerNPC : MonoBehaviour
{
    [Header("Vida")]
    public int maxHealth = 3;
    private int currentHealth;

    [Header("Componentes")]
    private Animator animator;
    private SpriteRenderer spriteRenderer;
    private NPCInteractuable npcInteractuable;
    private NavMeshAgent agent;

    [Header("Audios")]
    public AudioSource audioSource;
    public AudioClip damageSound;
    public AudioClip deathSound;

    [Header("Comportamiento")]
    public bool isAlerted = false;
    public float alertDuration = 5f;
    private float alertTimer = 0f;

    [Header("Detección y ataque")]
    public float rangoDeteccion = 15f;
    private Transform jugadorCercano;
    public GameObject balaPrefab;
    public Transform puntoDisparo;
    public float cadenciaDisparo = 1.5f;
    private bool puedeDisparar = true;

    [Header("Efectos Visuales")]
    public GameObject particulasMuertePrefab;

    [Header("Drop de Moneda")]
    public GameObject monedaPrefab;

    void Start()
    {
        currentHealth = maxHealth;
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        npcInteractuable = GetComponent<NPCInteractuable>();
        agent = GetComponent<NavMeshAgent>();

        if (audioSource == null)
            Debug.LogWarning("Falta asignar el AudioSource en el Inspector en " + gameObject.name);
    }

    void Update()
    {
        ActualizarJugadorCercano();

        if (isAlerted)
        {
            alertTimer -= Time.deltaTime;

            if (jugadorCercano != null)
            {
                float distancia = Vector3.Distance(transform.position, jugadorCercano.position);

                if (distancia <= rangoDeteccion)
                {
                    PerseguirJugador(jugadorCercano);
                    Disparar(jugadorCercano);
                }
            }

            if (alertTimer <= 0f)
            {
                isAlerted = false;
                Debug.Log("NPC deja de estar alerta.");
            }
        }
    }

    void ActualizarJugadorCercano()
    {
        GameObject[] playersGO = GameObject.FindGameObjectsWithTag("Player");
        if (playersGO.Length == 0) { jugadorCercano = null; return; }

        jugadorCercano = playersGO
            .Select(go => go.transform)
            .OrderBy(t => Vector3.Distance(transform.position, t.position))
            .FirstOrDefault();
    }

    void PerseguirJugador(Transform jugador)
    {
        if (jugador == null || agent == null) return;
        agent.SetDestination(jugador.position);

        Vector3 direction = (jugador.position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);
    }

    void Disparar(Transform jugador)
    {
        if (puedeDisparar && balaPrefab != null && puntoDisparo != null)
        {
            Instantiate(balaPrefab, puntoDisparo.position, puntoDisparo.rotation);
            puedeDisparar = false;
            Invoke(nameof(ResetDisparo), cadenciaDisparo);
        }
    }

    void ResetDisparo()
    {
        puedeDisparar = true;
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
        Debug.Log("NPC entra en modo alerta. ¡Busca y ataca al jugador!");
    }

    private void Die()
    {
        Debug.Log("NPC ha muerto.");

        if (audioSource != null && deathSound != null)
        {
            audioSource.PlayOneShot(deathSound);
        }

        if (particulasMuertePrefab != null)
        {
            Instantiate(particulasMuertePrefab, transform.position, Quaternion.identity);
        }

        // 💰 Drop de moneda
        if (monedaPrefab != null)
        {
            Instantiate(monedaPrefab, transform.position + Vector3.up * 1f, Quaternion.identity);
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
