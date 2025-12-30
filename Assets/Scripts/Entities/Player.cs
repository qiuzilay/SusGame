using UnityEngine;

public class Player : CharacterMovement
{
    struct HoldingItemInfo
    {
        public Transform parent;
        public LayerMask layer;
        public float linearDamping;
    }

    [Header("Controls")]
    [SerializeField][Range(0f, 8f)]
    private float _maxInteractDistance = 4f;
    [SerializeField][Range(0f, 8f)]
    private float _maxLiftDistance = 2f;
    [SerializeField]
    private LayerMask _mask;
    
    [Header("Appearances")]
    [SerializeField][Range(-.5f, .5f)]
    private float _liftX = 0f;
    [SerializeField][Range(0f, 2f)]
    private float _liftY = 1f;
    [SerializeField][Range(.5f, 1.5f)]
    private float _liftZ = 1f;
    [SerializeField][Range(0f, 40f)]
    private float _liftForce = 10f;

    private LayerMask _ignoreRaycast;
    private Vector3 _liftCenter = new (); 
    private HoldingItemInfo _temp = new ();
    private Transform _holding;
    private Rigidbody _holdingRB;
    private Transform _aiming;
    private RaycastHit _hit;

    public bool IsHolding {
        get { return _holding != null; }
    }
    public bool IsAiming {
        get { return _aiming != null; }
    }

    sealed protected override void Start()
    {
        base.Start();
        _liftCenter.x = _liftX;
        _liftCenter.y = _liftY;
        _liftCenter.z = _liftZ;
        _ignoreRaycast = LayerMask.NameToLayer("Ignore Raycast");
        // Debug.Log(LayerMask.LayerToName(_ignoreRaycast) + ": " + _ignoreRaycast);
    }

    private void Update()
    {
        Vector3 origin = _anchor.position;
        Vector3 direction = _anchor.forward;
        if (Physics.Raycast(origin, direction, out _hit, _maxInteractDistance))
        {
            if (_aiming != _hit.transform)
            {
                if (IsAiming && _aiming.TryGetComponent(out IInteractable item))
                {
                    item.OnLeaveFocus();
                }
                if ((_aiming = _hit.transform).TryGetComponent(out item))
                {
                    item.OnEnterFocus();
                }
            }
        }
        else
        {
            if (IsAiming)
            {
                if (_aiming.TryGetComponent(out IInteractable item))
                {
                    item.OnLeaveFocus();
                }
                _aiming = null;
            }
        }
        Debug.DrawRay(origin, direction * _maxInteractDistance, Color.red);

        if (IsHolding)
        {
            StabilizeCarryItem();
        }
    }

    public void Interact()
    {
        Debug.Log(_hit.distance);
        if (IsHolding)
        {
            if (_holding.TryGetComponent(out IPickable item))
            {
                DropDown(item);
            }
        }
        else if (IsAiming && _hit.distance <= _maxLiftDistance)
        {
            if (_aiming.TryGetComponent(out IPickable item))
            {
                PickUp(item);
            }
        }
    }

    private void PickUp(IPickable item)
    {
        _holding = _aiming;
        
        // process highlight issue
        item.OnLeaveFocus();
        _aiming = null;

        _holdingRB = _holding.GetComponent<Rigidbody>();
        _temp.linearDamping = _holdingRB.linearDamping;
        _temp.layer = _holding.gameObject.layer;
        _temp.parent = _holding.parent;
        _holdingRB.linearDamping = 10f;
        _holding.gameObject.layer = _ignoreRaycast;
        _holding.parent = transform;
        item.OnPick();
    }


    private void DropDown(IPickable item)
    {
        item.OnDrop();
        _holdingRB.linearDamping = _temp.linearDamping;
        _holding.gameObject.layer = _temp.layer;
        _holding.parent = _temp.parent;
        _temp.parent = null;
        _holding = null;
    }

    private void StabilizeCarryItem()
    {
        Vector3 bias = transform.TransformDirection(_liftCenter - _holding.localPosition);
        if (bias.magnitude > _maxLiftDistance)
        {
            DropDown(_holding.GetComponent<IPickable>());
        }
        else if (bias.magnitude > .1f)
        {
            _holdingRB.AddForce(bias * _liftForce);
        }
    }
}
