using UnityEngine;

public class Cablebox : InteractableBase
{
    [SerializeField]
    private Enemy _boss;

    public override string OnInteract(Transform transform)
    {
        if (transform != null && transform.name == "Scissors")
        {
            _boss.Location.Add(new Vector3(-61f, 9f, -40));
            _boss.SwitchTo(Enemy.StateType.Patrol);
            return _hintSuccessed;
        }
        else
        {
            return _hintFailed;
        }
    }
}