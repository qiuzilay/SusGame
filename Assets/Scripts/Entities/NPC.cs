using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class NPC : CharacterMovement
{
    [SerializeField][Range(0f, 1f)]
    private float _jumpBeforeDistance = .5f;

    private Transform _target;
    private NavMeshAgent _agent;
    private Vector3 _bias;

    [SerializeField][Range(0, 5)]
    private float _giveUpTime = 2f;
    private bool _isTimerWorking = false;
    private float _timer;

    public bool IsAngry
    {
        get { return _target != null; }
    }

    sealed protected override void Start()
    {
        base.Start();
        _agent = GetComponent<NavMeshAgent>();
        _agent.updatePosition = false;
        _agent.updateRotation = false;
        _bias.x = _bias.y = 0f;
        _bias.z = _characterControl.radius;
    }

    private void Update()
    {
        _agent.nextPosition = transform.position;
        Decide();

        if (_isTimerWorking && (Time.time - _timer >= _giveUpTime))
        {
            _target = null;
            _isTimerWorking = false;
            // Debug.Log("Exit!");
        }
    }

    private void Decide()
    {
        if (IsAngry)
        {
            Trace();
        }
        else
        {
            Move(Vector2.zero);
        }
    }

    public void Trace()
    {
        _agent.SetDestination(_target.position);

        if (_agent.remainingDistance > _agent.stoppingDistance)
        {
            Vector2 direction;
            {
                Vector3 localDirection = transform.InverseTransformDirection(_agent.desiredVelocity.normalized);
                direction.x = localDirection.x;
                direction.y = localDirection.z;
            }
            
            Move(direction);
            // Debug.Log(_agent.remainingDistance);

            if (Physics.Raycast(transform.position + transform.TransformDirection(_bias), transform.forward, _jumpBeforeDistance))
            {
                if (IsGrounded)
                {
                    Jump();
                }
            }
        }
        else
        {
            Move(Vector2.zero);
        }

        float angle = Vector3.SignedAngle(transform.forward, _agent.desiredVelocity, Vector3.up);
        if (angle < -5 || 5 < angle)
        {
            Vector2 rotation;
            rotation.x = angle;
            rotation.y = 0;
            Rotate(rotation);
        }
    }

    private bool Inspect(Transform otherTransform)
    {
        Vector3 origin = _anchor.position;
        Vector3 direction = otherTransform.position - transform.position;
        float maxDistance = direction.magnitude;
        Debug.DrawRay(origin, direction * maxDistance, Color.red);
        if (Physics.Raycast(origin, direction, out RaycastHit hit, maxDistance))
        {
            if (hit.transform == otherTransform)
            {
                return true;
            }
        }
        return false;
    }

    private void OnTriggerEnter(Collider other)
    {
        // nothing
    }

    private void OnTriggerStay(Collider other) {
        if (_target == null && Inspect(other.transform))
        {
            _target = other.transform;
            _isTimerWorking = false;
            // Debug.Log("Enter!");
        }
    }

    private void OnTriggerExit(Collider other) {
        _isTimerWorking = true;
        _timer = Time.time;
    }
}
