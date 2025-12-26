using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public enum MovementStatus
    {
        Standing,
        Walking,
        Sprinting
    };

    private CharacterController _characterControl;
    public float WalkSpeed = 3f;
    public float SprintSpeed = 10f;
    public float RotationSpeed = 60f;
    public float JumpForce = 8f;
    static public float Gravity = -30f;

    private MovementStatus _movementStatus;
    private const float _doubleTapTimeThreshold = .5f;
    private float _lastestTapTime = 0f;
    private float _rotationY = 0f;
    private Vector3 _velocity = new (0, Gravity, 0);

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _characterControl = GetComponent<CharacterController>();
    }

    public void Move(Vector2 direction)
    {
        if (_characterControl.isGrounded)
        {
            // velocity (horizontal) = movement speed * direction vector
            Vector3 horizontalMove = transform.forward * direction.y + transform.right * direction.x;
            
            Debug.Log("_lastestTapTime: " + _lastestTapTime + ", current_time: " + Time.time);
            
            switch (_movementStatus)
            {
                case MovementStatus.Standing:
                    if (direction.x > 0 || direction.y > 0)
                    {
                        if (Time.time - _lastestTapTime <= _doubleTapTimeThreshold)
                        {
                            _movementStatus = MovementStatus.Sprinting;
                        }
                        else
                        {
                            _movementStatus = MovementStatus.Walking;
                        }
                        
                        _lastestTapTime = Time.time;
                    }
                    horizontalMove *= 0;
                    break;

                case MovementStatus.Walking:
                    if (direction.x == 0 && direction.y == 0)
                    {
                        _movementStatus = MovementStatus.Standing;
                    }
                    horizontalMove *= WalkSpeed;
                    break;

                case MovementStatus.Sprinting:
                    if (direction.x == 0 && direction.y == 0)
                    {
                        _movementStatus = MovementStatus.Standing;
                    }
                    horizontalMove *= SprintSpeed;
                    break;
            }
            
            _velocity.x = horizontalMove.x;
            _velocity.y = Mathf.Max(_velocity.y, -2);
            _velocity.z = horizontalMove.z;
        }
        else
        {
            // velocity (vertical) = acceleration (gravity) * time;
            _velocity.y += Gravity * Time.deltaTime;
        }

        // displacement = velocity * time
        Vector3 displacement = _velocity * Time.deltaTime;
        // Debug.Log("Displacement: " + displacement);
        _characterControl.Move(displacement);
    }

    public void Rotate(Vector2 rotationVector)
    {
        _rotationY += rotationVector.x * RotationSpeed * Time.deltaTime;
        transform.localRotation = Quaternion.Euler(0, _rotationY, 0);
    }

    public void Jump()
    {
        if (_characterControl.isGrounded)
        {
            _velocity.y = JumpForce;
        }
    }
}
