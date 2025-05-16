using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

public class TankSelection : MonoBehaviour
{
    [SerializeField] private GameObject[] tanks;
    [SerializeField] private GameObject TankSelected;
    [SerializeField] private GameObject Platform;

    // Añade aquí los tipos de scripts que quieres desactivar
    private Type[] scriptsToDisable = new Type[]
    {
        typeof(WeaponShooting)
    };

    private int _currentTankIndex;

    private void Awake()
    {
        if (tanks.Length == 0)
        {
            Debug.LogError("No tank assigned in the inspector.");
            return;
        }

        foreach (GameObject tank in tanks)
        {
            tank.SetActive(false);
        }
    }

    private void Start()
    {
        _currentTankIndex = PlayerPrefs.GetInt("SelectedTank");

        if (_currentTankIndex >= 0 && _currentTankIndex < tanks.Length)
        {
            tanks[_currentTankIndex].SetActive(true);

            // Instanciar tanque
            TankSelected = Instantiate(tanks[_currentTankIndex], TankSelected.transform.position, TankSelected.transform.rotation);
            TankSelected.transform.SetParent(Platform.transform);

            // 🚫 Desactivar scripts solo en la instancia del tanque
            foreach (var type in scriptsToDisable)
            {
                Component script = TankSelected.GetComponentInChildren(type);
                if (script != null)
                {
                    ((MonoBehaviour)script).enabled = false;
                }
            }
        }
        else
        {
            Debug.LogError("Selected tank index is out of range.");
        }
    }

    private void FixedUpdate()
    {
        Platform.transform.Rotate(Vector3.up, 10 * Time.deltaTime);
    }

    public void NextTank()
    {
        if (TankSelected != null)
            Destroy(TankSelected);

        tanks[_currentTankIndex].SetActive(false);
        _currentTankIndex = (_currentTankIndex + 1) % tanks.Length;
        UpdateTankSelection();
    }

    public void PreviousTank()
    {
        if (TankSelected != null)
            Destroy(TankSelected);

        tanks[_currentTankIndex].SetActive(false);
        _currentTankIndex = (_currentTankIndex - 1 + tanks.Length) % tanks.Length;
        UpdateTankSelection();
    }

    private void UpdateTankSelection()
    {
        tanks[_currentTankIndex].SetActive(true);
        TankSelected = Instantiate(tanks[_currentTankIndex], TankSelected.transform.position, TankSelected.transform.rotation);
        TankSelected.transform.SetParent(Platform.transform);

        // 🚫 Desactivar scripts de nuevo en cada cambio de tanque
        foreach (var type in scriptsToDisable)
        {
            Component script = TankSelected.GetComponentInChildren(type);
            if (script != null)
            {
                ((MonoBehaviour)script).enabled = false;
            }
        }

        PlayerPrefs.SetInt("SelectedTank", _currentTankIndex);
        PlayerPrefs.Save();
    }

    public void PlayGame()
    {
        SceneManager.LoadScene("Game"); // Cambia el nombre según tu escena real
    }

    public void BackToMainMenu()
    {
        SceneManager.LoadScene("menu"); // Cambia el nombre según tu escena real
    }
}
