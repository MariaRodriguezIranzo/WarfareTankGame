using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance; // Singleton

    private const int MAX_VIDAS = 5;

    public VidaTanque[] tanques;
    public Transform[] puntosSpawn;

    public int vidasJugador = 3;
    public int monedas = 0;
    public int precioVida = 5; // Precio para comprar vida, editable en inspector
    public float tiempoRespawn = 3f;

    [Header("UI")]
    public TextMeshProUGUI textoVidas;
    public TextMeshProUGUI textoMonedas;

    [Header("UI Revivir")]
    public GameObject reviviendoUI;

    private bool tanqueRegistrado = false;

    public bool mercadoAbierto = false;

    void Awake()
    {
        // Singleton pattern
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

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
        if (reviviendoUI != null)
            reviviendoUI.SetActive(true);

        yield return new WaitForSeconds(tiempoRespawn);

        Vector3 spawn = puntosSpawn[Random.Range(0, puntosSpawn.Length)].position;
        tanque.Respawn(spawn);

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
        if (monedas >= precioVida && vidasJugador < MAX_VIDAS)
        {
            monedas -= precioVida;
            vidasJugador = Mathf.Clamp(vidasJugador + 1, 0, MAX_VIDAS);
            ActualizarUIMonedas();
            ActualizarHUDVidas();
            return true;
        }
        return false;
    }

    void ActualizarHUDVidas()
    {
        if (textoVidas != null)
            textoVidas.text = vidasJugador.ToString();
    }

    void ActualizarUIMonedas()
    {
        if (textoMonedas != null)
            textoMonedas.text = monedas.ToString();
    }
}
