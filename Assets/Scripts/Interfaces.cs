using UnityEngine;

public interface IInteractable
{
    public void OnEnterFocus();
    public void OnLeaveFocus();
    public void OnInteract();
}

public interface IPickable : IInteractable
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