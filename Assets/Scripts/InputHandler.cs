using UnityEngine;
using UnityEngine.InputSystem;

public class InputHandler : MonoBehaviour
{
    public PlayerController PlayerControl;
    private InputAction _moveAction, _lookAction, _jumpAction;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _moveAction = InputSystem.actions.FindAction("Move");
        _lookAction = InputSystem.actions.FindAction("Look");
        _jumpAction = InputSystem.actions.FindAction("Jump");
        
        _jumpAction.performed += OnJumpPerformed;

        Cursor.visible = false;
    }

    // Update is called once per frame
    void Update()
    {
        PlayerControl.Move(_moveAction.ReadValue<Vector2>());
        PlayerControl.Rotate(_lookAction.ReadValue<Vector2>());
    }

    private void OnJumpPerformed(InputAction.CallbackContext context)
    {
        PlayerControl.Jump();
    }
}
