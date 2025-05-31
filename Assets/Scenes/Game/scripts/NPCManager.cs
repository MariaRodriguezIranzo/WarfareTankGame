using UnityEngine;
using UnityEngine.SceneManagement;

public class NPCManager : MonoBehaviour
{
    public static NPCManager instance;

    private int npcCount;
    private bool bossInvocado = false;

    [Header("Boss")]
    public GameObject bossGO; // Este es el GameObject ya en escena, desactivado desde el editor

    void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }

    void Start()
    {
        GameObject[] npcs = GameObject.FindGameObjectsWithTag("NPC");
        npcCount = npcs.Length;
        Debug.Log("NPCs totales al inicio: " + npcCount);
    }

    public void NPCMuerto()
    {
        npcCount--;
        Debug.Log("NPCs restantes: " + npcCount);

        if (npcCount <= 0 && !bossInvocado)
        {
            ActivarBoss();
        }
    }

    void ActivarBoss()
    {
        if (bossGO != null)
        {
            bossGO.SetActive(true);
            bossInvocado = true;
            Debug.Log("👹 Boss activado. ¡Prepárate para la batalla final!");
        }
        else
        {
            Debug.LogWarning("No se asignó el GameObject del boss en el inspector.");
        }
    }

    public void BossMuerto()
    {
        Debug.Log("✅ Boss derrotado. ¡Ganaste!");
        SceneManager.LoadScene("WIN");
    }
}
