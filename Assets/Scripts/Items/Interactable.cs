using UnityEngine;

public class Interactable : InteractableBase
{
    public override void OnEnterFocus()
    {
        base.OnEnterFocus();
        Debug.Log(gameObject.name);
    }
}
