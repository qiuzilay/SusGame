using UnityEngine;

public class Player : CharacterMovement
{
    [SerializeField] private float _maxDistance = 8f;
    [SerializeField] private LayerMask _mask;

    private Transform _holding = null;
    private Transform _aiming = null;
    private RaycastHit _info;

    public bool IsHolding {
        get { return _holding != null; }
    }
    public bool IsAiming {
        get { return _aiming != null; }
    }

    void Update()
    {
        Vector3 origin = _anchor.position;
        Vector3 direction = _anchor.forward;
        if (Physics.Raycast(origin, direction, out _info, _maxDistance))
        {
            if (_aiming != _info.transform)
            {
                if (IsAiming && _aiming.TryGetComponent(out IInteractable item))
                {
                    item.OnLeaveFocus();
                }
                if ((_aiming = _info.transform).TryGetComponent(out item))
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
        Debug.DrawRay(origin, direction * _maxDistance, Color.red);
    }

    public void Use()
    {
        if (IsHolding)
        {
            if (_holding.TryGetComponent(out IUsable item))
            {
            }
            // else if (_holding.TryGetComponent)
        }
        else if (IsAiming)
        {
            if (_aiming.TryGetComponent(out IInteractable item)) {}
        }
    }
}
