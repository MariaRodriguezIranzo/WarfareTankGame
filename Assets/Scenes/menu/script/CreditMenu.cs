using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CreditMenu : MonoBehaviour
{
    void Start()
    {
        // Activar y desbloquear el cursor
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    // Funci�n para ir al men� principal
    public void OnBackButtonClick()
    {
        SceneManager.LoadScene("menu");
    }

    // Funci�n para salir del juego
    public void OnExitButtonClick()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
