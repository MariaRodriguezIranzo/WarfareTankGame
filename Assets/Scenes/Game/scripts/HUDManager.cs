using UnityEngine;
using TMPro;

public class HUDManager : MonoBehaviour
{
    public TextMeshProUGUI textoVidas;
    public TextMeshProUGUI textoMonedas;

    private GameManager gameManager;

    void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
    }

    void Update()
    {
        if (gameManager == null)
            gameManager = FindObjectOfType<GameManager>();

        if (gameManager == null) return;

        if (textoVidas != null)
            textoVidas.text = gameManager.vidasJugador.ToString();

        if (textoMonedas != null)
            textoMonedas.text = gameManager.monedas.ToString();
    }
}
