using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class TankSelection : MonoBehaviour
{
    [Header("Tanques")]
    [SerializeField] private GameObject[] tanks;
    [SerializeField] private GameObject TankSelected;
    [SerializeField] private GameObject Platform;

    [Header("UI de Stats")]
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI speedText;
    [SerializeField] private TextMeshProUGUI armorText;
    [SerializeField] private TextMeshProUGUI damageText;
    [SerializeField] private TextMeshProUGUI descriptionText;

    [Header("Estadísticas de Tanques")]
    [SerializeField] private TankStats[] tankStats;

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
        _currentTankIndex = PlayerPrefs.GetInt("SelectedTank", 0);

        if (_currentTankIndex >= 0 && _currentTankIndex < tanks.Length)
        {
            tanks[_currentTankIndex].SetActive(true);

            TankSelected = Instantiate(tanks[_currentTankIndex], TankSelected.transform.position, TankSelected.transform.rotation);
            TankSelected.transform.SetParent(Platform.transform);

            DisableTankScripts(TankSelected);
            UpdateTankStatsUI();
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

        DisableTankScripts(TankSelected);
        UpdateTankStatsUI();

        PlayerPrefs.SetInt("SelectedTank", _currentTankIndex);
        PlayerPrefs.Save();
    }

    private void DisableTankScripts(GameObject tank)
    {
        foreach (var type in scriptsToDisable)
        {
            Component script = tank.GetComponentInChildren(type);
            if (script != null)
                ((MonoBehaviour)script).enabled = false;
        }
    }

    private void UpdateTankStatsUI()
    {
        if (_currentTankIndex < tankStats.Length)
        {
            TankStats stats = tankStats[_currentTankIndex];
            nameText.text = stats.tankName;
            speedText.text = $"Speed: {stats.speed}";
            armorText.text = $"HP: {stats.armor}";
            damageText.text = $"Damage: {stats.damage}";
            descriptionText.text = stats.description;
        }
        else
        {
            Debug.LogWarning("No stats assigned for this tank index.");
        }
    }

    public void PlayGame()
    {
        SceneManager.LoadScene("Game");
    }

    public void BackToMainMenu()
    {
        SceneManager.LoadScene("menu");
    }
}

[System.Serializable]
public class TankStats
{
    public string tankName;
    public float speed;
    public float armor;
    public float damage;
    public string description;
}
