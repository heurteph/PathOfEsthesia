// GENERATED AUTOMATICALLY FROM 'Assets/Inputs/Inputs.inputactions'

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

public class @Inputs : IInputActionCollection, IDisposable
{
    public InputActionAsset asset { get; }
    public @Inputs()
    {
        asset = InputActionAsset.FromJson(@"{
    ""name"": ""Inputs"",
    ""maps"": [
        {
            ""name"": ""Player"",
            ""id"": ""9fb4fe32-b6ed-40ac-9281-fb219975dd70"",
            ""actions"": [
                {
                    ""name"": ""Walk"",
                    ""type"": ""Value"",
                    ""id"": ""b5c4afcd-140b-49b3-bae1-e2bd8d00563a"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""LookAround"",
                    ""type"": ""Value"",
                    ""id"": ""81add202-1392-4016-8400-bd5bfdc216d3"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""ProtectEyes"",
                    ""type"": ""Button"",
                    ""id"": ""6b7acb6c-6bd3-4e53-964f-60925b18677a"",
                    ""expectedControlType"": """",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""ProtectEars"",
                    ""type"": ""Button"",
                    ""id"": ""f454eaaf-bbd5-4e5f-8e16-efd0124aab39"",
                    ""expectedControlType"": """",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Interact"",
                    ""type"": ""Button"",
                    ""id"": ""7e37496e-044b-4f7a-bf6f-276cf5baccd2"",
                    ""expectedControlType"": """",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Quit"",
                    ""type"": ""Button"",
                    ""id"": ""7b24222e-529f-4651-99b5-cc7f1723cde5"",
                    ""expectedControlType"": """",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""GodMode"",
                    ""type"": ""Button"",
                    ""id"": ""973ba557-eae4-43ca-9e9e-c992c77d144d"",
                    ""expectedControlType"": """",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""SkipDialog"",
                    ""type"": ""Button"",
                    ""id"": ""145e2b22-bb7d-4eb7-a789-c7b18a406ee9"",
                    ""expectedControlType"": """",
                    ""processors"": """",
                    ""interactions"": """"
                }
            ],
            ""bindings"": [
                {
                    ""name"": ""2D Vector"",
                    ""id"": ""0afdaaab-4410-482a-a8bc-4855977bb57b"",
                    ""path"": ""2DVector(mode=2)"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Walk"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""up"",
                    ""id"": ""83c41280-28f3-4af3-a65b-a34104039c80"",
                    ""path"": ""<Keyboard>/w"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Mouse-Keyboard"",
                    ""action"": ""Walk"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""left"",
                    ""id"": ""540f6638-3cbc-4905-8210-da22910a2aa7"",
                    ""path"": ""<Keyboard>/a"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Mouse-Keyboard"",
                    ""action"": ""Walk"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""down"",
                    ""id"": ""63c18fac-e3c9-49f6-acdd-e8ce26d922ce"",
                    ""path"": ""<Keyboard>/s"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Mouse-Keyboard"",
                    ""action"": ""Walk"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""right"",
                    ""id"": ""a1045a32-3a6b-4509-9850-91eed3cfcbd9"",
                    ""path"": ""<Keyboard>/d"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Mouse-Keyboard"",
                    ""action"": ""Walk"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": """",
                    ""id"": ""a02fa8a4-a2f6-4741-a4e6-86205b67b218"",
                    ""path"": ""<Gamepad>/leftStick"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Gamepad"",
                    ""action"": ""Walk"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""2D Vector"",
                    ""id"": ""d4e36451-b170-42ff-bc23-04df577bb159"",
                    ""path"": ""2DVector(mode=2)"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""LookAround"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""up"",
                    ""id"": ""7265964b-3e5b-46b8-80ee-366fde0a3f10"",
                    ""path"": ""<Keyboard>/upArrow"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Mouse-Keyboard"",
                    ""action"": ""LookAround"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""left"",
                    ""id"": ""07a8359f-9483-45df-89ce-12e4da88f14f"",
                    ""path"": ""<Keyboard>/leftArrow"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Mouse-Keyboard"",
                    ""action"": ""LookAround"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""down"",
                    ""id"": ""c61ea7a2-0040-48ac-b59e-500bda90ce6b"",
                    ""path"": ""<Keyboard>/downArrow"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Mouse-Keyboard"",
                    ""action"": ""LookAround"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""right"",
                    ""id"": ""d37aa716-7dac-4244-b23e-3931becd8a55"",
                    ""path"": ""<Keyboard>/leftArrow"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Mouse-Keyboard"",
                    ""action"": ""LookAround"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": """",
                    ""id"": ""2a7923c4-e220-4be5-b1fb-5e2c2ad9e378"",
                    ""path"": ""<Gamepad>/rightStick"",
                    ""interactions"": """",
                    ""processors"": ""ScaleVector2"",
                    ""groups"": ""Gamepad"",
                    ""action"": ""LookAround"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""b178e623-8909-40de-bb7a-8fcc285284e8"",
                    ""path"": ""<Mouse>/leftButton"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Mouse-Keyboard"",
                    ""action"": ""ProtectEyes"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""100f4a26-d12d-4da2-9d98-b3d00f243cee"",
                    ""path"": ""<Gamepad>/buttonWest"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Gamepad"",
                    ""action"": ""ProtectEyes"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""15012268-4b2f-4229-813f-97b3d8f09b72"",
                    ""path"": ""<Keyboard>/escape"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Mouse-Keyboard"",
                    ""action"": ""Quit"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""fec07214-dc13-4b72-b5b8-471cb76eb55a"",
                    ""path"": ""<Gamepad>/start"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Gamepad"",
                    ""action"": ""Quit"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""55fee37f-7d14-432a-bce5-be44e86b75de"",
                    ""path"": ""<Mouse>/rightButton"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Mouse-Keyboard"",
                    ""action"": ""ProtectEars"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""a17d6c09-64c9-443c-8456-7d25e0237842"",
                    ""path"": ""<Gamepad>/buttonNorth"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Gamepad"",
                    ""action"": ""ProtectEars"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""c8f794e8-e65c-441e-9263-4493bed3830d"",
                    ""path"": ""<Keyboard>/e"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Mouse-Keyboard"",
                    ""action"": ""Interact"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""c9ab370b-8ef3-40f5-a51c-f257aade9e3b"",
                    ""path"": ""<Gamepad>/buttonSouth"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Gamepad"",
                    ""action"": ""Interact"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""ee428ff2-86a0-4f63-be7a-8bd03b885352"",
                    ""path"": ""<Keyboard>/g"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Mouse-Keyboard"",
                    ""action"": ""GodMode"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""1a078053-f1a6-4f51-b814-0e558b184536"",
                    ""path"": ""<Gamepad>/buttonEast"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Gamepad"",
                    ""action"": ""GodMode"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""a3e8756c-28d3-4d76-bd79-25a96ea48f74"",
                    ""path"": ""<Keyboard>/space"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Mouse-Keyboard"",
                    ""action"": ""SkipDialog"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""a99dddd5-ad01-4b29-b3b9-f91ed401defd"",
                    ""path"": ""<Gamepad>/buttonSouth"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Gamepad"",
                    ""action"": ""SkipDialog"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        }
    ],
    ""controlSchemes"": [
        {
            ""name"": ""Gamepad"",
            ""bindingGroup"": ""Gamepad"",
            ""devices"": []
        },
        {
            ""name"": ""Mouse-Keyboard"",
            ""bindingGroup"": ""Mouse-Keyboard"",
            ""devices"": []
        }
    ]
}");
        // Player
        m_Player = asset.FindActionMap("Player", throwIfNotFound: true);
        m_Player_Walk = m_Player.FindAction("Walk", throwIfNotFound: true);
        m_Player_LookAround = m_Player.FindAction("LookAround", throwIfNotFound: true);
        m_Player_ProtectEyes = m_Player.FindAction("ProtectEyes", throwIfNotFound: true);
        m_Player_ProtectEars = m_Player.FindAction("ProtectEars", throwIfNotFound: true);
        m_Player_Interact = m_Player.FindAction("Interact", throwIfNotFound: true);
        m_Player_Quit = m_Player.FindAction("Quit", throwIfNotFound: true);
        m_Player_GodMode = m_Player.FindAction("GodMode", throwIfNotFound: true);
        m_Player_SkipDialog = m_Player.FindAction("SkipDialog", throwIfNotFound: true);
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

    // Player
    private readonly InputActionMap m_Player;
    private IPlayerActions m_PlayerActionsCallbackInterface;
    private readonly InputAction m_Player_Walk;
    private readonly InputAction m_Player_LookAround;
    private readonly InputAction m_Player_ProtectEyes;
    private readonly InputAction m_Player_ProtectEars;
    private readonly InputAction m_Player_Interact;
    private readonly InputAction m_Player_Quit;
    private readonly InputAction m_Player_GodMode;
    private readonly InputAction m_Player_SkipDialog;
    public struct PlayerActions
    {
        private @Inputs m_Wrapper;
        public PlayerActions(@Inputs wrapper) { m_Wrapper = wrapper; }
        public InputAction @Walk => m_Wrapper.m_Player_Walk;
        public InputAction @LookAround => m_Wrapper.m_Player_LookAround;
        public InputAction @ProtectEyes => m_Wrapper.m_Player_ProtectEyes;
        public InputAction @ProtectEars => m_Wrapper.m_Player_ProtectEars;
        public InputAction @Interact => m_Wrapper.m_Player_Interact;
        public InputAction @Quit => m_Wrapper.m_Player_Quit;
        public InputAction @GodMode => m_Wrapper.m_Player_GodMode;
        public InputAction @SkipDialog => m_Wrapper.m_Player_SkipDialog;
        public InputActionMap Get() { return m_Wrapper.m_Player; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(PlayerActions set) { return set.Get(); }
        public void SetCallbacks(IPlayerActions instance)
        {
            if (m_Wrapper.m_PlayerActionsCallbackInterface != null)
            {
                @Walk.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnWalk;
                @Walk.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnWalk;
                @Walk.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnWalk;
                @LookAround.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnLookAround;
                @LookAround.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnLookAround;
                @LookAround.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnLookAround;
                @ProtectEyes.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnProtectEyes;
                @ProtectEyes.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnProtectEyes;
                @ProtectEyes.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnProtectEyes;
                @ProtectEars.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnProtectEars;
                @ProtectEars.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnProtectEars;
                @ProtectEars.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnProtectEars;
                @Interact.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnInteract;
                @Interact.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnInteract;
                @Interact.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnInteract;
                @Quit.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnQuit;
                @Quit.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnQuit;
                @Quit.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnQuit;
                @GodMode.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnGodMode;
                @GodMode.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnGodMode;
                @GodMode.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnGodMode;
                @SkipDialog.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnSkipDialog;
                @SkipDialog.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnSkipDialog;
                @SkipDialog.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnSkipDialog;
            }
            m_Wrapper.m_PlayerActionsCallbackInterface = instance;
            if (instance != null)
            {
                @Walk.started += instance.OnWalk;
                @Walk.performed += instance.OnWalk;
                @Walk.canceled += instance.OnWalk;
                @LookAround.started += instance.OnLookAround;
                @LookAround.performed += instance.OnLookAround;
                @LookAround.canceled += instance.OnLookAround;
                @ProtectEyes.started += instance.OnProtectEyes;
                @ProtectEyes.performed += instance.OnProtectEyes;
                @ProtectEyes.canceled += instance.OnProtectEyes;
                @ProtectEars.started += instance.OnProtectEars;
                @ProtectEars.performed += instance.OnProtectEars;
                @ProtectEars.canceled += instance.OnProtectEars;
                @Interact.started += instance.OnInteract;
                @Interact.performed += instance.OnInteract;
                @Interact.canceled += instance.OnInteract;
                @Quit.started += instance.OnQuit;
                @Quit.performed += instance.OnQuit;
                @Quit.canceled += instance.OnQuit;
                @GodMode.started += instance.OnGodMode;
                @GodMode.performed += instance.OnGodMode;
                @GodMode.canceled += instance.OnGodMode;
                @SkipDialog.started += instance.OnSkipDialog;
                @SkipDialog.performed += instance.OnSkipDialog;
                @SkipDialog.canceled += instance.OnSkipDialog;
            }
        }
    }
    public PlayerActions @Player => new PlayerActions(this);
    private int m_GamepadSchemeIndex = -1;
    public InputControlScheme GamepadScheme
    {
        get
        {
            if (m_GamepadSchemeIndex == -1) m_GamepadSchemeIndex = asset.FindControlSchemeIndex("Gamepad");
            return asset.controlSchemes[m_GamepadSchemeIndex];
        }
    }
    private int m_MouseKeyboardSchemeIndex = -1;
    public InputControlScheme MouseKeyboardScheme
    {
        get
        {
            if (m_MouseKeyboardSchemeIndex == -1) m_MouseKeyboardSchemeIndex = asset.FindControlSchemeIndex("Mouse-Keyboard");
            return asset.controlSchemes[m_MouseKeyboardSchemeIndex];
        }
    }
    public interface IPlayerActions
    {
        void OnWalk(InputAction.CallbackContext context);
        void OnLookAround(InputAction.CallbackContext context);
        void OnProtectEyes(InputAction.CallbackContext context);
        void OnProtectEars(InputAction.CallbackContext context);
        void OnInteract(InputAction.CallbackContext context);
        void OnQuit(InputAction.CallbackContext context);
        void OnGodMode(InputAction.CallbackContext context);
        void OnSkipDialog(InputAction.CallbackContext context);
    }
}
