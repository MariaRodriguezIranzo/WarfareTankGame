using UnityEngine;
using UnityEngine.Events;

public class VidaDual : MonoBehaviour
{
    [Header("Vida Tanque")]
    public int vidaMaxima;       // Ej: 300 o 160 según prefab
    private int vidaActual;

    [Header("Vidas Extra")]
    public int vidasExtra = 3;    // Cuántas "vidas grandes" tiene
    public int vidasMaximas = 5;  // Tope máximo que puede tener

    [Header("Eventos")]
    public UnityEvent OnVidaCambiada;
    public UnityEvent OnVidaExtraCambiada;
    public UnityEvent OnTanqueMuerto;
    public UnityEvent OnJuegoTerminado;

    void Start()
    {
        vidaActual = vidaMaxima;
    }

    // Llamar cuando el tanque recibe daño
    public void RecibirDaño(int daño)
    {
        vidaActual -= daño;
        vidaActual = Mathf.Clamp(vidaActual, 0, vidaMaxima);

        OnVidaCambiada?.Invoke();

        if (vidaActual <= 0)
        {
            PerderVidaExtra();
        }
    }

    void PerderVidaExtra()
    {
        vidasExtra--;
        vidasExtra = Mathf.Clamp(vidasExtra, 0, vidasMaximas);

        OnVidaExtraCambiada?.Invoke();

        if (vidasExtra > 0)
        {
            // Reset vida del tanque para la nueva vida
            vidaActual = vidaMaxima;
            Debug.Log($"Vida extra perdida, vidas restantes: {vidasExtra}");
        }
        else
        {
            // Se acabaron las vidas grandes, tanque muerto
            Debug.Log("Tanque muerto");
            OnTanqueMuerto?.Invoke();
            // Aquí puedes hacer algo tipo destruir o game over
            gameObject.SetActive(false);
            OnJuegoTerminado?.Invoke();
        }
    }

    // Para curar el tanque, pero solo vida interna
    public void Curar(int cantidad)
    {
        vidaActual += cantidad;
        vidaActual = Mathf.Clamp(vidaActual, 0, vidaMaxima);
        OnVidaCambiada?.Invoke();
    }

    // Para comprar o conseguir vida extra (hasta máximo)
    public void AñadirVidaExtra()
    {
        if (vidasExtra < vidasMaximas)
        {
            vidasExtra++;
            OnVidaExtraCambiada?.Invoke();
        }
    }

    // Getters para UI o lógica
    public int GetVidaActual() => vidaActual;
    public int GetVidaMaxima() => vidaMaxima;
    public int GetVidasExtra() => vidasExtra;
    public int GetVidasMaximas() => vidasMaximas;
}
