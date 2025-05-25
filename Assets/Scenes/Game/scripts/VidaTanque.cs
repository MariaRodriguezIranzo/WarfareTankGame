using UnityEngine;
using UnityEngine.UI;

public class VidaTanque : MonoBehaviour
{
    public float vidaMaxima = 300f;
    private float vidaActual;

    [Header("Barra de Vida")]
    public Slider barraDeVida;

    public System.Action OnTanqueMuerto;

    void Start()
    {
        vidaActual = vidaMaxima;
        ActualizarUI();
    }

    public void RecibirDaño(float daño)
    {
        Debug.Log($"💥 Tanque recibe {daño} de daño");
        vidaActual -= daño;
        vidaActual = Mathf.Clamp(vidaActual, 0, vidaMaxima);
        ActualizarUI();

        if (vidaActual <= 0)
        {
            Morir();
        }
    }

    public void Curar(float cantidad)
    {
        vidaActual += cantidad;
        vidaActual = Mathf.Clamp(vidaActual, 0, vidaMaxima);
        ActualizarUI();
    }

    void ActualizarUI()
    {
        if (barraDeVida != null)
        {
            barraDeVida.maxValue = vidaMaxima;
            barraDeVida.value = vidaActual;
        }
    }

    void Morir()
    {
        Debug.Log("☠️ Tanque ha muerto");
        OnTanqueMuerto?.Invoke();
        gameObject.SetActive(false);
    }

    public void Respawn(Vector3 posicion)
    {
        Debug.Log("🔄 Respawn del tanque en: " + posicion);
        transform.position = posicion;
        vidaActual = vidaMaxima;
        ActualizarUI();
        gameObject.SetActive(true);
    }
}
