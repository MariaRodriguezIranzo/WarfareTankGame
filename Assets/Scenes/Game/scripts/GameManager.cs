using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public VidaTanque[] tanques;         // No tocar desde el Inspector
    public Transform[] puntosSpawn;      // Asignar en Inspector

    public int vidasJugador = 3;
    public int monedas = 0;
    public float tiempoRespawn = 3f;

    [Header("UI")]
    public Image[] vidasUI;              // Corazones (hasta 5)
    public TextMeshProUGUI textoMonedas;

    private bool tanqueRegistrado = false;

    void Start()
    {
        StartCoroutine(EsperarYRegistrarTanque());
        ActualizarHUDVidas();
        ActualizarUIMonedas();
    }

    IEnumerator EsperarYRegistrarTanque()
    {
        // Espera hasta que el TankSpawner instancie el tanque
        while (!tanqueRegistrado)
        {
            VidaTanque tanqueInstanciado = FindObjectOfType<VidaTanque>();
            if (tanqueInstanciado != null)
            {
                tanques = new VidaTanque[] { tanqueInstanciado };
                tanqueInstanciado.OnTanqueMuerto += () => TanqueMuerto(tanqueInstanciado);
                tanqueRegistrado = true;
                Debug.Log("[GameManager] Tanque registrado correctamente.");
            }
            yield return null;
        }
    }

    public void TanqueMuerto(VidaTanque tanque)
    {
        vidasJugador--;

        if (vidasJugador <= 0)
        {
            Debug.Log("Game Over");
            SceneManager.LoadScene("GameOver");
            ActualizarHUDVidas();
            return;
        }

        ActualizarHUDVidas();
        StartCoroutine(RespawnTanque(tanque));
    }

    IEnumerator RespawnTanque(VidaTanque tanque)
    {
        yield return new WaitForSeconds(tiempoRespawn);
        Vector3 spawn = puntosSpawn[Random.Range(0, puntosSpawn.Length)].position;
        tanque.Respawn(spawn);
    }

    public void AñadirMonedas(int cantidad)
    {
        monedas += cantidad;
        ActualizarUIMonedas();
    }

    public bool ComprarVida()
    {
        if (monedas >= 5 && vidasJugador < 5)
        {
            monedas -= 5;
            vidasJugador++;
            ActualizarUIMonedas();
            ActualizarHUDVidas();
            return true;
        }
        return false;
    }

    void ActualizarHUDVidas()
    {
        for (int i = 0; i < vidasUI.Length; i++)
        {
            vidasUI[i].enabled = i < vidasJugador;
        }
    }

    void ActualizarUIMonedas()
    {
        textoMonedas.text = monedas.ToString();
    }
}
