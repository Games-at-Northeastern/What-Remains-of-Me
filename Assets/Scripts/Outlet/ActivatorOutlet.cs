using UnityEngine;

public class ActivatorOutlet : AControllable
{
    [SerializeField]
    private float requiredVirusAmount;

    [SerializeField]
    private GameObject[] objectsToActivate;

    // Update is called once per frame
    private void Update()
    {
        if (GetVirus() >= requiredVirusAmount)
        {
            foreach (var obj in objectsToActivate)
            {
                obj.SetActive(true);
            }
        }
    }
}
