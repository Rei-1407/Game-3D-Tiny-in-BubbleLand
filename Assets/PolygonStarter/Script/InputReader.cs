using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using static PlayerInputActions;

namespace Platformer {
    [CreateAssetMenu(fileName = "InputReader", menuName = "Platformer/InputReader")]
    public class InputReader : ScriptableObject, IPlayerActions {
        public event UnityAction<Vector2> Move = delegate { };
        public event UnityAction<Vector2, bool> Look = delegate { };
        public event UnityAction EnableMouseControlCamera = delegate { };
        public event UnityAction DisableMouseControlCamera = delegate { };
        public event UnityAction<bool> Jump = delegate { };
        public event UnityAction<bool> Dash = delegate { };
        public event UnityAction Throw = delegate { };  
        public event UnityAction MouseLeftClick= delegate { };
        public event UnityAction HandPistol=delegate { };
        public event UnityAction<bool> Attack = delegate { };
        PlayerInputActions inputActions;
        
        public Vector3 Direction => inputActions.Player.Move.ReadValue<Vector2>();

        void OnEnable() {
            if (inputActions == null) {
                inputActions = new PlayerInputActions();
                inputActions.Player.SetCallbacks(this);
                Debug.Log("InputReader enabled");
            }
        }
        
        public void EnablePlayerActions() {
            inputActions.Enable();
        }

        public void OnLook(InputAction.CallbackContext context) {
            Look.Invoke(context.ReadValue<Vector2>(), IsDeviceMouse(context));
            Debug.Log(context.ReadValue<Vector2>());
        }

        bool IsDeviceMouse(InputAction.CallbackContext context) => context.control.device.name == "Mouse";
        public void OnMouseThrow(InputAction.CallbackContext context)
        {
            switch (context.phase) {
                case InputActionPhase.Started:
                    Debug.Log("Mouse Left Click");
                    break;
                case InputActionPhase.Canceled:
                    Debug.Log("Mouse Left Click Canceled"); 
                    break;
            }
        }

        void IPlayerActions.OnMove(InputAction.CallbackContext context)
        {
            Move.Invoke(context.ReadValue<Vector2>());
        }

        void IPlayerActions.OnLook(InputAction.CallbackContext context)
        {
            Look.Invoke(context.ReadValue<Vector2>(), IsDeviceMouse(context));
        }

        void IPlayerActions.OnFire(InputAction.CallbackContext context)
        {
            // if (context.phase == InputActionPhase.Started) {
            //     Attack.Invoke(true);
            // }
            // else if(context.phase == InputActionPhase.Canceled) {
            //     Attack.Invoke(false);
            // }
        }

        void IPlayerActions.OnThrow(InputAction.CallbackContext context)
        {
            if(context.phase == InputActionPhase.Started) {
                Throw.Invoke(); 
            }  
        }

        void IPlayerActions.OnJump(InputAction.CallbackContext context)
        {
            switch (context.phase) {
                case InputActionPhase.Started:
                Debug.LogError("ON JUMP");
                    Jump.Invoke(true);
                    break;
                case InputActionPhase.Canceled:
                    Jump.Invoke(false);
                    break;
            }
        }

        public void OnPistol1(InputAction.CallbackContext context)
        {
            if(context.phase == InputActionPhase.Started){
                HandPistol.Invoke();

            }
        }

        public void OnAttack(InputAction.CallbackContext context)
        {
            if (context.phase == InputActionPhase.Started) {
                Attack.Invoke(true);
            }
            else if(context.phase == InputActionPhase.Canceled) {
                Attack.Invoke(false);
            }
        }
    }
}