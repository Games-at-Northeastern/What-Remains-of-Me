using UnityEngine;

public class SpawnInspectionModule : MonoBehaviour
{

    [SerializeField] private GameObject SpawnPoint;

    public void Spawn()
    {
        enabled = true;
        transform.position = SpawnPoint.transform.position;
    }


}
