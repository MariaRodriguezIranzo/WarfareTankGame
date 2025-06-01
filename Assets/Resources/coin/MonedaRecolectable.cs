using UnityEngine;

public class MonedaRecolectable : MonoBehaviour
{
    [Header("Valor de la moneda")]
    public int valorMoneda = 50;

    [Header("Animación")]
    public float rotacionVelocidad = 90f;
    public float flotacionAltura = 0.25f;
    public float flotacionVelocidad = 2f;

    [Header("Sonido")]
    public AudioClip sonidoRecolectar;

    private float posicionYInicial;
    private AudioSource audioSource;
    private bool recolectada = false;

    void Start()
    {
        posicionYInicial = transform.position.y;
        audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        if (recolectada) return;

        // Rotación + flotación
        transform.Rotate(Vector3.up * rotacionVelocidad * Time.deltaTime, Space.World);
        float nuevaY = posicionYInicial + Mathf.Sin(Time.time * flotacionVelocidad) * flotacionAltura;
        transform.position = new Vector3(transform.position.x, nuevaY, transform.position.z);
    }

    void OnTriggerEnter(Collider other)
    {
        if (recolectada) return;

        if (other.CompareTag("Player"))
        {
            GameManager gm = FindObjectOfType<GameManager>();
            if (gm != null)
            {
                gm.AñadirMonedas(valorMoneda);
            }

            recolectada = true;

            // Reproducir sonido
            if (sonidoRecolectar != null && audioSource != null)
            {
                audioSource.PlayOneShot(sonidoRecolectar);
            }

            // Desactivar visual y colisión mientras suena el audio
            GetComponent<Collider>().enabled = false;
            foreach (Transform child in transform)
            {
                child.gameObject.SetActive(false); // Oculta modelo visual si tiene hijos
            }

            Destroy(gameObject, sonidoRecolectar != null ? sonidoRecolectar.length : 0.1f);
        }
    }
}
