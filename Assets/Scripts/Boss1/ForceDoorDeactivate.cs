using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ForceDoorDeactivate : MonoBehaviour
{

    [SerializeField] private SpriteRenderer door;
    [SerializeField] private Sprite openDoorSprite;
    [SerializeField] private Collider2D doorCollider;
    [SerializeField] private Animator doorAnimator;
    private bool _hasVoiceModule;
    
    private void Awake()
    {
        _hasVoiceModule = false;
    }

    private IEnumerator OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && _hasVoiceModule)
        {
            yield return new WaitForSeconds(0.5f);
            door.sprite = openDoorSprite;
            doorAnimator.enabled = false;
            doorCollider.enabled = false;
        }
    }
    private void Update()
    {
	        _hasVoiceModule = ((Ink.Runtime.BoolValue) InkDialogueManager.GetInstance().GetVariableState("voiceModuleObtained")).value;
    }
}
