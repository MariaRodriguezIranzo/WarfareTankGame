using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MarketUI : MonoBehaviour
{
    public TextMeshProUGUI textoMonedasMarket;
    public TextMeshProUGUI textoVidasMarket;
    public TextMeshProUGUI textoPrecioVida;    // NUEVO: Texto que muestra el precio de la vida
    public Button botonComprarVida;

    private GameManager gameManager;

    void Start()
    {
        gameManager = GameManager.instance;

        if (botonComprarVida != null)
            botonComprarVida.onClick.AddListener(ComprarVida);
    }

    void Update()
    {
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
            textoPrecioVida.text = $"Precio vida: {gameManager.precioVida} monedas"; // Mostrar precio dinámico
    }
}
