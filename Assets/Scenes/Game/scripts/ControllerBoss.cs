using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class ControllerBoss : MonoBehaviour
{
    [Header("Vida")]
    public int maxHealth = 10;
    private int currentHealth;

    [Header("Fase del Boss")]
    public bool enSegundaFase = false;
    private int ultimaVidaCheck = 10;

    [Header("Ataque Especial")]
    public GameObject ataqueEspecialPrefab;
    public Transform puntoDisparo;
    public float cooldownEspecial = 5f;
    private float tiempoUltimoEspecial = 0f;

    [Header("Persecución")]
    public float rangoDeteccion = 30f;
    public float velocidadMovimiento = 4f;

    [Header("Componentes")]
    private SpriteRenderer spriteRenderer;
    private NavMeshAgent agente;

    [Header("Audio / Efectos")]
    public AudioSource audioSource;
    public AudioClip damageSound;
    public AudioClip deathSound;
    public GameObject particulasMuertePrefab;

    void Start()
    {
        currentHealth = maxHealth;
        ultimaVidaCheck = maxHealth;

        spriteRenderer = GetComponent<SpriteRenderer>();
        agente = GetComponent<NavMeshAgent>();
        if (agente != null)
        {
            agente.speed = velocidadMovimiento;
        }
    }

    void Update()
    {
        Transform jugadorCercano = ObtenerJugadorMasCercano();

        if (jugadorCercano != null)
            PerseguirJugador(jugadorCercano);

        if (Time.time - tiempoUltimoEspecial >= cooldownEspecial && (ultimaVidaCheck - currentHealth) >= 2)
        {
            LanzarAtaqueEspecial();
            tiempoUltimoEspecial = Time.time;
            ultimaVidaCheck = currentHealth;
        }
    }

    Transform ObtenerJugadorMasCercano()
    {
        GameObject[] jugadoresGO = GameObject.FindGameObjectsWithTag("Player");

        if (jugadoresGO.Length == 0)
            return null;

        return jugadoresGO
            .Select(go => go.transform)
            .OrderBy(t => Vector3.Distance(transform.position, t.position))
            .FirstOrDefault();
    }

    void PerseguirJugador(Transform jugador)
    {
        if (jugador == null || agente == null) return;

        float distancia = Vector3.Distance(transform.position, jugador.position);
        if (distancia <= rangoDeteccion)
        {
            agente.SetDestination(jugador.position);
        }
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        Debug.Log($"💢 Boss recibió daño. Vida actual: {currentHealth}");

        if (audioSource && damageSound) audioSource.PlayOneShot(damageSound);
        StartCoroutine(FlashDamage());

        if (!enSegundaFase && currentHealth <= 5)
        {
            ActivarSegundaFase();
        }

        if (currentHealth <= 0)
        {
            Morir();
        }
    }

    void ActivarSegundaFase()
    {
        enSegundaFase = true;
        Debug.Log("🔥 Boss entra en Segunda Fase. ¡Se vuelve más agresivo!");
        velocidadMovimiento *= 1.5f;
        if (agente != null) agente.speed = velocidadMovimiento;
    }

    void LanzarAtaqueEspecial()
    {
        if (ataqueEspecialPrefab != null && puntoDisparo != null)
        {
            Debug.Log("⚡ Boss lanza ATAQUE ESPECIAL");
            Instantiate(ataqueEspecialPrefab, puntoDisparo.position, puntoDisparo.rotation);
        }
    }

    void Morir()
    {
        Debug.Log("☠️ Boss ha muerto.");
        if (audioSource && deathSound) audioSource.PlayOneShot(deathSound);

        if (particulasMuertePrefab != null)
            Instantiate(particulasMuertePrefab, transform.position, Quaternion.identity);

        if (NPCManager.instance != null)
            NPCManager.instance.BossMuerto();

        Destroy(gameObject);
    }

    private IEnumerator FlashDamage()
    {
        if (spriteRenderer != null)
        {
            spriteRenderer.color = Color.red;
            yield return new WaitForSeconds(0.2f);
            spriteRenderer.color = Color.white;
        }
    }
}
