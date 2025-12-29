using UnityEngine;

public class Pickable : PickableBase
{
    public override void OnInteract() {}
    public override void OnUse() {}
    
    public override void OnEnterFocus()
    {
        base.OnEnterFocus();
        Debug.Log(gameObject.name);
    }
}
