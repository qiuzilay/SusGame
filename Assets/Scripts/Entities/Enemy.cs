using UnityEngine;
using System.Collections.Generic;

public class Enemy : NPCBase
{
    private enum StateType
    {
        Idle,
        Patrol,
        Angry
    }

    private class IdleState : StateBase
    {
        private Enemy _enemy;
        private float _lastAttempTime;
        private bool _isRotating = false;
        private Vector3 _faceTo;

        public IdleState(Enemy enemy)
        {
            _enemy = enemy;
        }

        public override void OnEnter()
        {
            base.OnEnter();
            _lastAttempTime = Time.time;
            Debug.Log("Idle!");
        }

        public override void Trigger()
        {
            if (_enemy.IsAngry)
            {
                _enemy.SwitchTo(StateType.Angry);
            }
            else if (_isRotating)
            {
                if (!(_isRotating = !_enemy.FaceTo(_faceTo)))
                {
                    _lastAttempTime = Time.time;
                }
            }
            else if (Time.time - _lastAttempTime > 5f)
            {
                if (Random.value > .95f)
                {
                    Debug.Log("Patrol!");
                    // _enemy.SwitchTo(StateType.Patrol);
                }
                else if (Random.value > .45f)
                {
                    int angle = Random.Range(-90, 90);
                    _faceTo = Quaternion.AngleAxis(angle, _enemy.transform.up) * _enemy.transform.forward;
                    _isRotating = true;
                }
            }
        }
        public override void OnLeave() {}
    }

    private class AngryState : StateBase
    {
        private Enemy _enemy;

        public AngryState(Enemy enemy)
        {
            _enemy = enemy;
        }

        public override void OnEnter()
        {
            Debug.Log("Angry!");
        }

        public override void Trigger()
        {
            if (_enemy.IsAngry)
            {
                _enemy.MoveTo(_enemy.Target.position);
                _enemy.FaceTo(_enemy.DesiredVelocity);
            }
            else
            {
                _enemy.Move(Vector2.zero);  // brake
                _enemy.SwitchTo(StateType.Idle);
            }
        }

        public override void OnLeave() {}
    }


    private Dictionary<StateType, StateBase> _stateTable;

    protected override void Start()
    {
        base.Start();
        _stateTable = new Dictionary<StateType, StateBase>();
        _stateTable.EnsureCapacity(3);
        _stateTable.Add(StateType.Idle, new IdleState(this));
        _stateTable.Add(StateType.Angry, new AngryState(this));

        SwitchTo(StateType.Idle);
    }

    private void SwitchTo(StateType stateType)
    {
        _stateTable.TryGetValue(stateType, out StateBase state);
        NextState = state;
    }
}