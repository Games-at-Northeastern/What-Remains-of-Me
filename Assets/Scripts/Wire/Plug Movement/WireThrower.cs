using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.Rendering.Universal;
using CharacterController;
using System;
using PlayerController;
/// <summary>
/// The main script for handling wire controls. Spawns/Fires, despawns, and
/// connects the wire. Also controls bullet time while the wire is being aimed.
/// </summary>
public class WireThrower : MonoBehaviour
{
    #region SerializedFields
    [Header("SFX")]
    public AudioSource src;
    public AudioClip shootWire;
    public float aimDistance;

    // Non-accessible fields

    [SerializeField] GameObject plugPrefab;
    //[SerializeField] float timeScaleForAim;

    [SerializeField] private PlugMovementSettings _plugMovementSettings;
    [SerializeField] private LineRenderer _lineRenderer;
    [SerializeField] private DistanceJoint2D _distanceJoint;
    [SerializeField] private GameObject reticle;
    [SerializeField] private ParticleSystem energySparks;
    [SerializeField] private float flowSpeed = 1.0f;
    private Vector2[] playerToOutletPoints = new Vector2[5];
    public AControllable ConnectedOutletsControllable {get; private set;}
    private float virusFromOutlet;
    
    // Accessible Fields
    public Outlet ConnectedOutlet { get; private set; } // If null, disconnected. Otherwise, connected.
    public UnityEvent onConnect = new UnityEvent();
    public UnityEvent onDisconnect = new UnityEvent();
    public PlayerController2D pc;
    public ContactFilter2D contactFilter;


    [SerializeField] private bool DirectionAffectsPriority; // To enable new directional targeting system
    #endregion

    #region Internal References
    private Camera mainCamera;
    private RaycastHit2D[] hits = new RaycastHit2D[1];
    private GameObject _activePlug;
    private ControlSchemes _controlSchemes;
    private float _framesHeld;
    private bool _isLockOn = false; // Whether or not Atlas is locking on to an outlet
    private GameObject _lockOnOutlet;
    private Vector2 _lastRecordedPosition;

    private PlugMovementSettings pms; // For calculating grappling range
    private CharacterController2D cc; // For determining which way the player is facing

    private List<GameObject> outletsInOverrideRange;

    private LevelManager levelManager;
    private Color green = new Color(0.0f, 1.0f, 0.0f);
    private Color purple = new Color(0.5f, 0.0f, 0.5f);
    private bool movingFromPlayer = true;
    #endregion

    #region StartUp
    private void Awake()
    {
        // Add left click handling functionality
        _controlSchemes = new ControlSchemes();
        _controlSchemes.Enable();
        _controlSchemes.Player.Throw.started += HandleThrowInputReleasedKeyboard;
        //_controlSchemes.Player.ThrowController.canceled += _ => HandleThrowInputReleasedController();
        //_controlSchemes.Player.ThrowMouse.canceled += _ => HandleThrowInputReleasedKeyboard();
        //_controlSchemes.Player.Jump.performed += HandlePotentialDisconnectByJump;
        // Handle line renderer
        _lineRenderer.enabled = false;
        ConnectedOutlet = null;
        _framesHeld = 0;
        reticle.GetComponent<Light2D>().enabled = false;
        mainCamera = Camera.main;

        pms = FindObjectOfType<PlugMovementSettings>();
        cc = GetComponentInParent<CharacterController2D>();

        outletsInOverrideRange = new List<GameObject>();
    }

    private void Start()
    {
        RegisterEvents();
    }

    private void RegisterEvents()
    {
        LevelManager.OnPlayerReset.AddListener(DespawnWire);
        LevelManager.OnPlayerDeath.AddListener(DespawnWire);
    }
    #endregion

    #region External Commands
    public void DisconnectWire()
    {
        HandlePotentialDisconnect();
    }
    public void ThrowWire()
    {
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
    /// To be called by the movement system to set the maximum wire length,
    /// this restricting player movement while attached to the wire.
    /// </summary>
    public void SetMaxWireLength(float amount)
    {
        _distanceJoint.distance = amount;
    }
    #endregion

    #region Handlers
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
    /// Is the wire connected to an object
    /// </summary>
    /// <returns></returns>
    public bool IsConnected() => ConnectedOutlet != null;


    /// <summary>
    /// Function that is passed to the control scheme to handle cancelling a throw when the
    /// keyboard/mouse button for this action is released.
    /// </summary>
    void HandleThrowInputReleasedKeyboard(InputAction.CallbackContext ctx)
    {
        if(!pc.LockedOrNot()) 
        {
        ThrowWire();
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

            _distanceJoint.connectedAnchor = ConnectedOutlet.transform.position;

            Vector3 wireThrowerPosition = transform.position;
            Vector3 directionToConnectedOutlet = (ConnectedOutlet.transform.position - wireThrowerPosition).normalized;

            float distanceToOutlet = Vector2.Distance(wireThrowerPosition, ConnectedOutlet.transform.position);
            bool hitSomething = Physics2D.Raycast(wireThrowerPosition, directionToConnectedOutlet, contactFilter, hits, distanceToOutlet) > 0;

            if (hitSomething)
            {
                RaycastHit2D wireRayCast2D = hits[0];
                HandlePotentialDisconnect();
            }
        }
    }
    #endregion

    #region On blank
    private void OnDestroy()
    {
        _controlSchemes.Player.Throw.started -= HandleThrowInputReleasedKeyboard;
    }

    /// <summary>
    /// Keeps track of which outlets are in override priority range
    /// </summary>
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("OutletOverrideRange"))
        {
            outletsInOverrideRange.Add(collision.GetComponentInParent<Outlet>().gameObject);
        }
    }

    /// <summary>
    /// Keeps track of which outlets are no longer in override priority range
    /// </summary>
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("OutletOverrideRange"))
        {
            outletsInOverrideRange.Remove(collision.GetComponentInParent<Outlet>().gameObject);
        }
    }

    #endregion

    #region Lockon

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


    private GameObject lastReticleLock;
    /// <summary>
    /// Sets the lockOnOutlet to the nearest object tagged "Outlet"
    /// Now prioritizes the direction the player is facing if "Direction Affects Priority" is enabled on the WireThrower component
    /// Also prioritizes outlets within override collider range
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
                    // reticle.transform.position = closest.transform.position;
                    // reticle.GetComponent<Renderer>().enabled = true;
                    //distance = curDistance;
                    _lockOnOutlet = closest;
                    _isLockOn = true;
                    UpdateMeter(closest);
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

            bool closestIsOverride = false;

            bool isFacingLeft = cc.LeftOrRight == Facing.left;
            bool currentIsInFront = false;

            Vector3 position = transform.position;

            if (!DirectionAffectsPriority) // If Direction Affects Priority is disabled, revert to old targeting system
            {
                // For every oulet in the level
                foreach (GameObject go in gos)
                {
                    Vector3 diff = go.transform.position - position;
                    float curDistance = Vector2.Distance(this.transform.position, go.transform.position);

                    // If the outlet is on screen
                    if (go.GetComponent<SpriteRenderer>().isVisible)
                    {
                        hasOutletsOnScreen = true;
                        // If our currently closest outlet isn't within override range
                        if (!closestIsOverride)
                        {
                            // If closer than current, target this one instead
                            if (curDistance < originalDistance)
                            {
                                closest = go;
                                _lockOnOutlet = closest;
                                originalDistance = curDistance;
                                UpdateMeter(closest);
                            }

                            // If the currently considered outlet has an override range
                            if (go.GetComponent<Outlet>().grappleOverrideRange != null)
                            {
                                // Check if we are in override range
                                foreach (GameObject overridenOutlet in outletsInOverrideRange)
                                {
                                    // If so, default to this one instead (even if it's further away)
                                    if (go == overridenOutlet)
                                    {
                                        closest = go;
                                        _lockOnOutlet = closest;
                                        originalDistance = curDistance;
                                        UpdateMeter(closest);

                                        // Make a note that the closest is in override range
                                        closestIsOverride = true;
                                    }
                                }
                            }
                        }
                        // If our currently targeted outlet is in override range, and the one we're considering is closer
                        else if (closestIsOverride && curDistance < originalDistance)
                        {
                            // Check if the new one is also in override range
                            foreach (GameObject overridenOutlet in outletsInOverrideRange)
                            {
                                // If so, switch to this one
                                if (go == overridenOutlet)
                                {
                                    closest = go;
                                    _lockOnOutlet = closest;
                                    originalDistance = curDistance;
                                    UpdateMeter(closest);
                                }
                            }
                        }
                    }
                }
            }
            // Begin new Directional Targeting System
            else
            {
                // Loop through all outlets in level
                foreach (GameObject go in gos)
                {
                    Vector3 diff = go.transform.position - position;
                    float curDistance = Vector2.Distance(this.transform.position, go.transform.position);

                    // If the outlet is on screen
                    if (go.GetComponent<SpriteRenderer>().isVisible)
                    {
                        hasOutletsOnScreen = true;

                        // Check to see if we are facing this outlet
                        bool isInFront = false;
                        float xdiff = go.transform.position.x - this.transform.position.x;

                        // If the outlet is to the right and we're facing right
                        if (xdiff >= 0 && !isFacingLeft)
                        {
                            isInFront = true;
                        }
                        // Or if the outlet is to the left and we're facing left
                        else if (xdiff <= 0 && isFacingLeft)
                        {
                            isInFront = true;
                        }

                        // If our currently targeted outlet isn't in override range
                        if (!closestIsOverride)
                        {
                            // If our currently targeted outlet is behind us...
                            if (!currentIsInFront)
                            {
                                // ...and either the new one is in front and in range, or behind us and closer, set this new one as the target
                                if ((isInFront && curDistance < pms.StraightSpeed * pms.StraightTimeTillRetraction + 0.75f) || (!isInFront && curDistance < originalDistance))
                                {
                                    closest = go;
                                    _lockOnOutlet = closest;
                                    originalDistance = curDistance;
                                    UpdateMeter(closest);

                                    // If the new one is in front, make a note of that
                                    if (isInFront)
                                    {
                                        currentIsInFront = true;
                                    }
                                }
                            }
                            // If our currently targeted outlet is in front of us, then the new one has to also be in front of us and closer for us to switch to it
                            else
                            {
                                if (isInFront && curDistance < originalDistance)
                                {
                                    closest = go;
                                    _lockOnOutlet = closest;
                                    originalDistance = curDistance;
                                    UpdateMeter(closest);
                                }
                            }

                            // If the outlet we're considering has an override range
                            if (go.GetComponent<Outlet>().grappleOverrideRange != null)
                            {
                                // Check to see if we're in override range
                                foreach (GameObject overridenOutlet in outletsInOverrideRange)
                                {
                                    // If we are, switch to this one regardless of which way we're facing
                                    if (go == overridenOutlet)
                                    {
                                        closest = go;
                                        _lockOnOutlet = closest;
                                        originalDistance = curDistance;
                                        UpdateMeter(closest);

                                        // If the new one is in front, make a note of that
                                        if (isInFront)
                                        {
                                            currentIsInFront = true;
                                        }

                                        // Make a note that we're targeting an overridden outlet
                                        closestIsOverride = true;
                                    }
                                }
                            }
                        }
                        // If the currently targeted outlet is overridden and the one we're considering is closer
                        else if (closestIsOverride && curDistance < originalDistance)
                        {
                            // Check if the new one is also overridden
                            foreach (GameObject overridenOutlet in outletsInOverrideRange)
                            {
                                // If it is...
                                if (go == overridenOutlet)
                                {
                                    // ...and the outlet we're targeting is behind us...
                                    if (!currentIsInFront)
                                    {
                                        // ...and the new one is either in front of us and within range, or behind us and closer...
                                        if ((isInFront && curDistance < pms.StraightSpeed * pms.StraightTimeTillRetraction + 0.75f) || (!isInFront && curDistance < originalDistance))
                                        {
                                            // Switch to this one
                                            closest = go;
                                            _lockOnOutlet = closest;
                                            originalDistance = curDistance;
                                            UpdateMeter(closest);

                                            // If the new one is in front, make a note of that
                                            if (isInFront)
                                            {
                                                currentIsInFront = true;
                                            }
                                        }
                                    }
                                    // If the outlet we're currently targeting is in front of us
                                    else
                                    {
                                        // The new outlet must be overridden, in front of us, and closer to swap to it
                                        if (isInFront && curDistance < originalDistance)
                                        {
                                            closest = go;
                                            _lockOnOutlet = closest;
                                            originalDistance = curDistance;
                                            UpdateMeter(closest);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            // End new Directional Targeting System

            if (!hasOutletsOnScreen)
            {
                _lockOnOutlet = null;
                _isLockOn = false;
            }
        }
        _lastRecordedPosition = transform.position;
    }

    /// <summary>
    /// Updates the outlet meter based on the closest GameObject.
    /// </summary>
    /// <param name="closest">The closest GameObject to update the meter for.</param>
    private void UpdateMeter(GameObject closest)
    {
        // Get the OutletMeter component from the previous locked reticle (if any)
        OutletMeter outletMeter = lastReticleLock?.GetComponentInChildren<OutletMeter>();

        // End the visuals of the previous locked reticle's outlet meter (if any)
        outletMeter?.EndVisuals();

        // Update the lastReticleLock to the closest GameObject
        lastReticleLock = closest;

        // Get the OutletMeter component from the new locked reticle (if any)
        outletMeter = lastReticleLock?.GetComponentInChildren<OutletMeter>();

        // Start the visuals of the new locked reticle's outlet meter (if any)
        outletMeter?.StartVisuals();
    }

    #endregion

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
    /// Determines whether or not the Wire for this WireThrower exits.
    /// </summary>
    /// <returns>the state of the line renderer attached to this game object</returns>
    public bool WireExists()
    {
        return _lineRenderer.enabled;
    }

    /// <summary>
    /// Register this as a listener to any necessary events
    /// </summary>

    private void Update()
    {
        ChangeOutletTarget();
        // sets reticle to targe the locked on outlet at all times
        if (_lockOnOutlet != null)
        {
            reticle.transform.position = _lockOnOutlet.transform.position;

            // Only show the reticle if the plug is within range
            if (Vector2.Distance(transform.position, reticle.transform.position) <= pms.StraightSpeed * pms.StraightTimeTillRetraction + 0.75f)
            {
                reticle.GetComponent<Light2D>().enabled = true;
            }
            else
            {
                reticle.GetComponent<Light2D>().enabled = false;
            }
        }
        else
        {
            reticle.GetComponent<Light2D>().enabled = false;
        }
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
        OutletMeter outletMeter = ConnectedOutlet.GetComponentInChildren<OutletMeter>();
        outletMeter?.StartVisuals();
        outletMeter?.ConnectPlug();
        ConnectedOutletsControllable = ConnectedOutlet.controlled;
        if(ConnectedOutletsControllable != null)
        {
            ConnectedOutletsControllable.OnVirusChange.AddListener(setSparkColor);
            ConnectedOutletsControllable.OnEnergyChange.AddListener(showEnergyFlow);
        }
        
    }

    /// <summary>
    /// Register the plug as connected to no GameObject.
    /// </summary>
    void Disconnect()
    {
        OutletMeter outletMeter = ConnectedOutlet.GetComponentInChildren<OutletMeter>();
        outletMeter?.DisconnectPlug();
        UpdateMeter(lastReticleLock);
        onDisconnect.Invoke();
        _distanceJoint.enabled = false;
        if (ConnectedOutletsControllable != null)
        {
            ConnectedOutletsControllable.OnVirusChange.RemoveListener(setSparkColor);
            ConnectedOutletsControllable.OnEnergyChange.RemoveListener(showEnergyFlow);
            ConnectedOutletsControllable = null;
        }
        ConnectedOutlet.Disconnect();
        ConnectedOutlet = null;
    }

    void setSparkColor(float newVirusPercentage)
    {
        var mainModule = energySparks.main;
        Color lerpedColor = Color.Lerp(green, purple, newVirusPercentage);
        mainModule.startColor = lerpedColor;
    }

    void showEnergyFlow(float newEnergy)
    {
        Debug.Log("showing energy flow");
        float step = flowSpeed * Time.deltaTime;
        if (newEnergy > 0 )
        {
            energySparks.transform.position = Vector3.MoveTowards(energySparks.transform.position, ConnectedOutlet.transform.position, step);
            //away from player
        }
        else
        {
            energySparks.transform.position = Vector3.MoveTowards(energySparks.transform.position, this.transform.position, step);
        }
    }

    /// <summary>
    /// Destroy the currently active plug.
    /// </summary>
    void DestroyPlug()
    {
        if (_activePlug != null)
            Destroy(_activePlug);
    }

    /// <summary>
    /// Disconnects and immediately destroys the plug
    /// </summary>
    private void DespawnWire()
    {
        HandlePotentialDisconnect();
        DestroyPlug();
    }

    #region Deprecated
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
    #endregion
}
