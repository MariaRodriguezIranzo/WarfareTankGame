using UnityEngine;
using UnityEngine.AI;
using System.Linq;

public class npcFSM : MonoBehaviour
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
    private Transform targetPlayer;
    private Transform jugadorPrincipal;

    public GameObject spawnTank;

    private NavMeshAgent agent;

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
        if (jugadorPrincipal == null)
        {
            if (currentState != State.Patrullar)
            {
                currentState = State.Patrullar;
                agent.isStopped = false;
                GoToNextWaypoint();
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
                    GoToNextWaypoint();
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
                    GoToNextWaypoint();
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

    void Perseguir()
    {
        if (targetPlayer != null)
        {
            agent.isStopped = false;
            agent.SetDestination(targetPlayer.position);
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
            GoToNextWaypoint();
            return;
        }

        Vector3 awayDir = (transform.position - targetPlayer.position).normalized;
        Vector3 escapePos = transform.position + awayDir * (huidaDistance + minDistanciaHuida);

        NavMeshHit hit;
        if (NavMesh.SamplePosition(escapePos, out hit, huidaDistance + 10f, NavMesh.AllAreas))
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
            GoToNextWaypoint();
            return;
        }

        agent.isStopped = false;

        Vector3 awayDir = (transform.position - jugadorPrincipal.position).normalized;
        Vector3 escapePos = transform.position + awayDir * (huidaDistance + minDistanciaHuida);

        NavMeshHit hit;
        if (NavMesh.SamplePosition(escapePos, out hit, huidaDistance + 10f, NavMesh.AllAreas))
        {
            agent.SetDestination(hit.position);
        }
        else
        {
            agent.Move(awayDir * Time.deltaTime * agent.speed);
        }
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