using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class ControllerPlayer : MonoBehaviour
{
    public int health = 5;
    public GameObject[] hearts; // Asignar los corazones en inspector
    private Animator animator;
    public AudioClip damageSound;
    public AudioClip deathSound;
    private AudioSource audioSource;

    void Start()
    {
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
        ActualizarHeartsUI();
    }

    private void Die()
    {
        Debug.Log("Player ha muerto.");

        if (audioSource != null && deathSound != null)
        {
            audioSource.PlayOneShot(deathSound);
        }

        StartCoroutine(PlayDeathAnimation());
    }

    void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("NPC"))
        {
            StartCoroutine(RecibirDaño());
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

    public IEnumerator RecibirDaño()
    {
        if (health > 1)
        {
            health--;
            Debug.Log($"Player recibió daño. Vida restante: {health}");

            if (audioSource != null && damageSound != null)
            {
                audioSource.PlayOneShot(damageSound);
            }

            ActualizarHeartsUI();
        }
        else
        {
            health--;
            ActualizarHeartsUI();
            animator.SetBool("Die", true);
            yield return new WaitForSeconds(2f);
            SceneManager.LoadScene("GameOver");
        }

        yield return null;
    }

    void ActualizarHeartsUI()
    {
        for (int i = 0; i < hearts.Length; i++)
        {
            hearts[i].SetActive(i < health);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Heal"))
        {
            if (health < hearts.Length)
            {
                health++;
                ActualizarHeartsUI();
                Destroy(other.gameObject);
            }
        }
    }
}
