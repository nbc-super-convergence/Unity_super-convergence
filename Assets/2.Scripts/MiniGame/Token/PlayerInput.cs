//------------------------------------------------------------------------------
// <auto-generated>
//     This code was auto-generated by com.unity.inputsystem:InputActionCodeGenerator
//     version 1.7.0
//     from Assets/AddressableDatas/Texture/InputSystem/PlayerInput.inputactions
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

public partial class @PlayerInput: IInputActionCollection2, IDisposable
{
    public InputActionAsset asset { get; }
    public @PlayerInput()
    {
        asset = InputActionAsset.FromJson(@"{
    ""name"": ""PlayerInput"",
    ""maps"": [
        {
            ""name"": ""MiniPlayerToken"",
            ""id"": ""6749c88b-81ee-4fea-bc76-6a2a4fe1b7a2"",
            ""actions"": [
                {
                    ""name"": ""Move"",
                    ""type"": ""Value"",
                    ""id"": ""3553aaf1-c350-4ab3-bff2-3e0f64518203"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": true
                },
                {
                    ""name"": ""Jump"",
                    ""type"": ""Value"",
                    ""id"": ""2a6e15a7-7a6b-43d7-9ba6-73cd485e723d"",
                    ""expectedControlType"": """",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": true
                },
                {
                    ""name"": ""Interaction"",
                    ""type"": ""Value"",
                    ""id"": ""fdeef296-2f5d-4b56-801c-7cf2387a826e"",
                    ""expectedControlType"": """",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": true
                }
            ],
            ""bindings"": [
                {
                    ""name"": ""2D Vector"",
                    ""id"": ""f8bda928-de5c-4f46-a25a-e5cdff120db7"",
                    ""path"": ""2DVector(mode=2)"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""up"",
                    ""id"": ""596bc4f2-206a-4085-9f9a-215f1476e6de"",
                    ""path"": ""<Keyboard>/w"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard_Mouse"",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""down"",
                    ""id"": ""510c24ea-0a59-4cdc-a112-4711f6344009"",
                    ""path"": ""<Keyboard>/s"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard_Mouse"",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""left"",
                    ""id"": ""ff9fa9a9-b622-482a-990a-7153c576064a"",
                    ""path"": ""<Keyboard>/a"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard_Mouse"",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""right"",
                    ""id"": ""43f1ae34-5223-4964-883d-c130e9c438b8"",
                    ""path"": ""<Keyboard>/d"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard_Mouse"",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": """",
                    ""id"": ""a5895efb-94eb-4b05-ab7b-f1e467b3d26e"",
                    ""path"": ""<Keyboard>/space"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard_Mouse"",
                    ""action"": ""Jump"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""2f796db7-1efc-43c9-bb8f-45e92e6ad1f1"",
                    ""path"": ""<Mouse>/leftButton"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard_Mouse"",
                    ""action"": ""Interaction"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        }
    ],
    ""controlSchemes"": [
        {
            ""name"": ""Keyboard_Mouse"",
            ""bindingGroup"": ""Keyboard_Mouse"",
            ""devices"": [
                {
                    ""devicePath"": ""<Keyboard>"",
                    ""isOptional"": false,
                    ""isOR"": false
                },
                {
                    ""devicePath"": ""<Mouse>"",
                    ""isOptional"": false,
                    ""isOR"": false
                }
            ]
        }
    ]
}");
        // MiniPlayerToken
        m_MiniPlayerToken = asset.FindActionMap("MiniPlayerToken", throwIfNotFound: true);
        m_MiniPlayerToken_Move = m_MiniPlayerToken.FindAction("Move", throwIfNotFound: true);
        m_MiniPlayerToken_Jump = m_MiniPlayerToken.FindAction("Jump", throwIfNotFound: true);
        m_MiniPlayerToken_Interaction = m_MiniPlayerToken.FindAction("Interaction", throwIfNotFound: true);
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

    // MiniPlayerToken
    private readonly InputActionMap m_MiniPlayerToken;
    private List<IMiniPlayerTokenActions> m_MiniPlayerTokenActionsCallbackInterfaces = new List<IMiniPlayerTokenActions>();
    private readonly InputAction m_MiniPlayerToken_Move;
    private readonly InputAction m_MiniPlayerToken_Jump;
    private readonly InputAction m_MiniPlayerToken_Interaction;
    public struct MiniPlayerTokenActions
    {
        private @PlayerInput m_Wrapper;
        public MiniPlayerTokenActions(@PlayerInput wrapper) { m_Wrapper = wrapper; }
        public InputAction @Move => m_Wrapper.m_MiniPlayerToken_Move;
        public InputAction @Jump => m_Wrapper.m_MiniPlayerToken_Jump;
        public InputAction @Interaction => m_Wrapper.m_MiniPlayerToken_Interaction;
        public InputActionMap Get() { return m_Wrapper.m_MiniPlayerToken; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(MiniPlayerTokenActions set) { return set.Get(); }
        public void AddCallbacks(IMiniPlayerTokenActions instance)
        {
            if (instance == null || m_Wrapper.m_MiniPlayerTokenActionsCallbackInterfaces.Contains(instance)) return;
            m_Wrapper.m_MiniPlayerTokenActionsCallbackInterfaces.Add(instance);
            @Move.started += instance.OnMove;
            @Move.performed += instance.OnMove;
            @Move.canceled += instance.OnMove;
            @Jump.started += instance.OnJump;
            @Jump.performed += instance.OnJump;
            @Jump.canceled += instance.OnJump;
            @Interaction.started += instance.OnInteraction;
            @Interaction.performed += instance.OnInteraction;
            @Interaction.canceled += instance.OnInteraction;
        }

        private void UnregisterCallbacks(IMiniPlayerTokenActions instance)
        {
            @Move.started -= instance.OnMove;
            @Move.performed -= instance.OnMove;
            @Move.canceled -= instance.OnMove;
            @Jump.started -= instance.OnJump;
            @Jump.performed -= instance.OnJump;
            @Jump.canceled -= instance.OnJump;
            @Interaction.started -= instance.OnInteraction;
            @Interaction.performed -= instance.OnInteraction;
            @Interaction.canceled -= instance.OnInteraction;
        }

        public void RemoveCallbacks(IMiniPlayerTokenActions instance)
        {
            if (m_Wrapper.m_MiniPlayerTokenActionsCallbackInterfaces.Remove(instance))
                UnregisterCallbacks(instance);
        }

        public void SetCallbacks(IMiniPlayerTokenActions instance)
        {
            foreach (var item in m_Wrapper.m_MiniPlayerTokenActionsCallbackInterfaces)
                UnregisterCallbacks(item);
            m_Wrapper.m_MiniPlayerTokenActionsCallbackInterfaces.Clear();
            AddCallbacks(instance);
        }
    }
    public MiniPlayerTokenActions @MiniPlayerToken => new MiniPlayerTokenActions(this);
    private int m_Keyboard_MouseSchemeIndex = -1;
    public InputControlScheme Keyboard_MouseScheme
    {
        get
        {
            if (m_Keyboard_MouseSchemeIndex == -1) m_Keyboard_MouseSchemeIndex = asset.FindControlSchemeIndex("Keyboard_Mouse");
            return asset.controlSchemes[m_Keyboard_MouseSchemeIndex];
        }
    }
    public interface IMiniPlayerTokenActions
    {
        void OnMove(InputAction.CallbackContext context);
        void OnJump(InputAction.CallbackContext context);
        void OnInteraction(InputAction.CallbackContext context);
    }
}