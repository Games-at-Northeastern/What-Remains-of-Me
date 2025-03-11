using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InkDialogueAmbientTrigger : MonoBehaviour
{
    [SerializeField] private List<TextAsset> dialougeLines;
    private bool dialogueActive = true;

    [SerializeField] private float timeBetweenDialouge;
    private float timer = 0f;
    private bool playerInRange = false;

    // Start is called before the first frame update
    private void Start()
    {
        Debug.Log(InkDialogueManager.GetInstance());
    }

    // Update is called once per frame
    private void Update()
    {
        timer = Mathf.Max(timer - Time.deltaTime, 0f);
        if (!playerInRange || InkDialogueManager.GetInstance().dialogueIsPlaying || !dialogueActive)
        {
            return;
        }
        if (timer == 0f)
        {
            var i = InkDialogueManager.GetInstance();
            i.waitBeforePageTurn = 2f;
            timer = timeBetweenDialouge;
            i.stopMovement = false;
            i.autoTurnPage = true;
            i.EnterDialogueMode(PickNextDialouge());
        }
        else if (InkDialogueManager.GetInstance().dialogueEnded)
        {
            StartCoroutine(CanInteractAgain());
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerInRange = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerInRange = false;
        }
    }


    private TextAsset PickNextDialouge()
    {
        if(dialougeLines.Count > 1)
        {
            int newAssetIndex = Random.Range(1, dialougeLines.Count - 1);
            TextAsset oldAsset = dialougeLines[0];
            dialougeLines[0] = dialougeLines[newAssetIndex];
            dialougeLines[newAssetIndex] = oldAsset;
        }
        Debug.Log("Line:" + dialougeLines[0].text);
        return dialougeLines[0];
    }

    private IEnumerator CanInteractAgain()
    {
        SetDialogueActive(false);
        yield return new WaitForSeconds(2f);
        SetDialogueActive(true);
        InkDialogueManager.GetInstance().dialogueEnded = false;
    }
    public void SetDialogueActive(bool status) => dialogueActive = status;
}
