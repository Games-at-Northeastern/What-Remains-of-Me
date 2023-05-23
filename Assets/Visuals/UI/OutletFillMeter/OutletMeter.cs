using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class OutletMeter : MonoBehaviour
{
    
    [Range(0, 9)]
    public int Limiter;

    
    [Range(0, 16)]
    public int Virus;

    [Range(0, 16)]
    public int Charge;

    /*public Animation healthBarAnimation;
    public string animationClipName = "HealthBarAnimation";
    public int targetFrame = 5;*/

    /*private void Start()
    {
        ChangeAnimationFrame();
    }

    // Call this method to change the frame of the animation
    public void ChangeAnimationFrame()
    {
        healthBarAnimation.Stop();  // Stop the animation to avoid conflicts
        AnimationClip clip = healthBarAnimation.GetClip(animationClipName);
        float frameRate = clip.frameRate;
        float targetTime = targetFrame / frameRate;

        healthBarAnimation.Play(animationClipName);
        healthBarAnimation[animationClipName].time = targetTime;
        healthBarAnimation.Sample();  // Force the animation to the desired frame
        healthBarAnimation.Stop();  // Stop the animation to freeze it at the desired frame
    }*/

    /*[SerializeField] private SpriteSheetData scriptable;

    [SerializeField] private string limiterPath;
    [SerializeField] private string virusPath;
    [SerializeField] private string chargePath;

    [SerializeField] private Sprite limiterPath;
    [SerializeField] private Sprite virusPath;
    [SerializeField] private Sprite chargePath;*/

    [SerializeField] private Sprite[] limiterSprites;
    [SerializeField] private Sprite[] virusSprites;
    [SerializeField] private Sprite[] chargeSprites;

    [SerializeField] private SpriteRenderer limiterMeter;
    [SerializeField] private SpriteRenderer virusMeter;
    [SerializeField] private SpriteRenderer chargeMeter;

    private void Awake()
    {
        //limiterSprites = Resources.LoadAll<Sprite>(AssetDatabase.GetAssetPath(limiterMeter.GetComponent<SpriteRenderer>().sprite.GetInstanceID()).Replace(".png", "").Replace(limiterPath, ""));
        //virusSprites = Resources.LoadAll<Sprite>(AssetDatabase.GetAssetPath(virusMeter.GetComponent<SpriteRenderer>().sprite.GetInstanceID()).Replace(".png", "").Replace(virusPath, ""));
        //chargeSprites = Resources.LoadAll<Sprite>(AssetDatabase.GetAssetPath(chargeMeter.GetComponent<SpriteRenderer>().sprite.GetInstanceID()).Replace(".png", "").Replace(chargePath, ""));
        /*Sprite[] sprites = new Sprite[scriptable.sprites.Length];
        for (int i = 0; i < scriptable.sprites.Length; i++)
        {
            sprites[i] = (Sprite)scriptable.sprites[i];
        }
        limiterMeter.GetComponent<SpriteRenderer>().sprite = sprites[0];*/
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        limiterMeter.sprite = limiterSprites[Limiter];
        virusMeter.sprite = virusSprites[Virus];
        chargeMeter.sprite = chargeSprites[Charge];
    }
}
