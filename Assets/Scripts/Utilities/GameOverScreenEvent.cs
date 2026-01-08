using UnityEngine;

public class GameOverScreenEvent : MonoBehaviour
{
    private GUIManager _gui;

    private void Awake()
    {
        _gui = GameObject.Find("GUI").GetComponent<GUIManager>();
    }

    public void OnAnimationEnd()
    {
        _gui.OnGameOverScreenDisplay();
    }
}
