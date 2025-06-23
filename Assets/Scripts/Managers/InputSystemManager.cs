using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputSystemManager : MonoSingleton<InputSystemManager>
{
    public struct InputInfo
    {
        public float MoveAxis;
        public float RotationAxis;
        public Vector2 MousePosition;
        public bool AttackDown;
        public bool AttackUp;
        public bool AttackHold;
    }
    private InputSystem_Actions inputActions;
    private float moveInput;
    private float rotationInput;
    private Vector2 mousePosition;
    public InputInfo CurrentInputInfo { get; private set; }
    protected override void Awake()
    {
        base.Awake();
        inputActions = new InputSystem_Actions();
        inputActions.Player.TankMove.performed += ctx => moveInput = ctx.ReadValue<float>();
        inputActions.Player.TankMove.canceled += ctx => moveInput = 0f;
        inputActions.Player.TankRotate.performed += ctx => rotationInput = ctx.ReadValue<float>();
        inputActions.Player.TankRotate.canceled += ctx => rotationInput = 0f;
        inputActions.Player.TankAim.performed += ctx => mousePosition = ctx.ReadValue<Vector2>();
        inputActions.Player.TankAim.canceled += ctx => mousePosition = Vector2.zero;
    }
    private void OnEnable()
    {
        inputActions.Player.Enable();
    }

    private void OnDisable()
    {
        inputActions.Player.Disable();
    }

    private void Update()
    {
        HandleCharacterInput();
        Debug.Log(CurrentInputInfo.MousePosition);
    }

    private void HandleCharacterInput()
    {
        var inputs = new InputInfo
        {
            MoveAxis = moveInput,
            RotationAxis = rotationInput,
            MousePosition = mousePosition,
            AttackDown = inputActions.Player.Attack.WasPressedThisFrame(),
            AttackUp = inputActions.Player.Attack.WasReleasedThisFrame(),
            AttackHold = inputActions.Player.Attack.IsPressed(),
        };
        CurrentInputInfo = inputs;
    }
}
