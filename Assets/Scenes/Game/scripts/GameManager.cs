using System.Collections;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public VidaTanque[] tanques;  // Asigna todos los tanques aquí en el inspector
    public Transform[] puntosSpawn; // Los spawn points en la escena
    public int vidasJugador = 3; // Vidas que tiene el jugador (mínimo 0)
    public int monedas = 0; // Monedas acumuladas

    public float tiempoRespawn = 3f;

    public void TanqueMuerto(VidaTanque tanque)
    {
        vidasJugador--;

        if (vidasJugador <= 0)
        {
            Debug.Log("Juego terminado. Se acabaron las vidas.");
            // Aquí puedes hacer lo que quieras, tipo game over, reiniciar, etc.
            return;
        }

        // Espera un tiempo y respawnea el tanque
        StartCoroutine(RespawnTanque(tanque));
    }

    IEnumerator RespawnTanque(VidaTanque tanque)
    {
        yield return new WaitForSeconds(tiempoRespawn);

        // Elige un spawn random
        Vector3 spawnPos = puntosSpawn[Random.Range(0, puntosSpawn.Length)].position;
        tanque.Respawnear(spawnPos);
    }

    // Aquí podrías añadir métodos para sumar monedas, comprar vidas, etc.
}
