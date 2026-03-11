using UnityEngine;
/// <summary>
///     A script for handling dialogue (changing and triggering) that changes based on Atlas' and the NPC's virus levels.
/// </summary>
public class DynamicDialogue : ADynamicAsset<DialogueEntry>
{
    [Space(15)]
    [SerializeField] [Tooltip("The NPC's virus source. If left null, assigned to the one on this GameObject.")]
    private AControllable _paired;

    private InkDialogueManager _cachedInstance;

    private void Start() => _cachedInstance = InkDialogueManager.GetInstance();

    private void OnTriggerEnter2D(Collider2D other)
    {
        // messages still get sent to disabled monobehaviors.
        if (!enabled) {
            return;
        }

        // the latter clause is to ensure that dialogues that don't lock movement don't mess things up
        if (other.CompareTag("Player") && !_cachedInstance.dialogueIsPlaying) {
            DialogueEntry data = MatchState(PlayerRef.PlayerManager.EnergyManager.Virus, _paired.GetVirus());
            _cachedInstance.stopMovement = data.stopMovement;
            _cachedInstance.autoTurnPage = data.autoPageTurn;
            _cachedInstance.waitBeforePageTurn = data.pageTurnWait;

            _cachedInstance.EnterDialogueMode(data.asset);
        }
    }
}
