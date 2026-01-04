using Unity.VisualScripting;
using UnityEngine;

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