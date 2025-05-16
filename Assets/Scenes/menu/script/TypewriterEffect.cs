using System.Collections;
using TMPro;
using UnityEngine;

public class TypewriterEffect : MonoBehaviour
{
    public TextMeshProUGUI textDisplay; // Asigna el texto en el Canvas
    public float typingSpeed = 0.05f;   // Velocidad de escritura

    [TextArea(3, 10)]
    public string fullText; // Puedes escribir el texto desde el Inspector

    private string currentText = "";

    void Start()
    {
        StartCoroutine(ShowText());
    }

    IEnumerator ShowText()
    {
        for (int i = 0; i < fullText.Length; i++)
        {
            currentText = fullText.Substring(0, i + 1);
            textDisplay.text = currentText;
            yield return new WaitForSeconds(typingSpeed);
        }
    }
}
