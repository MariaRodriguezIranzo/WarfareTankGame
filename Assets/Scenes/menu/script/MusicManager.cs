using UnityEngine;

public class MusicManager : MonoBehaviour
{
    public static MusicManager instance; // Instancia est�tica para acceder desde cualquier parte
    public AudioSource audioSource;
    public AudioClip mainTrack; // La canci�n principal
    public AudioClip gameTrack; // La canci�n para el modo de juego o la otra escena

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
        audioSource.loop = true; // Aseg�rate de que la m�sica se repita
        audioSource.Play();
    }

    // M�todo para cambiar la m�sica a la de la escena del juego
    public void SwitchToGameTrack()
    {
        audioSource.clip = gameTrack;
        audioSource.Play();
    }
}