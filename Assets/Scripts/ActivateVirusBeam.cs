using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActivateVirusBeam : MonoBehaviour
{
    public GameObject virusBeam;
    [SerializeField] private float startDelay = 1f;
    [SerializeField] private float repeatRate = 2f;

    //might want to consider using coroutines instead of invoke

    private void Start()
    {
        // Invoke the function every 2 seconds, starting after 1 second
        InvokeRepeating("SetVirusBeamActive", 1f, 2f);
    }

    private void SetVirusBeamActive()
    {
        virusBeam.SetActive(!virusBeam.activeSelf);
    }

    public void StopVirusBeam()
    {
        CancelInvoke("SetVirusBeamActive");
    }
}
