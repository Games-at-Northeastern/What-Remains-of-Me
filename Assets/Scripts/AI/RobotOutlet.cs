using UnityEngine;
public class RobotOutlet : AControllable
{
    [Header("Needed Targets")]
    [SerializeField] private GameObject player;
    [SerializeField] private EnemyAIAstar astarScript;
    [SerializeField] private GameObject inkDialogueTriggerOnRobot;

    [Header("inkJSON")]
    [SerializeField] private TextAsset atlasLowEnemyLowVirusScript;
    [SerializeField] private TextAsset atlasLowEnemyMediumVirusScript;
    [SerializeField] private TextAsset atlasLowEnemyHighVirusScript;
    [SerializeField] private TextAsset atlasHighEnemyLowVirusScript;
    [SerializeField] private TextAsset atlasHighEnemyMediumVirusScript;
    [SerializeField] private TextAsset atlasHighEnemyHighVirusScript;

    [Header("Enemy Sprites")]
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Sprite deadRobot;
    [SerializeField] private Sprite chargedRobot;
    [SerializeField] private Sprite virusRobot;

    [Header("Dialogue Trigger Configs")]
    [SerializeField] private float dialogueActivateDistance = 5f;
    [SerializeField] private bool stopMovement = true;
    [SerializeField] private bool autoTurnPage;
    [SerializeField] private float waitForPageTurn = 2f;
    [SerializeField] private float waitForInteractAvailable = 3f;
    [SerializeField] private int virusLevelUpdate;
    private bool _isDead;
    private bool _isPacified;
    private bool _setTriggerActive;

    private TextAsset currentInkJSONScript;

    // Start is called before the first frame update
    private void Awake()
    {
        inkDialogueTriggerOnRobot.SetActive(false);
        _isDead = false;
        _setTriggerActive = false;

        // if enemy robot has a virus level of below 50%, the enemy movement (A* pathfinding) will be set to not follow player (makes it passive)
        if (GetVirus() < 50) {
            astarScript.followEnabled = false;
            _isPacified = true;
        } else {
            _isPacified = false;
        }
    }

    // Update is called once per frame
    private void Update()
    {
        // this if is an intial question to check energy state of the robot, and see if they are active. If you wanted more states 
        // of energy, bring the virus switch case into a helper function, and replace the if statement with a switch statement for energy.
        // You can bring the virus switch case into a hepler function, and call this from the energy switch case. The idea is you have
        // different states of energy, changing how the robot reacts. Then on top of the energy, there is a virus switch case checking how
        // behavior should change depending on virus

        // update sprite based on current virus/clean energy level
        if (cleanEnergy > 1 || virus > 1) {
            spriteRenderer.sprite = chargedRobot;
        } else {
            spriteRenderer.sprite = deadRobot;
        }

        // Update ink script for enemy to say
        if (EnergyManager.Instance.Virus <= 50) // atlas has low virus level
        {
            if (virus < 50) // enemy has low virus level
            {
                currentInkJSONScript = atlasLowEnemyLowVirusScript;
            } else if (virus >= 50 && virus < 80) // enemy has medium virus level
            {
                currentInkJSONScript = atlasLowEnemyMediumVirusScript;
            } else // enemy has high virus level
            {
                currentInkJSONScript = atlasLowEnemyHighVirusScript;
            }

        } else // atlas has high virus level
        {
            if (virus < 50) {
                currentInkJSONScript = atlasHighEnemyLowVirusScript;
            } else if (virus >= 50 && virus < 80) // enemy has medium virus level
            {
                currentInkJSONScript = atlasHighEnemyMediumVirusScript;
            } else // atlas has high virus level
            {
                currentInkJSONScript = atlasHighEnemyHighVirusScript;
            }
        }

        // play script if player is within activate distance and enemy virus level 
        if (!_isPacified) {
            if (PlayerInTriggerDistance() && !InkDialogueManager.GetInstance().dialogueIsPlaying) {
                InkDialogueManager i = InkDialogueManager.GetInstance();
                i.stopMovement = stopMovement;
                i.autoTurnPage = autoTurnPage;
                i.waitBeforePageTurn = waitForPageTurn;
                i.EnterDialogueMode(currentInkJSONScript);
            }
        }

        // will enable the dialogue trigger when the robot is pacified, so that the robot defaults to a regular NPC and can be interacted with using 'F'
        if (_isPacified && !_setTriggerActive) {
            inkDialogueTriggerOnRobot.SetActive(true);
            _setTriggerActive = true;
        }

        // if enemy virus is updated so that it is less than 50%, set the astar to stop following the player
        if (GetVirus() < 50) {
            astarScript.followEnabled = false;
            _isPacified = true;
        } else // virus was increased beyong 50% so robot will attack and enable dialogue through distance (not ink dialogue trigger)
        {
            astarScript.followEnabled = true;
            _isPacified = false;
            inkDialogueTriggerOnRobot.SetActive(false);
        }
    }

    /// <summary>
    ///     Returns true if the player comes within dialogueActivateDistance distance of the enemy
    /// </summary>
    /// <returns>bool</returns>
    private bool PlayerInTriggerDistance() => Vector2.Distance(transform.position, player.transform.position) < dialogueActivateDistance;
}
