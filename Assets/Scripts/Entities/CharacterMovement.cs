using UnityEngine;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(CharacterAnimation))]
public class CharacterMovement : MonoBehaviour
{
    public enum MovementStatus
    {
        Idleling,
        Walking,
        Sprinting
    };
    
    [Header("Movement")]
    [SerializeField] private float WalkSpeed = 3f;
    [SerializeField] private float SprintSpeed = 5f;
    [SerializeField] private float RotationSpeed = 25f;
    [SerializeField] private float JumpForce = 8f;
    [SerializeField] private const float Gravity = -30f;
    [SerializeField] private float DoubleTapTimeThreshold = .5f;

    [Header("Camera")]
    [SerializeField] private float PitchAngle = 60;

    private CharacterAnimation _characterAnimation;
    protected CharacterController _characterControl;
    private MovementStatus _movementStatus;
    private float _lastestTapTime = 0f;
    private Vector3 _velocity = new (0, Gravity, 0);
    private float _rotationX = 0f;
    private float _rotationY = 0f;
    protected Transform _anchor;
    public bool IsGrounded => _characterControl.isGrounded;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    protected virtual void Start()
    {
        _characterAnimation = GetComponent<CharacterAnimation>();
        _characterControl = GetComponent<CharacterController>();
        _anchor = transform.Find("Anchor");
    }

    public void Move(Vector2 direction)
    {
        if (_characterControl.isGrounded)
        {
            // velocity (horizontal) = movement speed (at below) * direction vector
            Vector3 horizontalMove = transform.forward * direction.y + transform.right * direction.x;
            
            switch (_movementStatus)
            {
                case MovementStatus.Idleling:
                    if (direction.x != 0 || direction.y != 0)
                    {
                        float diff = Time.time - _lastestTapTime;
                        _lastestTapTime = Time.time;
                        if (diff <= DoubleTapTimeThreshold && direction == Vector2.up)
                        {
                            _movementStatus = MovementStatus.Sprinting;
                            _characterAnimation.Sprint();
                            goto case MovementStatus.Sprinting;
                        }
                        else
                        {
                            _movementStatus = MovementStatus.Walking;
                            _characterAnimation.Walk();
                            goto case MovementStatus.Walking;
                        }
                    }
                    horizontalMove *= 0;
                    break;

                case MovementStatus.Walking:
                    if (direction.x == 0 && direction.y == 0)
                    {
                        _characterAnimation.Idle();
                        _movementStatus = MovementStatus.Idleling;
                        goto case MovementStatus.Idleling;
                    }
                    horizontalMove *= WalkSpeed;
                    break;

                case MovementStatus.Sprinting:
                    if (direction.x == 0 && direction.y == 0)
                    {
                        _characterAnimation.Idle();
                        _movementStatus = MovementStatus.Idleling;
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
        // Debug.Log(rotationVector);
        rotationVector *= RotationSpeed * Time.deltaTime;
        _rotationY += rotationVector.x;
        transform.localRotation = Quaternion.Euler(0, _rotationY, 0);
        if (Mathf.Abs(_rotationX + rotationVector.y) <= PitchAngle)
        {
            _rotationX += rotationVector.y;
            _anchor.localRotation = Quaternion.Euler(_rotationX, 0, 0);
        }
    }

    public void Jump()
    {
        if (_characterControl.isGrounded)
        {
            _velocity.y = JumpForce;
        }
    }
}
