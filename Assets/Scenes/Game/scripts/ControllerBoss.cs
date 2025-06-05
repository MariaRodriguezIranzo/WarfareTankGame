using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class ControllerBoss : MonoBehaviour
{
    [Header("Vida")]
    public float HP = 760f;

    [Header("Fase del Boss")]
    private bool fase2Activa = false;
    private bool fase3Activa = false;
    private float dañoAcumulado = 0f;

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

    [Header("Partículas de Fases")]
    public GameObject fase2ParticulasPrefab;
    public GameObject fase3ParticulasPrefab;

    void Start()
    {
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

    public void TakeDamage(float damage)
    {
        HP -= damage;
        dañoAcumulado += damage;

        Debug.Log($"💢 Boss recibió daño: {damage}. HP restante: {HP}");

        if (audioSource && damageSound) audioSource.PlayOneShot(damageSound);
        StartCoroutine(FlashDamage());

        // Fase intermedia: si acumuló 120 o más, dispara doble y reinicia contador
        if (!fase2Activa && dañoAcumulado >= 120f)
        {
            dañoAcumulado = 0f;
            Transform jugador = ObtenerJugadorMasCercano();
            if (jugador != null)
            {
                Disparar(jugador, 2);
                tiempoUltimoDisparo = Time.time;
            }
        }

        // Activar Fase 2 permanente
        if (!fase2Activa && HP <= 550f)
        {
            fase2Activa = true;
            Debug.Log("🔥 Boss entra en FASE 2: disparo doble permanente");

            if (fase2ParticulasPrefab != null)
                Instantiate(fase2ParticulasPrefab, transform.position, Quaternion.identity, transform);
        }

        // Activar Fase 3 (Final)
        if (!fase3Activa && HP <= 300f)
        {
            fase3Activa = true;
            Debug.Log("💀 Boss entra en FASE FINAL: ráfagas triples");

            if (fase3ParticulasPrefab != null)
                Instantiate(fase3ParticulasPrefab, transform.position, Quaternion.identity, transform);
        }

        if (HP <= 0)
        {
            Morir();
        }
    }

    void Disparar(Transform objetivo, int cantidad = 1)
    {
        if (proyectilPrefab == null || puntoDisparo == null || objetivo == null) return;

        Vector3 direccion = (objetivo.position - puntoDisparo.position).normalized;

        for (int i = 0; i < cantidad; i++)
        {
            Quaternion rotacion = Quaternion.LookRotation(direccion);

            if (cantidad == 3) // Fase 3: Disparo ráfaga con ligero spread
            {
                float spread = Random.Range(-5f, 5f);
                rotacion = Quaternion.Euler(rotacion.eulerAngles + new Vector3(0, spread, 0));
            }

            GameObject proyectil = Instantiate(proyectilPrefab, puntoDisparo.position, rotacion);
            Rigidbody rb = proyectil.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.velocity = proyectil.transform.forward * 10f;
            }
        }

        Debug.Log($"🔫 Boss disparó {cantidad} proyectil(es)");
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
