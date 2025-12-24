using UnityEngine;
using UnityEngine.InputSystem;

public class InputHandler : MonoBehaviour
{
    public PlayerController PlayerControl;
    private InputAction _moveAction, _lookAction;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _moveAction = InputSystem.actions.FindAction("Move");
        _lookAction = InputSystem.actions.FindAction("Look");
        Cursor.visible = false;
    }

    // Update is called once per frame
    void Update()
    {
        PlayerControl.Move(_moveAction.ReadValue<Vector2>());
        PlayerControl.Rotate(_lookAction.ReadValue<Vector2>());
    }
}
