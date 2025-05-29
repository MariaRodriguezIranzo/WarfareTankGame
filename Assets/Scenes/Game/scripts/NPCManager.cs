using UnityEngine;
using UnityEngine.SceneManagement;

public class NPCManager : MonoBehaviour
{
    public static NPCManager instance;

    private int npcCount;

    void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }

    void Start()
    {
        // Contar todos los NPCs con tag "NPC" al iniciar la escena
        GameObject[] npcs = GameObject.FindGameObjectsWithTag("NPC");
        npcCount = npcs.Length;
        Debug.Log("NPCs totales al inicio: " + npcCount);
    }

    public void NPCMuerto()
    {
        npcCount--;
        Debug.Log("NPCs restantes: " + npcCount);

        if (npcCount <= 0)
        {
            Debug.Log("Todos los NPCs muertos, Game Over");
            SceneManager.LoadScene("WIN");
        }
    }
}
