using UnityEngine;
using System.Collections;

public class VidaTanque : MonoBehaviour
{
    public int vidaMaxima = 100;
    public int vidaActual;

    private GameManager gameManager;

    void Start()
    {
        vidaActual = vidaMaxima;
        gameManager = FindObjectOfType<GameManager>();
    }

    public void RecibirDanio(int danio)
    {
        vidaActual -= danio;
        if (vidaActual <= 0)
        {
            vidaActual = 0;
            Morir();
        }
    }

    void Morir()
    {
        // Desactiva el tanque temporalmente
        gameObject.SetActive(false);

        // Notifica al GameManager que murió este tanque
        if (gameManager != null)
            gameManager.TanqueMuerto(this);
    }

    public void Respawnear(Vector3 posicion)
    {
        transform.position = posicion;
        vidaActual = vidaMaxima;
        gameObject.SetActive(true);
    }
}
