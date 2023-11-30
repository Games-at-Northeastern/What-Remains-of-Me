using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Animates a warning for when a player is about to die. Currently, causes warning image to blink
/// </summary>
public class WarningAnimation : MonoBehaviour
{
    private bool _runAnimation;
    [SerializeField] private GameObject warning;
    [SerializeField] private GameObject lightA;
    [SerializeField] private GameObject lightB;
    private float time;
    private Animator anim;
    private Color colorA;
    private Color colorB;
    private Color originalColorA;
    private Color originalColorB;
    private Color spriteColor;

    /// <summary>
    /// Initializes animation to not run, and initializes time
    /// </summary>
    void Start()
    {
        _runAnimation = false;
        time = -1.0f;
        anim = warning.GetComponent<Animator>();
        colorA = lightA.GetComponent<UnityEngine.Rendering.Universal.Light2D>().color;
        colorB = lightB.GetComponent<UnityEngine.Rendering.Universal.Light2D>().color;
        originalColorA = colorA;
        originalColorB = colorB;
        spriteColor = warning.GetComponent<SpriteRenderer>().color;
        warning.GetComponent<SpriteRenderer>().color = new Color(spriteColor.r, spriteColor.g, spriteColor.b, 0.0f);

    }

    /// <summary>
    /// If the animation should running, updates animation so that the warning turns on and off every second 
    /// </summary>
    void Update()
    {
        if (anim == null && _runAnimation)
        {
            /*
            Debug.Log("lerping");
            colorA = Color.Lerp(originalColorA, new Color(176f, 84f, 204f, 255f), 0.0001f);
            colorB = Color.Lerp(originalColorB, new Color(176f, 35f, 204f, 255f), 0.0001f);
            lightA.GetComponent<UnityEngine.Rendering.Universal.Light2D>().color = colorA;
            lightB.GetComponent<UnityEngine.Rendering.Universal.Light2D>().color = colorB;
            */
            
            if (time <= 0.1)   // Since time is set to a negative number in Start and StopAnimation, 
                                // this line is always true on first update when _runAnimation is true
            {
                //warning.GetComponent<UnityEngine.UI.Image>().enabled = !warning.GetComponent<UnityEngine.UI.Image>().isActiveAndEnabled;
                // warning.GetComponent<SpriteRenderer>().enabled = !warning.GetComponent<SpriteRenderer>().enabled;
                lightA.SetActive(!lightA.activeSelf);
                lightB.SetActive(!lightB.activeSelf);
                time = 0.1f;
            }
            else if (time >= 0.3)
            {
                time = -1.0f;
            }
            time += Time.deltaTime;
            

        }
    }

    /// <summary>
    /// Starts the animation
    /// </summary>
    public void StartAnimation()
    {
        _runAnimation = true;
        if (anim != null)
        {
            anim.enabled = true;
            //warning.GetComponent<SpriteRenderer>().enabled = true;
            lightA.SetActive(false);
            lightB.SetActive(false);
        }
    }

    /// <summary>
    /// Ends the animation. Turns off the warning image and resets the time
    /// </summary>
    public void StopAnimation()
    {
        _runAnimation = false;
      //  warning.GetComponent<SpriteRenderer>().enabled = false;
        time = -1.0f;
        if (anim != null)
        {
            anim.enabled = false;
            //warning.GetComponent<SpriteRenderer>().enabled = false;
            lightA.SetActive(true);
            lightB.SetActive(true);
        }
    }

    /// <summary>
    /// Changes Atlas' eye and headlamp color based on virus percentage
    /// </summary>
    public void VirusControl(float percentVirus)
    {
       // Debug.Log(percentVirus);
        lightA.GetComponent<UnityEngine.Rendering.Universal.Light2D>().color =
            new Color(originalColorA.r + (percentVirus * ((176f / 255f) - originalColorA.r)),
            originalColorA.g + (percentVirus * ((84f / 255f) - originalColorA.g)),
            originalColorA.b + (percentVirus * ((204f / 255f) - originalColorA.b)),
            originalColorA.a);
        lightB.GetComponent<UnityEngine.Rendering.Universal.Light2D>().color = new Color(originalColorB.r + (percentVirus * ((176f / 255f) - originalColorB.r)),
            originalColorB.g + (percentVirus * ((35f / 255f) - originalColorB.g)),
            originalColorB.b + (percentVirus * ((204f / 255f) - originalColorB.b)),
            originalColorB.a);
        warning.GetComponent<SpriteRenderer>().color = new Color(spriteColor.r, spriteColor.g, spriteColor.b, percentVirus);

    }


    public void ResetVirus()
    {
        lightA.GetComponent<UnityEngine.Rendering.Universal.Light2D>().color = originalColorA;
        lightB.GetComponent<UnityEngine.Rendering.Universal.Light2D>().color = originalColorB;
        lightA.SetActive(true);
        lightB.SetActive(true);
        warning.GetComponent<SpriteRenderer>().color = new Color(spriteColor.r, spriteColor.g, spriteColor.b, 0.0f);
    }

    public void RegularVirus()
    {

    }
}
