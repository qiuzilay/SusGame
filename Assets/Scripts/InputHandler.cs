using UnityEngine;
using UnityEngine.InputSystem;

public class InputHandler : MonoBehaviour
{
    [SerializeField] private Player _player;
    private InputAction _moveAction;
    private InputAction _lookAction;
    private InputAction _jumpAction;
    private InputAction _attackAction;
    private InputAction _useAction;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _moveAction = InputSystem.actions.FindAction("Move");
        _lookAction = InputSystem.actions.FindAction("Look");
        _jumpAction = InputSystem.actions.FindAction("Jump");
        _attackAction = InputSystem.actions.FindAction("Attack");
        _useAction = InputSystem.actions.FindAction("Use");
        
        _useAction.performed += OnUsePerformed;
        // _jumpAction.performed += OnJumpPerformed;

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
        _player.Move(_moveAction.ReadValue<Vector2>());
        _player.Rotate(_lookAction.ReadValue<Vector2>());
        if (_jumpAction.IsPressed())
        {
            _player.Jump();
        }
    }

    private void OnUsePerformed(InputAction.CallbackContext context)
    {
        _player.Use();
    }

    // private void OnJumpPerformed(InputAction.CallbackContext context)
    // {
    //     PlayerControl.Jump();
    // }
}
