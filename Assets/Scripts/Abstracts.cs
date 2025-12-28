using UnityEngine;

public abstract class InteractableBase : MonoBehaviour, IInteractable
{
    protected int _defaultLayer;
    protected int _highlightLayer;

    protected virtual void Start()
    {
        _defaultLayer = gameObject.layer;
        _highlightLayer = LayerMask.NameToLayer("Highlight");
    }

    public virtual void OnEnterFocus()
    {
        gameObject.layer = _highlightLayer;
    }

    public virtual void OnInteract()
    {
    }

    public virtual void OnLeaveFocus()
    {
        gameObject.layer = _defaultLayer;
    }
}
