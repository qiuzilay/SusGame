using UnityEngine;

public class Interactable : InteractableBase
{
    public override string OnInteract(Transform item)
    {
        if (Random.value > 0.5)
        {
            return _hintFailed;
        }
        else
        {
            return _hintSuccessed;
        }
    }
}
