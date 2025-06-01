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

    [Header("Disparo")]
    public GameObject proyectilPrefab;
    public Transform puntoDisparo;
    public float cooldownDisparo = 2f;
    public float distanciaDisparo = 10f;
    private float tiempoUltimoDisparo = 0f;

    [Header("Persecución")]
    public float rangoDeteccion = 30f;
    public float velocidadMovimiento = 4f;
    public float distanciaMinimaAlJugador = 3f;

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
        spriteRenderer = GetComponent<SpriteRenderer>();
        agente = GetComponent<NavMeshAgent>();
        if (agente != null) agente.speed = velocidadMovimiento;
    }

    void Update()
    {
        Transform jugador = ObtenerJugadorMasCercano();

        if (jugador != null)
        {
            PerseguirYAtacar(jugador);
        }
    }

    Transform ObtenerJugadorMasCercano()
    {
        GameObject[] jugadores = GameObject.FindGameObjectsWithTag("Player");

        if (jugadores.Length == 0)
            return null;

        return jugadores
            .Select(j => j.transform)
            .OrderBy(t => Vector3.Distance(transform.position, t.position))
            .FirstOrDefault();
    }

    void PerseguirYAtacar(Transform jugador)
    {
        if (jugador == null || agente == null) return;

        float distancia = Vector3.Distance(transform.position, jugador.position);

        if (distancia <= rangoDeteccion)
        {
            if (distancia > distanciaMinimaAlJugador)
            {
                agente.isStopped = false;
                agente.SetDestination(jugador.position);
            }
            else
            {
                agente.isStopped = true;
            }

            if (Time.time - tiempoUltimoDisparo >= cooldownDisparo && distancia <= distanciaDisparo)
            {
                Disparar(jugador);
                tiempoUltimoDisparo = Time.time;
            }
        }
        else
        {
            agente.isStopped = true;
        }
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        Debug.Log($"💢 Boss recibió daño. Vida actual: {currentHealth}");

        if (audioSource && damageSound) audioSource.PlayOneShot(damageSound);
        StartCoroutine(FlashDamage());

        // Dispara inmediatamente al recibir daño
        Transform jugador = ObtenerJugadorMasCercano();
        if (jugador != null)
        {
            Disparar(jugador);
            tiempoUltimoDisparo = Time.time; // resetea cooldown
        }

        if (!enSegundaFase && currentHealth <= maxHealth / 2)
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

    void Disparar(Transform objetivo)
    {
        if (proyectilPrefab != null && puntoDisparo != null)
        {
            Vector3 direccion = (objetivo.position - puntoDisparo.position).normalized;
            Quaternion rotacion = Quaternion.LookRotation(direccion);
            GameObject proyectil = Instantiate(proyectilPrefab, puntoDisparo.position, rotacion);

            Rigidbody rb = proyectil.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.velocity = direccion * 10f;
            }

            Debug.Log("🔫 Boss dispara");
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
