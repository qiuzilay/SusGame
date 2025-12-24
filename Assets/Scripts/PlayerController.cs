using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private CharacterController _characterControl;
    public float MovementSpeed = 10f;
    public float RotationSpeed = 60f;
    private float _rotationY = 0f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _characterControl = GetComponent<CharacterController>();
    }

    public void Move(Vector2 movementVector)
    {
        Vector3 move = transform.forward * movementVector.y + transform.right * movementVector.x;
        Debug.Log("Move: " + move);
        Debug.Log("transform: " + transform.forward);
        _characterControl.Move(MovementSpeed * Time.deltaTime * move);
    }

    public void Rotate(Vector2 rotationVector)
    {
        _rotationY += rotationVector.x * RotationSpeed * Time.deltaTime;
        transform.localRotation = Quaternion.Euler(0, _rotationY, 0);
    }
}
