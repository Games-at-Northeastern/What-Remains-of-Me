//------------------------------------------------------------------------------
// <auto-generated>
//     This code was auto-generated by com.unity.inputsystem:InputActionCodeGenerator
//     version 1.4.4
//     from Assets/Scripts/Controls/ControlSchemes.inputactions
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

public partial class @ControlSchemes : IInputActionCollection2, IDisposable
{
    public InputActionAsset asset { get; }
    public @ControlSchemes()
    {
        asset = InputActionAsset.FromJson(@"{
    ""name"": ""ControlSchemes"",
    ""maps"": [
        {
            ""name"": ""Player"",
            ""id"": ""05411b70-d2c8-44fb-880c-a0734bd28e48"",
            ""actions"": [
                {
                    ""name"": ""Move"",
                    ""type"": ""Value"",
                    ""id"": ""9462c04d-585c-4c37-a004-c1215e6da1d1"",
                    ""expectedControlType"": ""Axis"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": true
                },
                {
                    ""name"": ""Jump"",
                    ""type"": ""Button"",
                    ""id"": ""a72efe12-9d57-4f77-8374-829960feee7e"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""Dash"",
                    ""type"": ""Button"",
                    ""id"": ""6211cdff-1ee3-4a1d-8a74-547acc33f1de"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""Throw"",
                    ""type"": ""Button"",
                    ""id"": ""0635193c-0a26-41b7-a04e-b6f5ece26cad"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""ThrowMouse"",
                    ""type"": ""Button"",
                    ""id"": ""ada60b51-5e0b-40a6-9e4b-63241fcb7ef2"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""ThrowController"",
                    ""type"": ""Button"",
                    ""id"": ""02b510ba-9854-44d6-a2d2-cc1722e77e7e"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""AimMouse"",
                    ""type"": ""Value"",
                    ""id"": ""e2b90691-3572-4587-bac2-f80360e23b52"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": true
                },
                {
                    ""name"": ""AimController"",
                    ""type"": ""Value"",
                    ""id"": ""3f79ac2c-7202-4c6c-a324-29c46d020182"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": true
                },
                {
                    ""name"": ""GiveEnergy"",
                    ""type"": ""Button"",
                    ""id"": ""84510443-86cd-4c2f-8411-91005b26958e"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""TakeEnergy"",
                    ""type"": ""Button"",
                    ""id"": ""2d39e606-5524-4f06-9a37-6bab42aa58dd"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""Dialogue"",
                    ""type"": ""Button"",
                    ""id"": ""4f6a4455-5e7e-4ea0-84b0-9f464323db54"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""UI Movement"",
                    ""type"": ""Value"",
                    ""id"": ""be20ef65-4008-458f-a79c-844a47b2ca83"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": true
                }
            ],
            ""bindings"": [
                {
                    ""name"": ""1D Axis"",
                    ""id"": ""45b2076e-40ab-48c6-8bf4-87e8d21d086f"",
                    ""path"": ""1DAxis"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""negative"",
                    ""id"": ""843c48ee-6a16-45bd-b1e2-fb63dfd53366"",
                    ""path"": ""<Keyboard>/a"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""positive"",
                    ""id"": ""1a51a183-3ee7-4ebc-ae1e-38766e77e918"",
                    ""path"": ""<Keyboard>/d"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": """",
                    ""id"": ""016638cf-eb80-4a08-8cae-3347608c1110"",
                    ""path"": ""<Gamepad>/leftStick/x"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""7e508754-bd65-40fb-87a5-6354a412e499"",
                    ""path"": ""<Keyboard>/space"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Jump"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""9ee43983-5c7c-47b8-b6f8-67ac33c5c2dc"",
                    ""path"": ""<Gamepad>/buttonSouth"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Jump"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""d996668b-0343-47a9-90bf-05f69b475c82"",
                    ""path"": ""<Keyboard>/shift"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Dash"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""455d53db-075a-4fbd-8119-0bc3791fb731"",
                    ""path"": ""<Keyboard>/rightShift"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Dash"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""c5555bac-4226-405a-9484-40729de4031d"",
                    ""path"": ""<Gamepad>/buttonWest"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Dash"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""c4ad2a8b-531b-4501-954a-45002b58a72d"",
                    ""path"": ""<Mouse>/leftButton"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Throw"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""12e0f4f2-8ee7-478a-ac30-e6346fbe6cb4"",
                    ""path"": ""<Gamepad>/rightTrigger"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Throw"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""e4685b14-639a-4fe6-96e6-2ec43bf0c2e0"",
                    ""path"": ""<Keyboard>/e"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""GiveEnergy"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""c1dd4a3a-f438-4592-9bfd-441c6c85286f"",
                    ""path"": ""<Gamepad>/buttonNorth"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""GiveEnergy"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""2eb5051d-51aa-49fb-a9d8-ae1a15bf100c"",
                    ""path"": ""<Keyboard>/q"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""TakeEnergy"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""63e570c2-a97a-49eb-ac8b-a9b03462b1d2"",
                    ""path"": ""<Gamepad>/buttonWest"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""TakeEnergy"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""8b9039bf-80bd-4ffe-8a8c-b86f9be82740"",
                    ""path"": ""<Mouse>/leftButton"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""ThrowMouse"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""2430b7d5-26d4-4bbb-b278-e83a3e54ba44"",
                    ""path"": ""<Gamepad>/rightTrigger"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""ThrowController"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""7b6f6445-b804-44ba-8ebc-ac0cbd6c77fd"",
                    ""path"": ""<Mouse>/position"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""AimMouse"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""9803ca0a-4425-4ece-a586-0b8059fbe8e9"",
                    ""path"": ""<Gamepad>/leftStick"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""AimController"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""34abc2c5-7a61-4244-8452-ea818f36081c"",
                    ""path"": ""<Keyboard>/enter"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Dialogue"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""2D Vector"",
                    ""id"": ""b37b502a-0506-4b51-acc7-095da763c856"",
                    ""path"": ""2DVector"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""UI Movement"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""up"",
                    ""id"": ""f45ed29b-63bb-47f2-b890-095b6ad4860e"",
                    ""path"": ""<Keyboard>/w"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""UI Movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""down"",
                    ""id"": ""316ebc00-6efa-49bd-9f26-82bf11257489"",
                    ""path"": ""<Keyboard>/s"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""UI Movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""left"",
                    ""id"": ""8a7bc391-e088-4cd2-a504-2fb3b6acf46f"",
                    ""path"": ""<Keyboard>/a"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""UI Movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""right"",
                    ""id"": ""113c6717-4df5-408e-ad55-9eb6f4793fdc"",
                    ""path"": ""<Keyboard>/d"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""UI Movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                }
            ]
        },
        {
            ""name"": ""Debug"",
            ""id"": ""a94b0dfe-6333-4e33-9c70-3ab7d01f2655"",
            ""actions"": [
                {
                    ""name"": ""Restart"",
                    ""type"": ""Button"",
                    ""id"": ""149ee7d2-34e3-4473-895a-814e690b0225"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""Pause"",
                    ""type"": ""Button"",
                    ""id"": ""a581cf48-78fe-4ac5-92cc-b44fd8064bd2"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""4537cf94-5a0a-49cd-987a-5681943eddbc"",
                    ""path"": ""<Keyboard>/rightBracket"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Restart"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""68354e88-65c8-4b05-b276-ad5400351f40"",
                    ""path"": ""<Keyboard>/p"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Pause"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        }
    ],
    ""controlSchemes"": []
}");
        // Player
        m_Player = asset.FindActionMap("Player", throwIfNotFound: true);
        m_Player_Move = m_Player.FindAction("Move", throwIfNotFound: true);
        m_Player_Jump = m_Player.FindAction("Jump", throwIfNotFound: true);
        m_Player_Dash = m_Player.FindAction("Dash", throwIfNotFound: true);
        m_Player_Throw = m_Player.FindAction("Throw", throwIfNotFound: true);
        m_Player_ThrowMouse = m_Player.FindAction("ThrowMouse", throwIfNotFound: true);
        m_Player_ThrowController = m_Player.FindAction("ThrowController", throwIfNotFound: true);
        m_Player_AimMouse = m_Player.FindAction("AimMouse", throwIfNotFound: true);
        m_Player_AimController = m_Player.FindAction("AimController", throwIfNotFound: true);
        m_Player_GiveEnergy = m_Player.FindAction("GiveEnergy", throwIfNotFound: true);
        m_Player_TakeEnergy = m_Player.FindAction("TakeEnergy", throwIfNotFound: true);
        m_Player_Dialogue = m_Player.FindAction("Dialogue", throwIfNotFound: true);
        m_Player_UIMovement = m_Player.FindAction("UI Movement", throwIfNotFound: true);
        // Debug
        m_Debug = asset.FindActionMap("Debug", throwIfNotFound: true);
        m_Debug_Restart = m_Debug.FindAction("Restart", throwIfNotFound: true);
        m_Debug_Pause = m_Debug.FindAction("Pause", throwIfNotFound: true);
    }

    public void Dispose()
    {
        UnityEngine.Object.Destroy(asset);
    }

    public InputBinding? bindingMask
    {
        get => asset.bindingMask;
        set => asset.bindingMask = value;
    }

    public ReadOnlyArray<InputDevice>? devices
    {
        get => asset.devices;
        set => asset.devices = value;
    }

    public ReadOnlyArray<InputControlScheme> controlSchemes => asset.controlSchemes;

    public bool Contains(InputAction action)
    {
        return asset.Contains(action);
    }

    public IEnumerator<InputAction> GetEnumerator()
    {
        return asset.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public void Enable()
    {
        asset.Enable();
    }

    public void Disable()
    {
        asset.Disable();
    }
    public IEnumerable<InputBinding> bindings => asset.bindings;

    public InputAction FindAction(string actionNameOrId, bool throwIfNotFound = false)
    {
        return asset.FindAction(actionNameOrId, throwIfNotFound);
    }
    public int FindBinding(InputBinding bindingMask, out InputAction action)
    {
        return asset.FindBinding(bindingMask, out action);
    }

    // Player
    private readonly InputActionMap m_Player;
    private IPlayerActions m_PlayerActionsCallbackInterface;
    private readonly InputAction m_Player_Move;
    private readonly InputAction m_Player_Jump;
    private readonly InputAction m_Player_Dash;
    private readonly InputAction m_Player_Throw;
    private readonly InputAction m_Player_ThrowMouse;
    private readonly InputAction m_Player_ThrowController;
    private readonly InputAction m_Player_AimMouse;
    private readonly InputAction m_Player_AimController;
    private readonly InputAction m_Player_GiveEnergy;
    private readonly InputAction m_Player_TakeEnergy;
    private readonly InputAction m_Player_Dialogue;
    private readonly InputAction m_Player_UIMovement;
    public struct PlayerActions
    {
        private @ControlSchemes m_Wrapper;
        public PlayerActions(@ControlSchemes wrapper) { m_Wrapper = wrapper; }
        public InputAction @Move => m_Wrapper.m_Player_Move;
        public InputAction @Jump => m_Wrapper.m_Player_Jump;
        public InputAction @Dash => m_Wrapper.m_Player_Dash;
        public InputAction @Throw => m_Wrapper.m_Player_Throw;
        public InputAction @ThrowMouse => m_Wrapper.m_Player_ThrowMouse;
        public InputAction @ThrowController => m_Wrapper.m_Player_ThrowController;
        public InputAction @AimMouse => m_Wrapper.m_Player_AimMouse;
        public InputAction @AimController => m_Wrapper.m_Player_AimController;
        public InputAction @GiveEnergy => m_Wrapper.m_Player_GiveEnergy;
        public InputAction @TakeEnergy => m_Wrapper.m_Player_TakeEnergy;
        public InputAction @Dialogue => m_Wrapper.m_Player_Dialogue;
        public InputAction @UIMovement => m_Wrapper.m_Player_UIMovement;
        public InputActionMap Get() { return m_Wrapper.m_Player; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(PlayerActions set) { return set.Get(); }
        public void SetCallbacks(IPlayerActions instance)
        {
            if (m_Wrapper.m_PlayerActionsCallbackInterface != null)
            {
                @Move.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnMove;
                @Move.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnMove;
                @Move.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnMove;
                @Jump.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnJump;
                @Jump.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnJump;
                @Jump.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnJump;
                @Dash.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnDash;
                @Dash.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnDash;
                @Dash.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnDash;
                @Throw.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnThrow;
                @Throw.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnThrow;
                @Throw.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnThrow;
                @ThrowMouse.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnThrowMouse;
                @ThrowMouse.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnThrowMouse;
                @ThrowMouse.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnThrowMouse;
                @ThrowController.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnThrowController;
                @ThrowController.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnThrowController;
                @ThrowController.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnThrowController;
                @AimMouse.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnAimMouse;
                @AimMouse.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnAimMouse;
                @AimMouse.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnAimMouse;
                @AimController.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnAimController;
                @AimController.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnAimController;
                @AimController.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnAimController;
                @GiveEnergy.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnGiveEnergy;
                @GiveEnergy.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnGiveEnergy;
                @GiveEnergy.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnGiveEnergy;
                @TakeEnergy.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnTakeEnergy;
                @TakeEnergy.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnTakeEnergy;
                @TakeEnergy.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnTakeEnergy;
                @Dialogue.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnDialogue;
                @Dialogue.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnDialogue;
                @Dialogue.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnDialogue;
                @UIMovement.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnUIMovement;
                @UIMovement.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnUIMovement;
                @UIMovement.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnUIMovement;
            }
            m_Wrapper.m_PlayerActionsCallbackInterface = instance;
            if (instance != null)
            {
                @Move.started += instance.OnMove;
                @Move.performed += instance.OnMove;
                @Move.canceled += instance.OnMove;
                @Jump.started += instance.OnJump;
                @Jump.performed += instance.OnJump;
                @Jump.canceled += instance.OnJump;
                @Dash.started += instance.OnDash;
                @Dash.performed += instance.OnDash;
                @Dash.canceled += instance.OnDash;
                @Throw.started += instance.OnThrow;
                @Throw.performed += instance.OnThrow;
                @Throw.canceled += instance.OnThrow;
                @ThrowMouse.started += instance.OnThrowMouse;
                @ThrowMouse.performed += instance.OnThrowMouse;
                @ThrowMouse.canceled += instance.OnThrowMouse;
                @ThrowController.started += instance.OnThrowController;
                @ThrowController.performed += instance.OnThrowController;
                @ThrowController.canceled += instance.OnThrowController;
                @AimMouse.started += instance.OnAimMouse;
                @AimMouse.performed += instance.OnAimMouse;
                @AimMouse.canceled += instance.OnAimMouse;
                @AimController.started += instance.OnAimController;
                @AimController.performed += instance.OnAimController;
                @AimController.canceled += instance.OnAimController;
                @GiveEnergy.started += instance.OnGiveEnergy;
                @GiveEnergy.performed += instance.OnGiveEnergy;
                @GiveEnergy.canceled += instance.OnGiveEnergy;
                @TakeEnergy.started += instance.OnTakeEnergy;
                @TakeEnergy.performed += instance.OnTakeEnergy;
                @TakeEnergy.canceled += instance.OnTakeEnergy;
                @Dialogue.started += instance.OnDialogue;
                @Dialogue.performed += instance.OnDialogue;
                @Dialogue.canceled += instance.OnDialogue;
                @UIMovement.started += instance.OnUIMovement;
                @UIMovement.performed += instance.OnUIMovement;
                @UIMovement.canceled += instance.OnUIMovement;
            }
        }
    }
    public PlayerActions @Player => new PlayerActions(this);

    // Debug
    private readonly InputActionMap m_Debug;
    private IDebugActions m_DebugActionsCallbackInterface;
    private readonly InputAction m_Debug_Restart;
    private readonly InputAction m_Debug_Pause;
    public struct DebugActions
    {
        private @ControlSchemes m_Wrapper;
        public DebugActions(@ControlSchemes wrapper) { m_Wrapper = wrapper; }
        public InputAction @Restart => m_Wrapper.m_Debug_Restart;
        public InputAction @Pause => m_Wrapper.m_Debug_Pause;
        public InputActionMap Get() { return m_Wrapper.m_Debug; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(DebugActions set) { return set.Get(); }
        public void SetCallbacks(IDebugActions instance)
        {
            if (m_Wrapper.m_DebugActionsCallbackInterface != null)
            {
                @Restart.started -= m_Wrapper.m_DebugActionsCallbackInterface.OnRestart;
                @Restart.performed -= m_Wrapper.m_DebugActionsCallbackInterface.OnRestart;
                @Restart.canceled -= m_Wrapper.m_DebugActionsCallbackInterface.OnRestart;
                @Pause.started -= m_Wrapper.m_DebugActionsCallbackInterface.OnPause;
                @Pause.performed -= m_Wrapper.m_DebugActionsCallbackInterface.OnPause;
                @Pause.canceled -= m_Wrapper.m_DebugActionsCallbackInterface.OnPause;
            }
            m_Wrapper.m_DebugActionsCallbackInterface = instance;
            if (instance != null)
            {
                @Restart.started += instance.OnRestart;
                @Restart.performed += instance.OnRestart;
                @Restart.canceled += instance.OnRestart;
                @Pause.started += instance.OnPause;
                @Pause.performed += instance.OnPause;
                @Pause.canceled += instance.OnPause;
            }
        }
    }
    public DebugActions @Debug => new DebugActions(this);
    public interface IPlayerActions
    {
        void OnMove(InputAction.CallbackContext context);
        void OnJump(InputAction.CallbackContext context);
        void OnDash(InputAction.CallbackContext context);
        void OnThrow(InputAction.CallbackContext context);
        void OnThrowMouse(InputAction.CallbackContext context);
        void OnThrowController(InputAction.CallbackContext context);
        void OnAimMouse(InputAction.CallbackContext context);
        void OnAimController(InputAction.CallbackContext context);
        void OnGiveEnergy(InputAction.CallbackContext context);
        void OnTakeEnergy(InputAction.CallbackContext context);
        void OnDialogue(InputAction.CallbackContext context);
        void OnUIMovement(InputAction.CallbackContext context);
    }
    public interface IDebugActions
    {
        void OnRestart(InputAction.CallbackContext context);
        void OnPause(InputAction.CallbackContext context);
    }
}
