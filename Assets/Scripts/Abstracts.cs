using UnityEngine;
using UnityEngine.AI;
using System;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Collider))]
public abstract class HighlightableBase : MonoBehaviour, IHighlightable
{
    public class Properties : MonoBehaviour
    {
        private int _initLayer;
        private int _highlightLayer;
        
        private void Start()
        {
            _initLayer = gameObject.layer;
            _highlightLayer = LayerMask.NameToLayer("Highlight");
        }

        public void OnEnter()
        {
            gameObject.layer = _highlightLayer;
        }

        public void OnLeave()
        {
            gameObject.layer = _initLayer;
        }
    }

    protected int _initLayer;
    protected int _highlightLayer;
    protected Properties[] _children;
    protected Rigidbody _rigidbody;


    protected virtual void Start()
    {
        _initLayer = gameObject.layer;
        _highlightLayer = LayerMask.NameToLayer("Highlight");
        
        _rigidbody = GetComponent<Rigidbody>();
        _rigidbody.useGravity = true;
        _rigidbody.isKinematic = false;

        Transform[] children = transform.GetComponentsInChildren<Transform>();
        Array.Resize(ref _children, children.Length);
        for (int i = 0; i < children.Length; i++)
        {
            _children[i] = children[i].gameObject.AddComponent<Properties>();
        }
    }

    public virtual void OnEnterFocus()
    {
        // Debug.Log(transform.name + ": " + "Enter focus");
        gameObject.layer = _highlightLayer;
        foreach (var child in _children)
        {
            child.OnEnter();
        }
    }

    public virtual void OnLeaveFocus()
    {
        // Debug.Log(transform.name + ": " + "Leave focus");
        gameObject.layer = _initLayer;
        foreach (var child in _children)
        {
            child.OnLeave();
        }
    }
}

public abstract class InteractableBase : HighlightableBase, IInteractable
{
    [SerializeField]
    protected string _hintFailed;
    [SerializeField]
    protected string _hintSuccessed;

    public abstract string OnInteract(Transform item);
}

public abstract class PickableBase : HighlightableBase, IPickable
{

    public virtual void OnPick()
    {
        _rigidbody.useGravity = false;
        _rigidbody.constraints = RigidbodyConstraints.FreezeRotation;
    }

    public abstract void OnUse();
    
    public virtual void OnDrop()
    {
        _rigidbody.useGravity = true;
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
    [Header("Behaviours")]
    [SerializeField][Range(0f, 1f)]
    private float _jumpBeforeDistance = .5f;
    [SerializeField][Range(0f, 10f)]
    private float _rotationDamping = 5f;
    [SerializeField][Range(0f, 5f)]
    private float _giveUpTime = 3f;

    private NavMeshAgent _agent;
    private Vector3 _bias;
    private float _timer;
    protected Vector3 DesiredVelocity => _agent.desiredVelocity;
    protected Transform Target { get; set; }
    protected StateBase CurrState { get; private set; }
    protected StateBase PrevState { get; set; }
    protected StateBase NextState { get; set; }

    public bool IsTimerWorking { get; private set; } = false;
    public bool IsTracking => _agent.pathPending || _agent.desiredVelocity.sqrMagnitude >= 0.01f;
    
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

        if (IsTimerWorking && (Time.time - _timer >= _giveUpTime))
        {
            Target = null;
            IsTimerWorking = false;
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

        // Debug.Log("remainingDistance: " + _agent.remainingDistance);

        if (_agent.pathPending || _agent.remainingDistance > _agent.stoppingDistance)
        {
            Vector2 direction;
            {
                Vector3 localDirection = transform.InverseTransformDirection(_agent.desiredVelocity.normalized);
                direction.x = localDirection.x;
                direction.y = localDirection.z;
            }
            
            // Debug.Log("direction: " + direction);
            Move(direction);

            if (Physics.CapsuleCast(transform.position, transform.position + (Vector3.up * 2), .5f, transform.forward, _jumpBeforeDistance))
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

    protected bool FaceTo(Vector3 direction, float stopThreshold = 1f)
    {
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
            if (hit.transform == otherTransform && otherTransform.name == "Player")
            {
                return true;
            }
        }
        return false;
    }
    private void OnTriggerStay(Collider other) {
        if ((IsTimerWorking || !IsAngry) && Inspect(other.transform))
        {
            Target = other.transform;
            IsTimerWorking = false;
            // Debug.Log("Target: " + Target.name);
        }
    }
    private void OnTriggerExit(Collider other) {
        _timer = Time.time;
        IsTimerWorking = true;
    }
}