using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

//Requires some kind of collider to know we're in the hint area
[RequireComponent(typeof(Collider2D))]
public class HintArea : MonoBehaviour
{
    private enum DialogueOrder
    {
        Random,
        In_Order
    }

    private enum HintStyle
    {
        Death,
        Time,
    }


    [Header("Dialogue Options")]
    [SerializeField]
    private DialogueOrder dialogueOrder;

    [SerializeField]
    private TextAsset[] hintDialogue;
    private int hintIndex = 0;

    [Header("Hint Style")]
    [SerializeField]
    private HintStyle hintStyle;

    [SerializeField]
    //The time before hints can start being given out
    private float timeBeforeHints;
    private bool canGiveHints = false;

    [SerializeField]
    //If using the time hintStyle, how much time between hints
    private float timeBetweenHints;

    [SerializeField]
    //If using the death hintStyle, it will only work if you respawn at the associated respawns
    private Checkpoint[] hintCheckpoints;

    void Start()
    {
        if (hintStyle != HintStyle.Death)
        {
            return;
        }

        for (var i = 0; i < hintCheckpoints.Length; i++)
        {
            hintCheckpoints[i].OnRespawn.AddListener(GiveHint);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Player"))
        {
            StartCoroutine(HintDelay(timeBeforeHints));
        }
    }

    private IEnumerator HintDelay(float time)
    {
        yield return new WaitForSeconds(time);

        if (!canGiveHints)
        {
            canGiveHints = true;
            if (hintStyle == HintStyle.Time)
            {
                StartCoroutine(WaitToGiveHint(timeBetweenHints));
            }
        }
    }

    private IEnumerator WaitToGiveHint(float time)
    {
        yield return new WaitUntil(() => canGiveHints);
        while (true)
        {
            yield return new WaitForSeconds(time);
            GiveHint();
        }
    }

    private void GiveHint()
    {
        var dialogueManager = InkDialogueManager.GetInstance();


        if (!dialogueManager.dialogueEnded)
        {
            return;
        }

        dialogueManager.autoTurnPage = true;
        dialogueManager.waitBeforePageTurn = 0;
        dialogueManager.stopMovement = false;

        var hint = GetNextHint();

        print("Giving hint: " + hint.name);

        dialogueManager.EnterDialogueMode(hint);
    }

    private TextAsset GetNextHint()
    {
        var hint = hintDialogue[hintIndex];

        switch (dialogueOrder)
        {
            case DialogueOrder.Random:
                hintIndex = Random.Range(0, hintDialogue.Length);
                break;
            case DialogueOrder.In_Order:
                hintIndex++;
                hintIndex %= hintDialogue.Length;
                break;
            default:
                break;
        }

        return hint;
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Player"))
        {
            StopCoroutine(WaitToGiveHint(timeBetweenHints));
            StopCoroutine(HintDelay(timeBeforeHints));

            canGiveHints = false;
        }
    }
}
