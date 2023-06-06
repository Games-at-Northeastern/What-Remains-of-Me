using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

/// <summary>
/// This script handles the visuals of the energy level contained within an outlet.
///
/// To assign new a new sprite sheet to this visual, go to the inspector and select each sprite in the sliced spritesheet,
/// then drag the selection into the serialized array in the inspector.
/// </summary>
public class OutletMeter : MonoBehaviour
{
    
    /*[Range(0, 9)]
    public int Limiter = 0;

    [Range(0, 16)]
    public int Virus = 0;

    [Range(0, 16)]
    public int Clean = 0;*/

    [SerializeField] private Sprite[] limiterSprites;
    [SerializeField] private Sprite[] virusSprites;
    [SerializeField] private Sprite[] cleanSprites;

    [SerializeField] private SpriteRenderer limiterMeter;
    [SerializeField] private SpriteRenderer virusMeter;
    [SerializeField] private SpriteRenderer cleanMeter;

    [SerializeField] private float visualFillSpeed = 1;

    private float targetVirus;
    private float targetClean;

    private float currentVirus;
    private float currentClean;

    private float actualVirus;
    private float actualClean;

    private bool coroutineRunning = false;
    private bool powered = false;

    private int _limiterState = 0;

    private int LimiterState
    {
        get { return _limiterState; }
        set { _limiterState = Mathf.Clamp(value, 0, limiterSprites.Length - 1); }
    }

    private int _virusState = 0;
    private int VirusState
    {
        get { return _virusState; }
        set { _virusState = Mathf.Clamp(value, 0, virusSprites.Length - 1); }
    }

    private int _cleanState = 0;

    private int CleanState
    {
        get { return _cleanState; }
        set { _cleanState = Mathf.Clamp(value, 0, cleanSprites.Length - 1); }
    }

    public void UpdateValues(float virus, float clean, float max)
    {

        float limiterAmount = ((100 - max) / 100) * limiterSprites.Length - 1;
        LimiterState = Mathf.FloorToInt(limiterAmount);

        /*float cleanAmount = (clean / 100) * cleanSprites.Length - 1;
        if (cleanAmount is < 1 and > 0)
        {
            CleanState = 1;
        }
        else
        {
            CleanState = Mathf.FloorToInt(cleanAmount);
        }

        float virusAmount = (virus / 100) * virusSprites.Length - 1;
        if (virusAmount is < 1 and > 0)
        {
            VirusState = 1;
        }
        else
        {
            VirusState = Mathf.FloorToInt(virusAmount);
        }*/


        /*CleanState = Mathf.FloorToInt((clean / 12.5f) * 2);
        VirusState = Mathf.FloorToInt((virus / 12.5f) * 2);
        if (virus > 0 && VirusState == 0)
        {
            VirusState = 1;
        }
        if (clean > 0 && CleanState == 0)
        {
            CleanState = 1;
        }*/

        actualVirus = virus;
        actualClean = clean;

        limiterMeter.sprite = limiterSprites[_limiterState];
        /*virusMeter.sprite = virusSprites[_virusState];
        cleanMeter.sprite = cleanSprites[Mathf.Min(_cleanState + _virusState, cleanSprites.Length - 1)];*/
    }

    public void StartVisuals()
    {
        Debug.Log("startVisuals");
        targetVirus = actualVirus;
        targetClean = actualClean;
        if (coroutineRunning) { return; }
        StartCoroutine(UpdateVisuals());
    }

    public void EndVisuals()
    {
        Debug.Log("endVisuals");
        powered = false;
        targetVirus = 0;
        targetClean = 0;
    }

    private IEnumerator UpdateVisuals()
    {
        Debug.Log("Starting outletmeter coroutine");
        coroutineRunning = true;
        powered = true;
        while (true)
        {

            currentVirus = Mathf.Lerp(currentVirus, targetVirus, visualFillSpeed * Time.deltaTime);
            currentClean = Mathf.Lerp(currentClean, targetClean, visualFillSpeed * Time.deltaTime);

            VirusState = Mathf.FloorToInt((currentVirus / 12.5f) * 2);
            CleanState = Mathf.FloorToInt((currentClean / 12.5f) * 2);
            if (currentVirus > 0 && VirusState == 0)
            {
                VirusState = 1;
            }
            if (currentClean > 0 && CleanState == 0)
            {
                Debug.Log("TEST");
                CleanState = 1;
            }

            virusMeter.sprite = virusSprites[_virusState];
            cleanMeter.sprite = cleanSprites[Mathf.Min(_cleanState + _virusState, cleanSprites.Length - 1)];

            if (currentClean < 6f && currentVirus < 6f && !powered)
            {
                virusMeter.sprite = virusSprites[0];
                cleanMeter.sprite = cleanSprites[0];
                coroutineRunning = false;
                Debug.Log("Stopping outletmeter coroutine");
                StopAllCoroutines();
            }

            yield return null;
        } 
    }
}
