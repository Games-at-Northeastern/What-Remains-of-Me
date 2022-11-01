using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// The main script for handling wire controls. Spawns/Fires, despawns, and
/// connects the wire. Also controls bullet time while the wire is being aimed.
/// </summary>
[RequireComponent(typeof(PlugMovementSettings))]
[RequireComponent(typeof(LineRenderer))]
[RequireComponent(typeof(MovementExecuter))]
[RequireComponent(typeof(DistanceJoint2D))]
public class WireThrower : MonoBehaviour
{
    [Header("SFX")]
    public AudioSource src;
    public AudioClip shootWire;
    public float aimDistance;

    // Non-accessible fields
    [SerializeField] Camera mainCamera;
    [SerializeField] GameObject plugPrefab;
    [SerializeField] float timeScaleForAim;
    GameObject activePlug;
    ControlSchemes cs;
    PlugMovementSettings pms;
    MovementExecuter me;
    LineRenderer lineRenderer;
    DistanceJoint2D distanceJoint;
    float framesHeld;
    bool isLockOn = false; // Whether or not Atlas is locking on to an outlet
    GameObject lockOnOutlet;
    Vector2 lastRecordedPosition;

    // Accessible Fields
    public Outlet connectedOutlet { get; private set; } // If null, disconnected. Otherwise, connected.
    public UnityEvent onConnect = new UnityEvent();
    public UnityEvent onDisconnect = new UnityEvent();


    private void Awake()
    {
        // Add left click handling functionality
        cs = new ControlSchemes();
        cs.Enable();
        cs.Player.Throw.started += _ => HandleThrowInputReleasedKeyboard();
        //cs.Player.ThrowController.canceled += _ => HandleThrowInputReleasedController();
        //cs.Player.ThrowMouse.canceled += _ => HandleThrowInputReleasedKeyboard();
        cs.Player.Jump.performed += _ => HandlePotentialDisconnectByJump();
        // Initialize basic variables
        pms = GetComponent<PlugMovementSettings>();
        distanceJoint = GetComponent<DistanceJoint2D>();
        me = GetComponent<MovementExecuter>();
        // Handle line renderer
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.enabled = false;
        connectedOutlet = null;
        framesHeld = 0;
    }

    void HandleThrowInputHeld()
    {
        if (activePlug == null && connectedOutlet == null)
        {
            // Prepare to fire wire
            Time.timeScale = 1;
            framesHeld = 0;
        }
    }

    void HandleThrowInputReleasedKeyboard()
    {
        if (activePlug == null && connectedOutlet == null)
        {
            Time.timeScale = 1;
            if (framesHeld < 0.1)
                if (isLockOn) { FirePlugLockOn(); }
                else { FirePlugAutoAim(); }
            else
                FirePlugMouse();
        }
        HandlePotentialDisconnect();
    }

    void HandleThrowInputReleasedController()
    {
        if (activePlug == null && connectedOutlet == null)
        {
            Time.timeScale = 1;
            if (framesHeld < 0.1)
                if (isLockOn) { FirePlugLockOn(); }
                else { FirePlugAutoAim(); }
            else
                FirePlugController();
        }
        HandlePotentialDisconnect();
    }

    void HandlePotentialDisconnectByJump()
    {
        if (me.GetCurrentMove().DisconnectByJumpOkay())
        {
            HandlePotentialDisconnect();
        }
    }

    void HandlePotentialDisconnect()
    {
        if (connectedOutlet != null)
        {
            Disconnect();
        }
    }

    void FirePlugLockOn()
    {
        if (lockOnOutlet == null)
        {
            ChangeOutletTarget();
        }

        Vector2 playerScreenPos = mainCamera.WorldToScreenPoint(transform.position);
        Vector2 fireDir = playerScreenPos;

        if (lockOnOutlet != null)
        {
            Vector2 closestPos = mainCamera.WorldToScreenPoint(lockOnOutlet.transform.position);
            fireDir = closestPos - playerScreenPos;
        }

        activePlug = Instantiate(plugPrefab, transform.position, transform.rotation);
        PlugMovementExecuter pme = activePlug.GetComponent<PlugMovementExecuter>();
        pme.Fire(new Straight(fireDir, activePlug.transform, transform, pms));
        pme.onTerminateRequest.AddListener(() => DestroyPlug());
        pme.onConnectionRequest.AddListener((GameObject g) => ConnectPlug(g));
    }

    /// <summary>
    /// Spawns a plug and launches it in the air towards the nearest object with the tag "Outlet",
    /// setting it as the active plug.
    /// Prepares for the possibility of the plug despawning or getting connected.
    /// </summary>
    void FirePlugAutoAim()
    {
        GameObject[] gos;
        gos = GameObject.FindGameObjectsWithTag("Outlet");
        GameObject closest = null;
        float distance = Mathf.Infinity;
        Vector3 position = transform.position;
        foreach (GameObject go in gos)
        {
            Vector3 diff = go.transform.position - position;
            float curDistance = diff.sqrMagnitude;
            if (curDistance < distance)
            {
                closest = go;
                distance = curDistance;
            }
        }

        Vector2 playerScreenPos = mainCamera.WorldToScreenPoint(transform.position);
        Vector2 fireDir = playerScreenPos;

        if (closest != null)
        {
            Vector2 closestPos = mainCamera.WorldToScreenPoint(closest.transform.position);
            fireDir = closestPos - playerScreenPos;
        }
        activePlug = Instantiate(plugPrefab, transform.position, transform.rotation);
        PlugMovementExecuter pme = activePlug.GetComponent<PlugMovementExecuter>();
        pme.Fire(new Straight(fireDir, activePlug.transform, transform, pms));
        pme.onTerminateRequest.AddListener(() => DestroyPlug());
        pme.onConnectionRequest.AddListener((GameObject g) => ConnectPlug(g));
    }

    void ChangeOutletTarget()
    {
        if (lockOnOutlet == null)
        {
            GameObject[] gos;
            gos = GameObject.FindGameObjectsWithTag("Outlet");
            GameObject closest = null;
            float distance = Mathf.Infinity;
            Vector3 position = transform.position;
            foreach (GameObject go in gos)
            {
                Vector3 diff = go.transform.position - position;
                float curDistance = diff.sqrMagnitude;
                if (curDistance < distance)
                {
                    closest = go;
                    distance = curDistance;
                    lockOnOutlet = closest;
                }
            }
        }
        else
        {
            GameObject[] gos;
            gos = GameObject.FindGameObjectsWithTag("Outlet");
            GameObject closest = null;
            float originalDistance = Vector2.Distance(this.transform.position, lockOnOutlet.transform.position);
            float distance = Mathf.Infinity;
            Vector3 position = transform.position;
            foreach (GameObject go in gos)
            {
                Vector3 diff = go.transform.position - position;
                float curDistance = diff.sqrMagnitude;
                if (go.GetComponent<SpriteRenderer>().isVisible && curDistance < distance)
                {
                    if (lastRecordedPosition == new Vector2(this.transform.position.x, this.transform.position.y))
                    {
                        if (curDistance > originalDistance)
                        {
                            closest = go;
                            distance = curDistance;
                            lockOnOutlet = closest;
                        }
                    }
                    else
                    {
                        if (go != lockOnOutlet)
                        {
                            closest = go;
                            distance = curDistance;
                            lockOnOutlet = closest;
                        }
                    }
                }
            }
        }
        lastRecordedPosition = transform.position;
    }

    /// <summary>
    /// Using a mouse for input, Spawns a plug and launches it in the air,
    /// setting it as the active plug.
    /// Prepares for the possibility of the plug despawning or getting connected.
    /// </summary>
    void FirePlugMouse()
    {
        Vector2 playerScreenPos = mainCamera.WorldToScreenPoint(transform.position);
        Vector2 aimScreenPos = cs.Player.AimMouse.ReadValue<Vector2>();
        Vector2 fireDir = aimScreenPos - playerScreenPos;
        activePlug = Instantiate(plugPrefab, transform.position, transform.rotation);
        PlugMovementExecuter pme = activePlug.GetComponent<PlugMovementExecuter>();
        pme.Fire(new Straight(fireDir, activePlug.transform, transform, pms));

        // Play SFX for shooting plug
        src.clip = shootWire;
        src.Play();

        pme.onTerminateRequest.AddListener(() => DestroyPlug());
        pme.onConnectionRequest.AddListener((GameObject g) => ConnectPlug(g));
    }

    /// <summary>
    /// Using a controller for input, Spawns a plug and launches it in the air,
    /// setting it as the active plug.
    /// Prepares for the possibility of the plug despawning or getting connected.
    /// </summary>
    void FirePlugController()
    {
        Vector2 fireDir = cs.Player.AimController.ReadValue<Vector2>();
        activePlug = Instantiate(plugPrefab, transform.position, transform.rotation);
        PlugMovementExecuter pme = activePlug.GetComponent<PlugMovementExecuter>();
        pme.Fire(new Straight(fireDir, activePlug.transform, transform, pms));

        // Play SFX for shooting plug
        src.clip = shootWire;
        src.Play();

        pme.onTerminateRequest.AddListener(() => DestroyPlug());
        pme.onConnectionRequest.AddListener((GameObject g) => ConnectPlug(g));
    }

    private void Update()
    {
        HandleLineRendering();
        HandleThrowInputHeld();
        HandleConnectionPhysics();
        framesHeld += Time.deltaTime;
        if (Input.GetKeyDown(KeyCode.Q)) { isLockOn = !isLockOn; }
        if (isLockOn && Input.GetKeyDown(KeyCode.E)) { ChangeOutletTarget(); }
    }

    /// <summary>
    /// Depending on the state of the wire, decides how to handle line
    /// rendering (and whether to do it at all)
    /// </summary>
    void HandleLineRendering()
    {
        lineRenderer.enabled = activePlug != null || connectedOutlet != null;
        if (activePlug != null)
        {
            lineRenderer.SetPosition(0, transform.position);
            lineRenderer.SetPosition(1, activePlug.transform.position);
        }
        else if (connectedOutlet != null)
        {
            lineRenderer.SetPosition(0, transform.position);
            lineRenderer.SetPosition(1, connectedOutlet.transform.position);
        }
    }

    /// <summary>
    /// Manage any physics inherent in wire connection that needs to be
    /// managed frame by frame.
    /// This includes updating the position of the connected outlet, in case
    /// that position ever changes.
    /// </summary>
    void HandleConnectionPhysics()
    {
        if (connectedOutlet != null)
        {
            distanceJoint.connectedAnchor = connectedOutlet.transform.position;
        }
    }

    /// <summary>
    /// Register the plug as connected to the given GameObject.
    /// </summary>
    void ConnectPlug(GameObject g)
    {
        onConnect.Invoke();
        connectedOutlet = g.GetComponent<Outlet>();
        connectedOutlet.Connect();
        distanceJoint.enabled = true;
        distanceJoint.connectedAnchor = connectedOutlet.transform.position;
        Destroy(activePlug);
    }

    /// <summary>
    /// Register the plug as connected to no GameObject.
    /// </summary>
    void Disconnect()
    {
        onDisconnect.Invoke();
        distanceJoint.enabled = false;
        connectedOutlet.Disconnect();
        connectedOutlet = null;
    }

    /// <summary>
    /// Destroy the currently active plug.
    /// </summary>
    void DestroyPlug()
    {
        if (activePlug != null)
            Destroy(activePlug);
    }

    /// <summary>
    /// To be called by the movement system to set the maximum wire length,
    /// this restricting player movement while attached to the wire.
    /// </summary>
    public void SetMaxWireLength(float amount)
    {
        distanceJoint.distance = amount;
    }

    public bool WireExists()
    {
        return lineRenderer.enabled;
    }
}
