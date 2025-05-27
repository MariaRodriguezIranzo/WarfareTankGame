using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private const int MAX_VIDAS = 5;

    public VidaTanque[] tanques;
    public Transform[] puntosSpawn;

    public int vidasJugador = 3;
    public int monedas = 0;
    public float tiempoRespawn = 3f;

    [Header("UI")]
    public TextMeshProUGUI textoVidas;         
    public TextMeshProUGUI textoMonedas;

    [Header("UI Revivir")]
    public GameObject reviviendoUI;            

    private bool tanqueRegistrado = false;

    void Start()
    {
        StartCoroutine(EsperarYRegistrarTanque());
        ActualizarHUDVidas();
        ActualizarUIMonedas();
    }

    IEnumerator EsperarYRegistrarTanque()
    {
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
        // Mostrar panel de "Reviviendo..."
        if (reviviendoUI != null)
            reviviendoUI.SetActive(true);

        yield return new WaitForSeconds(tiempoRespawn);

        Vector3 spawn = puntosSpawn[Random.Range(0, puntosSpawn.Length)].position;
        tanque.Respawn(spawn);

        // Ocultar panel de "Reviviendo..."
        if (reviviendoUI != null)
            reviviendoUI.SetActive(false);
    }

    public void AñadirMonedas(int cantidad)
    {
        monedas += cantidad;
        ActualizarUIMonedas();
    }

    public bool ComprarVida()
    {
        if (monedas >= 5 && vidasJugador < MAX_VIDAS)
        {
            monedas -= 5;
            vidasJugador = Mathf.Clamp(vidasJugador + 1, 0, MAX_VIDAS);
            ActualizarUIMonedas();
            ActualizarHUDVidas();
            return true;
        }
        return false;
    }

    void ActualizarHUDVidas()
    {
        textoVidas.text = vidasJugador.ToString();
    }

    void ActualizarUIMonedas()
    {
        textoMonedas.text = monedas.ToString();
    }
}
