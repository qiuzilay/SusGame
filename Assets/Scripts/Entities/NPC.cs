using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class NPC : CharacterMovement
{
    [SerializeField][Range(0f, 1f)]
    private float _jumpBeforeDistance = .5f;

    [SerializeField]
    private Transform _target;
    private Transform _scanner;
    private NavMeshAgent _agent;
    private Vector3 _bias;

    public bool IsAngry
    {
        get { return _target != null; }
    }

    sealed protected override void Start()
    {
        base.Start();
        _scanner = transform.Find("Anchor").Find("Scanner").transform;
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
        // Debug.DrawRay(transform.position + transform.TransformDirection(_bias), transform.forward, Color.red);
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

    private void Trace()
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

            if (_agent.desiredVelocity.sqrMagnitude > .25f)
            {
                Vector2 rotation;
                rotation.x = Vector3.SignedAngle(transform.forward, _agent.desiredVelocity, Vector3.up);
                rotation.y = 0;
                Rotate(rotation);
            }
        }
        else
        {
            Move(Vector2.zero);
        }
    }

    public void OnTriggerEnter(Collider other) {
        Debug.Log("Enter!");
    }

    public void OnTriggerStay(Collider other) {
        Debug.Log("Stay!");
    }

    public void OnTriggerExit(Collider other) {
        Debug.Log("Exit!");
    }
}
