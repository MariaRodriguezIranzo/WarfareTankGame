using UnityEngine;
using TMPro;

public class AmmoUI : MonoBehaviour
{
    public static AmmoUI instance;

    [SerializeField] private TextMeshProUGUI ammoText;

    private void Awake()
    {
        instance = this;
    }

    public void UpdateAmmoText(int current, int max)
    {
        if (ammoText != null)
        {
            ammoText.text = current + "/" + max;
        }
    }
}
