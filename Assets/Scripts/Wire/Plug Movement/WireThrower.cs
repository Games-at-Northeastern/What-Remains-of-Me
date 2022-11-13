using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// The main script for handling wire controls. Spawns/Fires, despawns, and
/// connects the wire. Also controls bullet time while the wire is being aimed.
/// </summary>
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
    private GameObject _activePlug;
    private ControlSchemes _controlSchemes;
    [SerializeField] private PlugMovementSettings _plugMovementSettings;
    [SerializeField] private MovementExecuter _movementExecuter;
    [SerializeField] private LineRenderer _lineRenderer;
    [SerializeField] private DistanceJoint2D _distanceJoint;
    [SerializeField] private GameObject reticle;
    private float _framesHeld;
    private bool _isLockOn = false; // Whether or not Atlas is locking on to an outlet
    private GameObject _lockOnOutlet;
    private Vector2 _lastRecordedPosition;

    // Accessible Fields
    public Outlet ConnectedOutlet { get; private set; } // If null, disconnected. Otherwise, connected.
    public UnityEvent onConnect = new UnityEvent();
    public UnityEvent onDisconnect = new UnityEvent();


    private void Awake()
    {




        // Add left click handling functionality
        _controlSchemes = new ControlSchemes();
        _controlSchemes.Enable();
        _controlSchemes.Player.Throw.started += _ => HandleThrowInputReleasedKeyboard();
        //_controlSchemes.Player.ThrowController.canceled += _ => HandleThrowInputReleasedController();
        //_controlSchemes.Player.ThrowMouse.canceled += _ => HandleThrowInputReleasedKeyboard();
        _controlSchemes.Player.Jump.performed += _ => HandlePotentialDisconnectByJump();
        // Handle line renderer
        _lineRenderer.enabled = false;
        ConnectedOutlet = null;
        _framesHeld = 0;
        reticle.GetComponent<Renderer>().enabled = false;
    }

    /// <summary>
    /// Function that is passed to the control scheme to handle the start of a throw.
    /// </summary>
    void HandleThrowInputHeld()
    {
        if (_activePlug == null && ConnectedOutlet == null)
        {
            // Prepare to fire wire
            Time.timeScale = 1;
            _framesHeld = 0;
        }
    }

    /// <summary>
    /// Function that is passed to the control scheme to handle cancelling a throw when the
    /// keyboard/mouse button for this action is released.
    /// </summary>
    void HandleThrowInputReleasedKeyboard()
    {
        Debug.Log("LOCKED ON: " + _isLockOn);
        if (_activePlug == null && ConnectedOutlet == null)
        {
            Time.timeScale = 1;
            // if (_framesHeld < 0.1)
            //     if (_isLockOn) { FirePlugLockOn(); }
            //     else { FirePlugAutoAim(); }
            // else
            //     FirePlugMouse();
            if (_isLockOn)
            {
                FirePlugLockOn();
            }
            else
            {
                FirePlugMouse();
            }

        }
        HandlePotentialDisconnect();
    }

    /// <summary>
    /// Function that is passed to the control scheme to handle cancelling a throw when the
    /// controller button for this action is released.
    /// </summary>
    void HandleThrowInputReleasedController()
    {
        if (_activePlug == null && ConnectedOutlet == null)
        {
            Time.timeScale = 1;
            if (_framesHeld < 0.1)
                if (_isLockOn) { FirePlugLockOn(); }
                else { FirePlugAutoAim(); }
            else
                FirePlugController();
        }
        HandlePotentialDisconnect();
    }

    /// <summary>
    /// Function passed to the control scheme to handle disconnecting the wire when jumping.
    /// </summary>
    void HandlePotentialDisconnectByJump()
    {
        if (_movementExecuter.GetCurrentMove().DisconnectByJumpOkay())
        {
            HandlePotentialDisconnect();
        }
    }

    /// <summary>
    /// Handles disconnection of wire.
    /// </summary>
    void HandlePotentialDisconnect()
    {
        if (ConnectedOutlet != null)
        {
            Disconnect();
        }
    }

    /// <summary>
    /// Spawns a plug and launches it in the air towards the target outlet (found by calling ChangeOutletTarget).
    /// Prepares for the possibility of the plug despawning or getting connected.
    /// </summary>
    void FirePlugLockOn()
    {
        if (_lockOnOutlet == null)
        {
            ChangeOutletTarget();
        }

        Vector2 playerScreenPos = mainCamera.WorldToScreenPoint(transform.position);
        Vector2 fireDir = playerScreenPos;

        if (_lockOnOutlet != null)
        {
            Vector2 closestPos = mainCamera.WorldToScreenPoint(_lockOnOutlet.transform.position);
            fireDir = closestPos - playerScreenPos;

        }

        _activePlug = Instantiate(plugPrefab, transform.position, transform.rotation);
        PlugMovementExecuter pme = _activePlug.GetComponent<PlugMovementExecuter>();
        pme.Fire(new Straight(fireDir, _activePlug.transform, transform, _plugMovementSettings));
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
            Debug.Log("FOUND");
            reticle.transform.position = closest.transform.position;
            reticle.GetComponent<Renderer>().enabled = true;
            Vector2 closestPos = mainCamera.WorldToScreenPoint(closest.transform.position);
            fireDir = closestPos - playerScreenPos;
        }
        _activePlug = Instantiate(plugPrefab, transform.position, transform.rotation);
        PlugMovementExecuter pme = _activePlug.GetComponent<PlugMovementExecuter>();
        pme.Fire(new Straight(fireDir, _activePlug.transform, transform, _plugMovementSettings));
        pme.onTerminateRequest.AddListener(() => DestroyPlug());
        pme.onConnectionRequest.AddListener((GameObject g) => ConnectPlug(g));
    }

    /// <summary>
    /// Sets the lockOnOutlet to the nearest object tagged "Outlet".
    /// </summary>
    void ChangeOutletTarget()
    {
        if (_lockOnOutlet == null)
        {
            GameObject[] gos;
            gos = GameObject.FindGameObjectsWithTag("Outlet");
            GameObject closest = null;
            Vector3 position = transform.position;
            foreach (GameObject go in gos)
            {
                Vector3 diff = go.transform.position - position;
                float curDistance = diff.sqrMagnitude;
                if (go.GetComponent<SpriteRenderer>().isVisible)
                {
                    closest = go;
                    reticle.transform.position = closest.transform.position;
                    reticle.GetComponent<Renderer>().enabled = true;
                    //distance = curDistance;
                    _lockOnOutlet = closest;
                    _isLockOn = true;
                }
            }
        }
        else
        {
            GameObject[] gos;
            gos = GameObject.FindGameObjectsWithTag("Outlet");
            GameObject closest = null;
            float originalDistance = Vector2.Distance(this.transform.position, _lockOnOutlet.transform.position);
            bool hasOutletsOnScreen = false;
            Vector3 position = transform.position;
            foreach (GameObject go in gos)
            {
                Vector3 diff = go.transform.position - position;
                float curDistance = Vector2.Distance(this.transform.position, go.transform.position);
                if (go.GetComponent<SpriteRenderer>().isVisible)
                {
                    hasOutletsOnScreen = true;
                    if (curDistance < originalDistance)
                    {
                        closest = go;
                        reticle.transform.position = closest.transform.position;
                        reticle.GetComponent<Renderer>().enabled = true;
                        //distance = curDistance;
                        _lockOnOutlet = closest;
                        originalDistance = curDistance;
                    }
                }
            }

            if (!hasOutletsOnScreen)
            {
                reticle.GetComponent<Renderer>().enabled = false;
                //distance = curDistance;
                _lockOnOutlet = null;
                _isLockOn = false;
            }
        }
        _lastRecordedPosition = transform.position;
    }

    /// <summary>
    /// Using a mouse for input, Spawns a plug and launches it in the air,
    /// setting it as the active plug.
    /// Prepares for the possibility of the plug despawning or getting connected.
    /// </summary>
    void FirePlugMouse()
    {
        Vector2 playerScreenPos = mainCamera.WorldToScreenPoint(transform.position);
        Vector2 aimScreenPos = _controlSchemes.Player.AimMouse.ReadValue<Vector2>();
        Vector2 fireDir = aimScreenPos - playerScreenPos;
        _activePlug = Instantiate(plugPrefab, transform.position, transform.rotation);
        PlugMovementExecuter pme = _activePlug.GetComponent<PlugMovementExecuter>();
        pme.Fire(new Straight(fireDir, _activePlug.transform, transform, _plugMovementSettings));

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
        Vector2 fireDir = _controlSchemes.Player.AimController.ReadValue<Vector2>();
        _activePlug = Instantiate(plugPrefab, transform.position, transform.rotation);
        PlugMovementExecuter pme = _activePlug.GetComponent<PlugMovementExecuter>();
        pme.Fire(new Straight(fireDir, _activePlug.transform, transform, _plugMovementSettings));

        // Play SFX for shooting plug
        src.clip = shootWire;
        src.Play();

        pme.onTerminateRequest.AddListener(() => DestroyPlug());
        pme.onConnectionRequest.AddListener((GameObject g) => ConnectPlug(g));
    }

    private void Update()
    {
        ChangeOutletTarget();
        HandleLineRendering();
        HandleThrowInputHeld();
        HandleConnectionPhysics();
        _framesHeld += Time.deltaTime;
        // REMOVED THIS AND REPLACED THIS WITH AUTO TARGETING RETICLE
        //if (Input.GetKeyDown(KeyCode.Q)) { _isLockOn = !_isLockOn; }
        // if (_isLockOn && Input.GetKeyDown(KeyCode.E)) { ChangeOutletTarget(); }
        // // lock-on reticle is not visible when the plug has locked on
        // if (ConnectedOutlet != null) {
        //     reticle.GetComponent<Renderer>().enabled = false;
        // }


    }

    /// <summary>
    /// Depending on the state of the wire, decides how to handle line
    /// rendering (and whether to do it at all)
    /// </summary>
    void HandleLineRendering()
    {
        _lineRenderer.enabled = _activePlug != null || ConnectedOutlet != null;
        if (_activePlug != null)
        {
            _lineRenderer.SetPosition(0, transform.position);
            _lineRenderer.SetPosition(1, _activePlug.transform.position);
        }
        else if (ConnectedOutlet != null)
        {
            _lineRenderer.SetPosition(0, transform.position);
            _lineRenderer.SetPosition(1, ConnectedOutlet.transform.position);
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
        if (ConnectedOutlet != null)
        {
            _distanceJoint.connectedAnchor = ConnectedOutlet.transform.position;
        }
    }

    /// <summary>
    /// Register the plug as connected to the given GameObject.
    /// </summary>
    void ConnectPlug(GameObject g)
    {
        onConnect.Invoke();
        ConnectedOutlet = g.GetComponent<Outlet>();
        ConnectedOutlet.Connect();
        _distanceJoint.enabled = true;
        _distanceJoint.connectedAnchor = ConnectedOutlet.transform.position;
        Destroy(_activePlug);
    }

    /// <summary>
    /// Register the plug as connected to no GameObject.
    /// </summary>
    void Disconnect()
    {
        onDisconnect.Invoke();
        _distanceJoint.enabled = false;
        ConnectedOutlet.Disconnect();
        ConnectedOutlet = null;
    }

    /// <summary>
    /// Destroy the currently active plug.
    /// </summary>
    void DestroyPlug()
    {
        if (_activePlug != null)
            Destroy(_activePlug);
            reticle.GetComponent<Renderer>().enabled = false;

    }

    /// <summary>
    /// To be called by the movement system to set the maximum wire length,
    /// this restricting player movement while attached to the wire.
    /// </summary>
    public void SetMaxWireLength(float amount)
    {
        _distanceJoint.distance = amount;
    }

    /// <summary>
    /// Determines whether or not the Wire for this WireThrower exits.
    /// </summary>
    /// <returns>the state of the line renderer attached to this game object</returns>
    public bool WireExists()
    {
        return _lineRenderer.enabled;
    }
}
