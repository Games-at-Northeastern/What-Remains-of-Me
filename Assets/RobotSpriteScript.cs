using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RobotSpriteScript : MonoBehaviour
{
    [SerializeField] public PlayerInfo playerInfo;

    public SpriteRenderer spriteRenderer;


    public Sprite robotSpriteOne;
    public Sprite robotSpriteTwo;
    public Sprite robotSpriteThree;
    public Sprite robotSpriteFour;
    public Sprite robotSpriteFive;
    public Sprite robotSpriteSix;
    public Sprite robotSpriteSeven;
    public Sprite robotSpriteEight;
    public Sprite robotSpriteNine;
    public Sprite robotSpriteTen;


    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (playerInfo.virus < 10) {
            this.gameObject.GetComponent<SpriteRenderer>().sprite = robotSpriteTen;
        } else if(playerInfo.virus < 20 && playerInfo.virus > 9) {
            this.gameObject.GetComponent<SpriteRenderer>().sprite = robotSpriteNine;
        } else if(playerInfo.virus < 30 && playerInfo.virus > 19) {
            this.gameObject.GetComponent<SpriteRenderer>().sprite = robotSpriteEight;
        } else if(playerInfo.virus < 40 && playerInfo.virus > 29) {
            this.gameObject.GetComponent<SpriteRenderer>().sprite = robotSpriteSeven;
        } else if(playerInfo.virus < 50 && playerInfo.virus > 39) {
            this.gameObject.GetComponent<SpriteRenderer>().sprite = robotSpriteSix;
        } else if(playerInfo.virus < 60 && playerInfo.virus > 49) {
            this.gameObject.GetComponent<SpriteRenderer>().sprite = robotSpriteFive;
        } else if(playerInfo.virus < 70 && playerInfo.virus > 59) {
            this.gameObject.GetComponent<SpriteRenderer>().sprite = robotSpriteFour;
        } else if(playerInfo.virus < 80 && playerInfo.virus > 69) {
            this.gameObject.GetComponent<SpriteRenderer>().sprite = robotSpriteThree;
        } else if(playerInfo.virus < 90 && playerInfo.virus > 79) {
            this.gameObject.GetComponent<SpriteRenderer>().sprite = robotSpriteTwo;
        } else if(playerInfo.virus > 89) {
            this.gameObject.GetComponent<SpriteRenderer>().sprite = robotSpriteOne;
        }
    }
}
