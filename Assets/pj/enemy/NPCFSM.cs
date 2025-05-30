using UnityEngine;
using UnityEngine.AI;
using System.Linq;
using System.Collections;

public class NPCFSM : MonoBehaviour
{
    public enum State { Patrullar, Perseguir, Huir, JugadorCercano }
    public State currentState;

    public Transform[] waypoints;
    private int currentWaypoint = 0;

    [Header("Rangos")]
    public float detectionDistance = 10f;
    public float huidaDistance = 15f;
    public float distanciaParaHuir = 5f;
    public float distanciaJugadorCercano = 1.5f;
    public float minDistanciaHuida = 7f;
    public float distanciaBuffer = 2f;

    [Header("Jugadores")]
    public Transform[] players;
    public Transform targetPlayer;
    public Transform jugadorPrincipal;

    public GameObject spawnTank;

    private NavMeshAgent agent;

    [Header("Disparo")]
    public GameObject balaNPCPrefab;
    public Transform puntoDisparo;
    public float cadenciaDisparo = 1.5f;
    private bool puedeDisparar = true;

    // Control alerta y timer para volver a patrullar
    private bool isAlertedTimerRunning = false;
    private float alertaTimer = 0f;
    public float alertaDuracion = 2f; // segundos que persigue tras alerta

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        UpdatePlayersArray();
        FindJugadorPrincipal();
        currentState = State.Patrullar;
        GoToNextWaypoint();
    }

    void Update()
    {
        UpdatePlayersArray();
        FindJugadorPrincipal();
        UpdateTargetPlayerByDistance();

        RevisarTransiciones();

        if (isAlertedTimerRunning)
        {
            alertaTimer += Time.deltaTime;
            if (alertaTimer >= alertaDuracion)
            {
                currentState = State.Patrullar;
                agent.isStopped = false;
                GoToClosestWaypoint(); // Va al waypoint más cercano para patrullar
                isAlertedTimerRunning = false;
                alertaTimer = 0f;
            }
        }

        switch (currentState)
        {
            case State.Patrullar:
                Patrullar();
                break;
            case State.Perseguir:
                Perseguir();
                break;
            case State.Huir:
                Huir();
                break;
            case State.JugadorCercano:
                JugadorCercano();
                break;
        }
    }

    void UpdatePlayersArray()
    {
        GameObject[] playersGO = GameObject.FindGameObjectsWithTag("Player");
        players = playersGO.Select(go => go.transform).ToArray();
    }

    void FindJugadorPrincipal()
    {
        if (spawnTank != null && spawnTank.transform.childCount > 0)
        {
            jugadorPrincipal = spawnTank.transform.GetChild(0);
        }
        else
        {
            jugadorPrincipal = null;
        }
    }

    void UpdateTargetPlayerByDistance()
    {
        float minDist = Mathf.Infinity;
        Transform nearestPlayer = null;

        foreach (Transform p in players)
        {
            if (p == null) continue;
            float dist = Vector3.Distance(transform.position, p.position);
            if (dist < minDist && dist <= detectionDistance)
            {
                minDist = dist;
                nearestPlayer = p;
            }
        }
        targetPlayer = nearestPlayer;
    }

    void RevisarTransiciones()
    {
        ControllerNPC controller = GetComponent<ControllerNPC>();
        if (controller != null && controller.isAlerted)
        {
            if (jugadorPrincipal != null)
            {
                targetPlayer = jugadorPrincipal;
                currentState = State.Perseguir;

                // Empezar timer para volver a patrullar en 2 segundos
                if (!isAlertedTimerRunning)
                {
                    isAlertedTimerRunning = true;
                    alertaTimer = 0f;
                }
            }
            return;
        }

        if (jugadorPrincipal == null)
        {
            if (currentState != State.Patrullar)
            {
                currentState = State.Patrullar;
                agent.isStopped = false;
                GoToClosestWaypoint();
            }
            return;
        }

        float distToJugador = Vector3.Distance(transform.position, jugadorPrincipal.position);

        switch (currentState)
        {
            case State.Patrullar:
                if (distToJugador <= detectionDistance)
                    currentState = State.Perseguir;
                break;

            case State.Perseguir:
                if (distToJugador < distanciaJugadorCercano)
                    currentState = State.JugadorCercano;
                else if (distToJugador > detectionDistance + distanciaBuffer)
                {
                    currentState = State.Patrullar;
                    agent.isStopped = false;
                    GoToClosestWaypoint();
                }
                break;

            case State.JugadorCercano:
                if (distToJugador > distanciaJugadorCercano + distanciaBuffer)
                    currentState = State.Perseguir;
                break;

            case State.Huir:
                if (distToJugador > distanciaParaHuir + distanciaBuffer)
                {
                    currentState = State.Patrullar;
                    agent.isStopped = false;
                    GoToClosestWaypoint();
                }
                break;
        }
    }

    void Patrullar()
    {
        if (!agent.pathPending && agent.remainingDistance < 0.5f)
        {
            currentWaypoint = (currentWaypoint + 1) % waypoints.Length;
            GoToNextWaypoint();
        }
    }

    void GoToNextWaypoint()
    {
        if (waypoints.Length > 0)
            agent.SetDestination(waypoints[currentWaypoint].position);
    }

    void GoToClosestWaypoint()
    {
        if (waypoints.Length == 0) return;

        float minDist = Mathf.Infinity;
        int closestIndex = 0;

        for (int i = 0; i < waypoints.Length; i++)
        {
            float dist = Vector3.Distance(transform.position, waypoints[i].position);
            if (dist < minDist)
            {
                minDist = dist;
                closestIndex = i;
            }
        }

        currentWaypoint = closestIndex;
        agent.SetDestination(waypoints[currentWaypoint].position);
    }

    void Perseguir()
    {
        if (targetPlayer != null)
        {
            agent.isStopped = false;
            agent.SetDestination(targetPlayer.position);

            Vector3 direction = (targetPlayer.position - transform.position).normalized;
            Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);

            if (puedeDisparar)
            {
                Disparar();
            }
        }
    }

    void Huir()
    {
        if (targetPlayer == null) return;

        float distToPlayer = Vector3.Distance(transform.position, targetPlayer.position);
        if (distToPlayer > distanciaParaHuir + distanciaBuffer)
        {
            currentState = State.Patrullar;
            agent.isStopped = false;
            GoToClosestWaypoint();
            return;
        }

        Vector3 awayDir = (transform.position - targetPlayer.position).normalized;
        Vector3 escapePos = transform.position + awayDir * (huidaDistance + minDistanciaHuida);

        if (NavMesh.SamplePosition(escapePos, out NavMeshHit hit, huidaDistance + 10f, NavMesh.AllAreas))
        {
            agent.isStopped = false;
            agent.SetDestination(hit.position);
        }
        else
        {
            agent.Move(awayDir * Time.deltaTime * agent.speed);
        }
    }

    void JugadorCercano()
    {
        if (jugadorPrincipal == null) return;

        float dist = Vector3.Distance(transform.position, jugadorPrincipal.position);

        if (dist > detectionDistance + distanciaBuffer)
        {
            currentState = State.Patrullar;
            agent.isStopped = false;
            GoToClosestWaypoint();
            return;
        }

        agent.isStopped = false;

        Vector3 awayDir = (transform.position - jugadorPrincipal.position).normalized;
        Vector3 escapePos = transform.position + awayDir * (huidaDistance + minDistanciaHuida);

        if (NavMesh.SamplePosition(escapePos, out NavMeshHit hit, huidaDistance + 10f, NavMesh.AllAreas))
        {
            agent.SetDestination(hit.position);
        }
        else
        {
            agent.Move(awayDir * Time.deltaTime * agent.speed);
        }
    }

    void Disparar()
    {
        if (balaNPCPrefab != null && puntoDisparo != null)
        {
            Instantiate(balaNPCPrefab, puntoDisparo.position, puntoDisparo.rotation);
            puedeDisparar = false;
            StartCoroutine(TemporizadorDisparo());
        }
    }

    IEnumerator TemporizadorDisparo()
    {
        yield return new WaitForSeconds(cadenciaDisparo);
        puedeDisparar = true;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionDistance);

        if (targetPlayer != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.position + Vector3.up, targetPlayer.position);
        }

        if (jugadorPrincipal != null)
        {
            Gizmos.color = Color.magenta;
            Gizmos.DrawWireSphere(transform.position, distanciaJugadorCercano);
        }
    }
}
