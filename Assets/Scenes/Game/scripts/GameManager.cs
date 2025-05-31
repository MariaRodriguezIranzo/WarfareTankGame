using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private const int MAX_VIDAS = 5;

    public VidaTanque[] tanques;
    public Transform[] puntosSpawn;

    public int vidasJugador = 3;
    public int monedas = 0;
    public int precioVida = 5;
    public float tiempoRespawn = 3f;

    public bool mercadoAbierto = false;

    private bool tanqueRegistrado = false;

    void Start()
    {
        StartCoroutine(EsperarYRegistrarTanque());
    }

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        StartCoroutine(EsperarYRegistrarTanque());
    }

    IEnumerator EsperarYRegistrarTanque()
    {
        tanqueRegistrado = false;

        while (!tanqueRegistrado)
        {
            VidaTanque tanqueInstanciado = FindObjectOfType<VidaTanque>();
            if (tanqueInstanciado != null)
            {
                tanques = new VidaTanque[] { tanqueInstanciado };
                tanqueInstanciado.OnTanqueMuerto += () => TanqueMuerto(tanqueInstanciado);
                tanqueRegistrado = true;
                Debug.Log("[GameManager] Tanque registrado.");
            }
            yield return null;
        }
    }

    public void TanqueMuerto(VidaTanque tanque)
    {
        vidasJugador--;

        if (vidasJugador <= 0)
        {
            Debug.Log("[GameManager] Game Over");
            SceneManager.LoadScene("GameOver");
            return;
        }

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
    }

    public bool ComprarVida()
    {
        if (monedas >= precioVida && vidasJugador < MAX_VIDAS)
        {
            monedas -= precioVida;
            vidasJugador = Mathf.Clamp(vidasJugador + 1, 0, MAX_VIDAS);
            return true;
        }
        return false;
    }
}
