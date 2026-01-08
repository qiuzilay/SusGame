using UnityEngine;

public class Gate : InteractableBase
{
    private GUIManager _gui;

    protected override void Start()
    {
        base.Start();
        _gui = GameObject.Find("GUI").GetComponent<GUIManager>();
    }

    public override string OnInteract(Transform transform)
    {
        if (transform != null && transform.name == "Test Paper")
        {
            _gui.OnGameOver(true);
            return _hintSuccessed;
        }
        else
        {
            return _hintFailed;
        }
    }
}