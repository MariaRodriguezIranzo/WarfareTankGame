using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuController : MonoBehaviour
{
    // Funci�n para iniciar la historia (escena de historia)
    public void OnPlayButtonClick()
    {
        
        SceneManager.LoadScene("SelectTank");
    }

    // Funci�n para ir a los cr�ditos
   public void  OnCreditsButtonClick()
    {
        // Cambiar a la escena de cr�ditos
        SceneManager.LoadScene("CreditsScene");
    }

    public void OnOptionssButtonClick()
    {
        // Cambiar a la escena de cr�ditos
        SceneManager.LoadScene("OptiosScene");
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
