using UnityEngine;

public class MarketChest : MonoBehaviour
{
    public GameObject marketUI;
    public GameObject textoInteractuarUI;
    public float distanciaMinima = 2.5f;

    private GameObject jugador;
    private bool mercadoAbierto = false;

    [Header("Sound Effects")]
    public AudioClip sonidoAbrir;
    public AudioClip sonidoCerrar;
    private AudioSource audioSource;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
    }

    void Update()
    {
        if (jugador == null) return;

        float distancia = Vector3.Distance(transform.position, jugador.transform.position);

        if (distancia <= distanciaMinima && !mercadoAbierto)
        {
            if (textoInteractuarUI != null)
                textoInteractuarUI.SetActive(true);

            if (Input.GetKeyDown(KeyCode.M))
            {
                mercadoAbierto = true;
                marketUI?.SetActive(true);
                textoInteractuarUI?.SetActive(false);
                audioSource.PlayOneShot(sonidoAbrir);
            }
        }
        else
        {
            textoInteractuarUI?.SetActive(false);

            if (mercadoAbierto && Input.GetKeyDown(KeyCode.M))
            {
                mercadoAbierto = false;
                marketUI?.SetActive(false);
                audioSource.PlayOneShot(sonidoCerrar);
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            jugador = other.gameObject;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            textoInteractuarUI?.SetActive(false);
            marketUI?.SetActive(false);
            mercadoAbierto = false;
            jugador = null;
        }
    }
}
