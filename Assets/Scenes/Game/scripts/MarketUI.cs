using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MarketUI : MonoBehaviour
{
    public TextMeshProUGUI textoMonedasMarket;
    public TextMeshProUGUI textoVidasMarket;
    public TextMeshProUGUI textoPrecioVida;
    public Button botonComprarVida;

    private GameManager gameManager;

    void Start()
    {
        // Buscar el GameManager en la escena si no hay instancia
        gameManager = FindObjectOfType<GameManager>();

        if (botonComprarVida != null)
            botonComprarVida.onClick.AddListener(ComprarVida);
    }

    void Update()
    {
        // Volver a buscar por si se cambió de escena y se volvió a instanciar
        if (gameManager == null)
            gameManager = FindObjectOfType<GameManager>();

        ActualizarUI();
    }

    void ComprarVida()
    {
        if (gameManager != null)
            gameManager.ComprarVida();
    }

    void ActualizarUI()
    {
        if (gameManager == null) return;

        if (textoMonedasMarket != null)
            textoMonedasMarket.text = gameManager.monedas.ToString();

        if (textoVidasMarket != null)
            textoVidasMarket.text = gameManager.vidasJugador.ToString();

        if (textoPrecioVida != null)
            textoPrecioVida.text = $"PRICE LIVES: {gameManager.precioVida} COINS";
    }
}
