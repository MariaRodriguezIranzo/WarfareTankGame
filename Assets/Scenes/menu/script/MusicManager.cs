using UnityEngine;

public class MusicManager : MonoBehaviour
{
    public static MusicManager instance; // Instancia estática para acceder desde cualquier parte
    public AudioSource audioSource;
    public AudioClip mainTrack; // La canción principal
    public AudioClip gameTrack; // La canción para el modo de juego o la otra escena

    private void Awake()
    {
        if (instance == null)
        {
            instance = this; // Asigna la instancia si no existe
            DontDestroyOnLoad(gameObject); // Evita que el objeto sea destruido al cambiar de escena
        }
        else
        {
            Destroy(gameObject); // Si ya existe, destruye el objeto duplicado
        }
    }

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.clip = mainTrack;
        audioSource.loop = true; // Asegúrate de que la música se repita
        audioSource.Play();
    }

    // Método para cambiar la música a la de la escena del juego
    public void SwitchToGameTrack()
    {
        audioSource.clip = gameTrack;
        audioSource.Play();
    }
}