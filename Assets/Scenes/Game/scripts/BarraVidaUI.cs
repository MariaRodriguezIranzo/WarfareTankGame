using UnityEngine;
using UnityEngine.UI;

public class BarraVidaUI : MonoBehaviour
{
    public VidaTanque tanque;         // Referencia al tanque
    public Slider sliderVida;         // Slider UI para la barra
    public Vector3 offset = new Vector3(0, 2.5f, 0); // Altura sobre el tanque

    Camera cam;

    void Start()
    {
        cam = Camera.main;

        if (tanque != null)
        {
            sliderVida.maxValue = tanque.vidaMaxima;
            sliderVida.value = tanque.vidaActual;
        }
    }

    void Update()
    {
        if (tanque == null)
        {
            Destroy(gameObject);
            return;
        }

        // Seguir al tanque en pantalla
        Vector3 worldPos = tanque.transform.position + offset;
        transform.position = cam.WorldToScreenPoint(worldPos);

        // Actualizar la barra
        sliderVida.value = tanque.vidaActual;
    }
}
