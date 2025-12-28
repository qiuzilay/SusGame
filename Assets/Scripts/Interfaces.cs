using UnityEngine;

public interface IInteractable
{
    public void OnEnterFocus();
    public void OnLeaveFocus();
    public void OnInteract();
}

public interface IUsable : IInteractable
{
    public void OnPick();
    public void OnUse();
    public void OnDrop();
}
