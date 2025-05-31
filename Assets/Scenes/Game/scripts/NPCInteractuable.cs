using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using System.Linq;

public class NPCInteractuable : MonoBehaviour
{
    [Header("Waypoints")]
    public Transform[] waypoints;
    private int waypointIndex = 0;
    private bool esperandoEnWaypoint = false;

    [Header("Interacción")]
    public int monedasQueDa = 5;
    public float distanciaInteraccion = 3f;
    public int maxInteracciones = 3;
    private int contadorInteracciones = 0;
    public GameObject textoInteraccionUI;

    [Header("Detección y ataque")]
    public float rangoDeteccion = 10f;
    public GameObject balaPrefab;
    public Transform puntoDisparo;
    public float cadenciaDisparo = 1.5f;
    private bool puedeDisparar = true;

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip sonidoCompra;

    [Header("Efectos Visuales")]
    public GameObject particulasModoDiabloPrefab;
    public GameObject particulasEstadoHostilPrefab;

    private GameObject particulasModoDiabloInstanciadas;
    private GameObject particulasEstadoHostilInstanciadas;

    private Transform jugadorCercano;
    private NavMeshAgent agent;

    private bool modoDiabloActivo = false;
    private bool estadoHostilPermanente = false;
    private bool jugadorDetectado = false;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        if (textoInteraccionUI != null)
            textoInteraccionUI.SetActive(false);

        StartCoroutine(Patrullar());
    }

    void Update()
    {
        ActualizarJugadorCercano();

        float distancia = jugadorCercano != null ? Vector3.Distance(transform.position, jugadorCercano.position) : Mathf.Infinity;
        jugadorDetectado = jugadorCercano != null && distancia <= rangoDeteccion;

        if (!estadoHostilPermanente && jugadorDetectado && !modoDiabloActivo)
        {
            textoInteraccionUI.SetActive(true);

            if (Input.GetKeyDown(KeyCode.E))
            {
                if (contadorInteracciones < maxInteracciones)
                {
                    Interactuar();
                }
                else
                {
                    StartCoroutine(ActivarModoDiablo());
                }
            }
        }
        else
        {
            textoInteraccionUI.SetActive(false);
        }

        if ((modoDiabloActivo || estadoHostilPermanente) && jugadorDetectado)
        {
            PerseguirJugador(jugadorCercano);
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

    void Interactuar()
    {
        contadorInteracciones++;
        Debug.Log($"Interacción {contadorInteracciones} de {maxInteracciones}");

        GameManager gm = FindObjectOfType<GameManager>();
        if (gm != null)
        {
            gm.AñadirMonedas(monedasQueDa);

            if (audioSource != null && sonidoCompra != null)
            {
                audioSource.PlayOneShot(sonidoCompra);
            }
        }
    }

    public void RecibirDisparo()
    {
        if (!modoDiabloActivo && !estadoHostilPermanente)
        {
            Debug.Log("🔫 NPC recibió disparo. Activando modo diablo...");
            StartCoroutine(ActivarModoDiablo());
        }
    }

    IEnumerator ActivarModoDiablo()
    {
        if (modoDiabloActivo || estadoHostilPermanente) yield break;

        modoDiabloActivo = true;
        textoInteraccionUI.SetActive(false);

        // Instanciar partículas de modo diablo si hay prefab asignado
        if (particulasModoDiabloPrefab != null && particulasModoDiabloInstanciadas == null)
        {
            particulasModoDiabloInstanciadas = Instantiate(particulasModoDiabloPrefab, transform);
            particulasModoDiabloInstanciadas.SetActive(true);
        }

        Debug.Log("🔴 ¡Modo Diablo ACTIVADO por 5 segundos!");

        yield return new WaitForSeconds(5f);

        modoDiabloActivo = false;

        // Destruir partículas modo diablo
        if (particulasModoDiabloInstanciadas != null)
        {
            Destroy(particulasModoDiabloInstanciadas);
            particulasModoDiabloInstanciadas = null;
        }

        // Activar estado hostil permanente
        estadoHostilPermanente = true;

        // Instanciar partículas de estado hostil si hay prefab asignado
        if (particulasEstadoHostilPrefab != null && particulasEstadoHostilInstanciadas == null)
        {
            particulasEstadoHostilInstanciadas = Instantiate(particulasEstadoHostilPrefab, transform);
            particulasEstadoHostilInstanciadas.SetActive(true);
        }

        Debug.Log("🟡 NPC entra en estado Hostil Permanente. Patrulla sin pausas y ataca al jugador si lo ve.");
    }

    void PerseguirJugador(Transform jugador)
    {
        if (jugador == null) return;

        agent.SetDestination(jugador.position);

        Vector3 direction = (jugador.position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);

        if (puedeDisparar)
        {
            Disparar();
        }
    }

    void Disparar()
    {
        if (balaPrefab != null && puntoDisparo != null)
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

    IEnumerator Patrullar()
    {
        while (true)
        {
            if (!jugadorDetectado && !modoDiabloActivo)
            {
                agent.SetDestination(waypoints[waypointIndex].position);

                if (Vector3.Distance(transform.position, waypoints[waypointIndex].position) < 1f)
                {
                    if (!estadoHostilPermanente)
                    {
                        esperandoEnWaypoint = true;
                        yield return new WaitForSeconds(15f);
                        esperandoEnWaypoint = false;
                    }

                    waypointIndex = (waypointIndex + 1) % waypoints.Length;
                }
            }

            yield return null;
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, rangoDeteccion);
    }

    void OnDestroy()
    {
        if (particulasModoDiabloInstanciadas != null)
            Destroy(particulasModoDiabloInstanciadas);
        if (particulasEstadoHostilInstanciadas != null)
            Destroy(particulasEstadoHostilInstanciadas);
    }
}
