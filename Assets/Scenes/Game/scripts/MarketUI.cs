using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MarketUI : MonoBehaviour
{
    public TextMeshProUGUI textoMonedasMarket;
    public TextMeshProUGUI textoVidasMarket;
    public TextMeshProUGUI textoPrecioVida;
    public Button botonComprarVida;

    public GameObject marketCanvas;

    private GameManager gameManager;
    private bool mercadoAbierto = false;

    void Start()
    {
        gameManager = FindObjectOfType<GameManager>();

        if (botonComprarVida != null)
            botonComprarVida.onClick.AddListener(ComprarVida);

        if (marketCanvas != null)
            marketCanvas.SetActive(false);

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.M))
        {
            if (mercadoAbierto)
                CerrarMercado();
            else
                AbrirMercado();
        }

        ActualizarUI();
    }

    void AbrirMercado()
    {
        Debug.Log("Abriendo mercado");
        marketCanvas.SetActive(true);
        mercadoAbierto = true;

        GameManager.inputBloqueado = true;
        Time.timeScale = 0f;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    void CerrarMercado()
    {
        Debug.Log("Cerrando mercado");
        marketCanvas.SetActive(false);
        mercadoAbierto = false;

        GameManager.inputBloqueado = false;
        Time.timeScale = 1f;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
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
            textoVidasMarket.text = $" {gameManager.vidasJugador} / 5";

        if (textoPrecioVida != null)
            textoPrecioVida.text = $"PRICE LIVES: {gameManager.precioVida} COINS";
    }
}
