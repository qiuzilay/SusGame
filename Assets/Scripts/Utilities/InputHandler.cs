using UnityEngine;
using UnityEngine.InputSystem;

public class InputHandler : MonoBehaviour
{
    [SerializeField] private Player _player;
    private InputAction _moveAction;
    private InputAction _lookAction;
    private InputAction _jumpAction;
    private InputAction _attackAction;
    private InputAction _interactAction;
    private InputAction _useAction;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _moveAction = InputSystem.actions.FindAction("Move");
        _lookAction = InputSystem.actions.FindAction("Look");
        _jumpAction = InputSystem.actions.FindAction("Jump");
        _attackAction = InputSystem.actions.FindAction("Attack");
        _interactAction = InputSystem.actions.FindAction("Interact");
        _useAction = InputSystem.actions.FindAction("Use");

        _moveAction?.Enable();
        _lookAction?.Enable();
        _jumpAction?.Enable();
        _attackAction?.Enable();
        _interactAction?.Enable();
        _useAction?.Enable();
        
        _interactAction.performed += OnInteractPerformed;
        _useAction.started += OnUsePressed;
        _useAction.performed += OnUsePerformed;
        _useAction.canceled += OnUseReleased;
        // _attackAction.started += OnAttackStarted;
        // _jumpAction.performed += OnJumpPerformed;

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    void OnDestroy()
    {
        if (_interactAction != null)
        {
            _interactAction.performed -= OnInteractPerformed;
            _interactAction.Disable();
        }
        if (_useAction != null)
        {
            _useAction.started -= OnUsePressed;
            _useAction.performed -= OnUsePerformed;
            _useAction.canceled -= OnUseReleased;
            _useAction.Disable();
        }
        _moveAction?.Disable();
        _lookAction?.Disable();
        _jumpAction?.Disable();
        _attackAction?.Disable();

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        // Debug.Log("Destroy!");
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
        if (_useAction.IsPressed())
        {
            _player.Use();
        }
    }

    private void OnInteractPerformed(InputAction.CallbackContext context)
    {
        _player.Interact();
    }
    private void OnUsePressed(InputAction.CallbackContext context)
    {
        _player.PressUse();
    }
    private void OnUsePerformed(InputAction.CallbackContext context)
    {
        _player.Use();
    }
    private void OnUseReleased(InputAction.CallbackContext context)
    {
        _player.ReleaseUse();
    }
    // private void OnJumpPerformed(InputAction.CallbackContext context)
    // {
    //     PlayerControl.Jump();
    // }
    private void OnAttackStarted(InputAction.CallbackContext context) // for debug used
    {
        // var gui = GameObject.Find("GUI").GetComponent<GUIManager>();
        // gui.OnGameOver(true);
    } 
}
