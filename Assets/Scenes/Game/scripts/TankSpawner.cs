using UnityEngine;

public class TankSpawner : MonoBehaviour
{
    [SerializeField] private GameObject[] tanks;
    [SerializeField] private GameObject TankSelected;
    [SerializeField] private GameObject SpawnPoint;

    private void Start()
    {
        foreach (GameObject car in tanks)
        {
            car.SetActive(false);
        }
        SpawnSelectedTank();
    }

    private void SpawnSelectedTank()
    {
        int tankToSpawn = PlayerPrefs.GetInt("SelectedTank");
        tanks[tankToSpawn].SetActive(true);
        TankSelected = Instantiate(tanks[tankToSpawn], TankSelected.transform.position, TankSelected.transform.rotation);
        TankSelected.transform.SetParent(SpawnPoint.transform);

    }
}
