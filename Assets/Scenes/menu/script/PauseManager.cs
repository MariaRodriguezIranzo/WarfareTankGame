using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseManager : MonoBehaviour
{
    public GameObject pauseMenuUI;
    private bool isPaused = false;

    void Start()
    {
        pauseMenuUI.SetActive(false);
        GameManager.inputBloqueado = false;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
                Resume();
            else
                Pause();
        }
    }

    public void Resume()
    {
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1f;
        isPaused = false;
        Cursor.lockState = CursorLockMode.Locked; // Oculta cursor si FPS
        Cursor.visible = false;

        GameManager.inputBloqueado = false; // Desbloquea input global
    }

    public void Pause()
    {
        pauseMenuUI.SetActive(true);
        Time.timeScale = 0f;
        isPaused = true;
        Cursor.lockState = CursorLockMode.None; // Muestra cursor
        Cursor.visible = true;

        GameManager.inputBloqueado = true; // Bloquea input global
    }

    public void LoadMenu()
    {
        Time.timeScale = 1f;
        GameManager.inputBloqueado = false; // Asegura desbloqueo al cargar menú
        SceneManager.LoadScene("menu");
    }

    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
