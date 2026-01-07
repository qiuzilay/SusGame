using UnityEngine;

public interface IHighlightable
{
    public void OnEnterFocus();
    public void OnLeaveFocus();
}

public interface IInteractable : IHighlightable
{
    public string OnInteract(Transform item);
}

public interface IPickable : IHighlightable
{
    public void OnPick();
    public void OnUse();
    public void OnDrop();
}

public interface IState
{
    public void OnEnter();
    public void Trigger();
    public void OnLeave();
}