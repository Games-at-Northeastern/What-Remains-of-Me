using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class VoxLightControl : MonoBehaviour
{
    [SerializeField] GameObject hologramLight;
    [SerializeField] Animator animator;
    private bool isRed = false;

    // Update is called once per frame
    void Update()
    {
        if (animator.GetBool("isRed") != isRed) {
            isRed = animator.GetBool("isRed");
            if (isRed) {
                hologramLight.GetComponent<Light2D>().color = new Color32(255, 18, 0, 116);
            } else {
                hologramLight.GetComponent<Light2D>().color = new Color32(0, 186, 255, 116);
            }
        }
    }
}
