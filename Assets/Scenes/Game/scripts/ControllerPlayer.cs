using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine.UI;

public class ControllerPlayer : MonoBehaviour
{
    public int health = 5;
    
    public GameObject[] hearts;
    private Animator animator;
    public AudioClip damageSound;  // Sonido de da�o
    public AudioClip deathSound;   // Sonido de muerte
    private AudioSource audioSource;

    void Start()
    {
       
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>(); // Obt�n el componente AudioSource

    }

    private void Die()
    {
        Debug.Log("Player ha muerto.");

        // Reproduce sonido de muerte
        if (audioSource != null && deathSound != null)
        {
            audioSource.PlayOneShot(deathSound);  // Reproduce el sonido de muerte
        }

        StartCoroutine(PlayDeathAnimation());
    }
    void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("NPC"))
        {
            StartCoroutine(RecibirDa�o());
        }
    }
    private IEnumerator PlayDeathAnimation()
    {
        if (animator != null)
        {
            animator.SetBool("Die", true);
            yield return new WaitForSeconds(1.5f);
        }

    }

    public IEnumerator RecibirDa�o()
    {
        if (health > 1)
        {
            health -= 1;
            Debug.Log($"Player recibi� da�o. Vida restante: {health}");
        }
        else
        {
            health -= 1;
            for (int i = 5; i >= 0; i--)
            {
                hearts[i].SetActive(i == health);
            }
            animator.SetBool("Die", true);
            yield return new WaitForSeconds(2);
            SceneManager.LoadScene("GameOver");
        }

        // Actualizar HUD
        for (int i = 5; i >= 0; i--)
        {
            hearts[i].SetActive(i == health);
        }

        yield return null; // Sin esperar, para permitir que el jugador reciba da�o inmediatamente.
    }


    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Heal"))
        {
            if (health < 5)
            {
                health += 1;
                for (int i = 0; i < 6; i++)
                {

                    hearts[i].SetActive(i == health);
                } 
                Destroy(other.gameObject);
            }
           
        }
    }
}