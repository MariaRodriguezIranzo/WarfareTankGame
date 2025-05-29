using UnityEngine;
using UnityEngine.AI;
using System.Linq;

public class NPCInteractuable : MonoBehaviour
{
    public int monedasQueDa = 5;
    public float distanciaInteraccion = 3f;
    public GameObject textoInteraccionUI;

    private Transform[] jugadores;
    private Transform jugadorCercano;
    private bool jugadorCerca = false;
    private bool modoDiablo = false;

    [Header("Interacción")]
    public int maxInteracciones = 3;
    private int contadorInteracciones = 0;

    [Header("Ataque")]
    public GameObject balaPrefab;
    public Transform puntoDisparo;
    public float cadenciaDisparo = 1.5f;
    private bool puedeDisparar = true;

    private NavMeshAgent agent;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        if (textoInteraccionUI != null)
            textoInteraccionUI.SetActive(false);

        ActualizarJugadores();
    }

    void Update()
    {
        ActualizarJugadores();
        jugadorCercano = ObtenerJugadorCercano();

        if (jugadorCercano != null)
        {
            float distancia = Vector3.Distance(transform.position, jugadorCercano.position);
            jugadorCerca = distancia <= distanciaInteraccion;

            if (jugadorCerca && !modoDiablo)
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
                        ActivarModoDiablo();
                    }
                }
            }
            else
            {
                textoInteraccionUI.SetActive(false);
            }

            if (modoDiablo)
            {
                PerseguirJugador(jugadorCercano);
            }
        }
        else
        {
            textoInteraccionUI.SetActive(false);
        }
    }

    void ActualizarJugadores()
    {
        GameObject[] playersGO = GameObject.FindGameObjectsWithTag("Player");
        jugadores = playersGO.Select(go => go.transform).ToArray();
    }

    Transform ObtenerJugadorCercano()
    {
        if (jugadores == null || jugadores.Length == 0) return null;

        Transform jugadorMasCercano = null;
        float minDist = Mathf.Infinity;

        foreach (var jugador in jugadores)
        {
            if (jugador == null) continue;
            float dist = Vector3.Distance(transform.position, jugador.position);
            if (dist < minDist)
            {
                minDist = dist;
                jugadorMasCercano = jugador;
            }
        }
        return jugadorMasCercano;
    }

    void Interactuar()
    {
        contadorInteracciones++;
        Debug.Log($"Interacción {contadorInteracciones} de {maxInteracciones}");

        GameManager gm = FindObjectOfType<GameManager>();
        if (gm != null)
        {
            gm.AñadirMonedas(monedasQueDa);
        }
    }

    void ActivarModoDiablo()
    {
        modoDiablo = true;
        textoInteraccionUI.SetActive(false);
        Debug.Log("¡Modo diablo activado por exceso de interacciones!");
    }

    public void TomarDaño()
    {
        if (!modoDiablo)
        {
            ActivarModoDiablo();
        }
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

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, distanciaInteraccion);
    }
}
