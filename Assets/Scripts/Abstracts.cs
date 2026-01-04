using UnityEngine;
using UnityEngine.AI;

public abstract class InteractableBase : MonoBehaviour, IInteractable
{
    protected int __init_Layer;
    protected int _highlightLayer;

    protected virtual void Start()
    {
        __init_Layer = gameObject.layer;
        _highlightLayer = LayerMask.NameToLayer("Highlight");
    }

    public virtual void OnEnterFocus()
    {
        // Debug.Log(transform.name + ": " + "Enter focus");
        gameObject.layer = _highlightLayer;
    }

    public abstract void OnInteract();

    public virtual void OnLeaveFocus()
    {
        // Debug.Log(transform.name + ": " + "Leave focus");
        gameObject.layer = __init_Layer;
    }
}

[RequireComponent(typeof(Rigidbody))]
public abstract class PickableBase : InteractableBase, IPickable
{
    private bool __init_useGravity;
    protected Rigidbody _rigidbody;

    protected override void Start()
    {
        base.Start();
        _rigidbody = GetComponent<Rigidbody>();
        __init_useGravity = _rigidbody.useGravity;
    }

    public virtual void OnPick()
    {
        _rigidbody.useGravity = false;
        _rigidbody.constraints = RigidbodyConstraints.FreezeRotation;
    }

    public abstract void OnUse();
    
    public virtual void OnDrop()
    {
        _rigidbody.useGravity = __init_useGravity;
        _rigidbody.constraints = RigidbodyConstraints.None;
    }
}

public abstract class StateBase : IState
{
    public float StartTime { get; protected set; }
    public float RunTime => Time.time - StartTime;

    public virtual void OnEnter()
    {
        StartTime = Time.time;
    }
    public virtual void Trigger() {}
    public virtual void OnLeave() {}
}

[RequireComponent(typeof(NavMeshAgent))]
public abstract class NPCBase : CharacterMovement
{
    [Header("Movements")]
    [SerializeField][Range(0f, 1f)]
    private float _jumpBeforeDistance = .5f;
    [SerializeField][Range(0f, 10f)]
    private float _rotationDamping = 5f;

    [Header("Behaviours")]
    [SerializeField][Range(0f, 5f)]
    private float _giveUpTime = 1.5f;

    private NavMeshAgent _agent;
    private Vector3 _bias;
    private bool _isTimerWorking = false;
    private float _timer;
    protected Transform Target { get; set; }
    protected StateBase CurrState { get; private set; }
    protected StateBase NextState { get; set; }
    
    public bool IsAngry
    {
        get { return Target != null; }
    }

    protected override void Start()
    {
        base.Start();
        _agent = GetComponent<NavMeshAgent>();
        _agent.updatePosition = false;
        _agent.updateRotation = false;
        _bias.x = _bias.y = 0f;
        _bias.z = Controller.radius;
    }

    protected virtual void Update()
    {
        _agent.nextPosition = transform.position;

        if (_isTimerWorking && (Time.time - _timer >= _giveUpTime))
        {
            Target = null;
            _isTimerWorking = false;
            // Debug.Log("Exit!");
        }

        if (NextState != null)
        {
            UpdateState();
        }
        CurrState?.Trigger();
    }

    private void UpdateState()
    {
        CurrState?.OnLeave();
        CurrState = NextState;
        NextState = null;
        CurrState?.OnEnter();
    }

    protected bool MoveTo(Vector3 position)
    {
        _agent.SetDestination(position);

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
            return false;
        }
        else
        {
            Move(Vector2.zero);
            return true;
        }
    }

    protected bool FaceTo(Vector3 position, float stopThreshold = 1f)
    {
        Vector3 direction = position - transform.position;
        direction.y = 0;

        if (direction.sqrMagnitude > 0.01f)
        {
            direction.Normalize();
            
            float angle = Vector3.SignedAngle(transform.forward, direction, Vector3.up);
            
            if (Mathf.Abs(angle) > stopThreshold)
            {
                Vector2 rotation;
                rotation.y = 0;
                rotation.x = Mathf.Clamp(angle / _rotationDamping, -5, 5);
                Rotate(rotation);
                return false;
            }
            return true;
        }
        return true;
    }

    private bool Inspect(Transform otherTransform)
    {
        Vector3 origin = Anchor.position;
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
    private void OnTriggerStay(Collider other) {
        if (Target == null && Inspect(other.transform))
        {
            Target = other.transform;
            _isTimerWorking = false;
            // Debug.Log("Target: " + Target.name);
        }
    }
    private void OnTriggerExit(Collider other) {
        _isTimerWorking = true;
        _timer = Time.time;
    }
}