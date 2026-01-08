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
    [SerializeField] private float _walkSpeed = 3f;
    [SerializeField] private float _sprintSpeed = 5f;
    [SerializeField] private float _rotationSpeed = 25f;
    [SerializeField] private float _jumpForce = 8f;
    [SerializeField] private float _doubleTapTimeThreshold = .5f;
    private const float _gravity = -30f;

    [Header("Camera")]
    [SerializeField] private float PitchAngle = 60;

    private CharacterAnimation _characterAnimation;
    private MovementStatus _movementStatus;
    private float _lastestTapTime = 0f;
    private Vector3 _velocity = new (0, _gravity, 0);
    private float _rotationX = 0f;
    private float _rotationY = 0f;
    protected Transform Anchor { get; set; }
    protected CharacterController Controller { get; set; }

    public bool IsGrounded => Controller.isGrounded;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    protected virtual void Start()
    {
        _characterAnimation = GetComponent<CharacterAnimation>();
        Controller = GetComponent<CharacterController>();
        Anchor = transform.Find("Anchor");
    }

    public void Move(Vector2 direction)
    {
        if (Controller.isGrounded)
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
                        if (diff <= _doubleTapTimeThreshold && direction == Vector2.up)
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
                    horizontalMove *= _walkSpeed;
                    break;

                case MovementStatus.Sprinting:
                    if (direction.x == 0 && direction.y == 0)
                    {
                        _characterAnimation.Idle();
                        _movementStatus = MovementStatus.Idleling;
                    }
                    horizontalMove *= _sprintSpeed;
                    break;
            }
            
            _velocity.x = horizontalMove.x;
            _velocity.y = Mathf.Max(_velocity.y, -2);
            _velocity.z = horizontalMove.z;
        }
        else
        {
            // velocity (vertical) = acceleration (_gravity) * time;
            _velocity.y += _gravity * Time.deltaTime;
            if (direction.x == 0 && direction.y == 0)
            {
                _characterAnimation.Idle();
                _movementStatus = MovementStatus.Idleling;
            }
            else if (_movementStatus != MovementStatus.Sprinting)
            {
                _characterAnimation.Walk();
                _movementStatus = MovementStatus.Walking;
            }
        }

        // displacement = velocity * time
        Vector3 displacement = _velocity * Time.deltaTime;
        // Debug.Log("Displacement: " + displacement);
        Controller.Move(displacement);
    }

    public void Rotate(Vector2 rotationVector)
    {
        // Debug.Log(rotationVector);
        rotationVector *= _rotationSpeed * Time.deltaTime;
        _rotationY += rotationVector.x;
        transform.localRotation = Quaternion.Euler(0, _rotationY, 0);
        if (Mathf.Abs(_rotationX + rotationVector.y) <= PitchAngle)
        {
            _rotationX += rotationVector.y;
            Anchor.localRotation = Quaternion.Euler(_rotationX, 0, 0);
        }
    }

    public void Jump()
    {
        if (Controller.isGrounded)
        {
            _velocity.y = _jumpForce;
        }
    }
}
