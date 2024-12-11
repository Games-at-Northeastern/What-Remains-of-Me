using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using PlayerController;

public class ElevatorController : Interaction
{
    [Tooltip("Can be left null for scenes accessed after acquiring the voice box")]
    [SerializeField] private LevelPortalData defaultNextBuildIndexPortalData;
    [Tooltip("Can be left null for scenes accessed after acquiring the voice box")]
    [SerializeField] private bool upDefualtNextBuild = true;

    [Header("Scene References")]
    [SerializeField] private GameObject player;
    [SerializeField] private PlayerController2D cc;
    [SerializeField] private CompositeCollider2D groundCollider;
    [SerializeField] private GameObject canvas;

    [Header("Internal References")]
    [SerializeField] private GameObject platform;
    [SerializeField] private GameObject panel;
    [SerializeField] private Checkpoint checkpoint;
    [SerializeField] private GameObject elevatorObj;

    private bool hasTriggered = false;

    private List<LevelPortalData> portalData;
    private int thisPortalIndex;

    private void Start()
    {
        var elevatorData = checkpoint.LinkedPortalData as ElevatorPortalData;
        if (elevatorData == null)
        {
            throw new System.NullReferenceException("Elevator linked portal data not of type elevator portal data");
        }

        portalData = elevatorData.Layout;
        thisPortalIndex = elevatorData.Layout.IndexOf(elevatorData);
        if (thisPortalIndex == -1)
        {
            throw new System.Exception("Elevator layout does not contain a you are here self reference");
        }
    }

    private bool usingComplex = false;
    private void Update()
    {
        if (UpgradeHandler.HasVoiceBox)
        {
            usingComplex = true;
        }
        usingComplex = false;
    }

    public void GoToLevel(int index)
    {
        Debug.Log(index);
        UIUp = true; // so player cant reuse elevator

        var end = portalData[index];
        var scene = SceneUtility.GetBuildIndexByScenePath(end.FoundInScene);

        if (index < thisPortalIndex)
        {
            GoUp(scene, end);
        }

        if (index > thisPortalIndex)
        {
            GoDown(scene, end);
        }
    }

    private void GoUp(int scene, LevelPortalData end)
    {
        if (hasTriggered)
        {
            Debug.Log("alr did pardner");
            return;
        }
        hasTriggered = true;

        cc.LockInputs();
        Debug.Log("going up");
        StartCoroutine(UpAnimation(scene, end));
    }

    private IEnumerator UpAnimation(int scene, LevelPortalData end)
    {
        var originalY = platform.transform.position.y;
        var lerp = 0.0f;
        var switched = false;

        while (lerp < 1.0f)
        {
            if (lerp > 0.3f && !switched)
            {
                groundCollider.enabled = false;
                switched = true;
            }

            lerp += Time.deltaTime / 1.5f;
            var newY = originalY + (lerp * 5);
            platform.transform.position = new Vector3(platform.transform.position.x, newY, platform.transform.position.z);
            yield return new WaitForEndOfFrame();
        }

        // do transition
        cc.UnlockInputs();
        NextScene(scene, end);
    }

    private void GoDown(int scene, LevelPortalData end)
    {
        if (hasTriggered)
        {
            return;
        }
        hasTriggered = true;

        cc.LockInputs();

        StartCoroutine(DownAnimation(scene, end));
    }

    private IEnumerator DownAnimation(int scene, LevelPortalData end)
    {
        player.GetComponent<Collider2D>().enabled = false;
        player.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeAll;

        var originalY = player.transform.position.y;
        var lerp = 0.0f;

        while (lerp < 1.0f)
        {
            lerp += Time.deltaTime / 1.5f;
            var newY = originalY - (lerp * 5);
            player.transform.position = new Vector3(player.transform.position.x, newY, player.transform.position.z);
            yield return new WaitForEndOfFrame();
        }

        // do transition
        cc.UnlockInputs();
        NextScene(scene, end);
    }

    private void NextScene(int scene, LevelPortalData end)
    {
        LevelManager.NextStartPotal = end;
        SceneManager.LoadScene(scene);
    }

    // UI
    public bool UIUp { get; set; } = false;
    public override void Execute()
    {
        if (UIUp || !usingComplex)
        {
            return;
        }
        UIUp = true;

        var newPanel = Instantiate(panel, canvas.transform);
        var initializer = newPanel.GetComponentInChildren<ElevatorPanelInitializer>();

        initializer.PassData(portalData, thisPortalIndex);
        initializer.Init(this, cc);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !usingComplex)
        {
            var nextIndex = SceneManager.GetActiveScene().buildIndex + 1;
            if (upDefualtNextBuild)
            {
                GoUp(nextIndex, defaultNextBuildIndexPortalData);
            }
            else
            {
                GoDown(nextIndex, defaultNextBuildIndexPortalData);
            }
        }
    }
}
