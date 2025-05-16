using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StoryScene : MonoBehaviour
{

    // Funci�n para iniciar la historia (escena de historia)
    public void OnPlayButtonClick()
    {
        // Cambiar a la escena de historia
        SceneManager.LoadScene("Game");
    }

    public void OnBackButtonClick()
    {
        // Cambiar a la escena de cr�ditos
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
