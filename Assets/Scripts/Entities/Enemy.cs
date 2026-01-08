using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class Enemy : NPCBase
{
    public enum StateType
    {
        Idle,
        Patrol,
        Angry,
        Confuse
    }

    private class IdleState : StateBase
    {
        private readonly Enemy _enemy;
        private float _lastAttempTime;
        private bool _isRotating;
        private Vector3 _faceTo;

        public IdleState(Enemy enemy)
        {
            _enemy = enemy;
        }

        public override void OnEnter()
        {
            base.OnEnter();
            _lastAttempTime = Time.time;
            _isRotating = false;
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
                // Debug.Log(_enemy._patrolChance);
                if (Random.value < _enemy._patrolChance)
                {
                    _enemy.SwitchTo(StateType.Patrol);
                }
                else if (Random.value > .45f)
                {
                    int angle = Random.Range(-90, 90);
                    _faceTo = Quaternion.AngleAxis(angle, _enemy.transform.up) * _enemy.transform.forward;
                    _isRotating = true;
                }
            }
        }
    }

    private class AngryState : StateBase
    {
        private readonly Enemy _enemy;

        public AngryState(Enemy enemy)
        {
            _enemy = enemy;
        }

        public override void OnEnter()
        {
            base.OnEnter();
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
                _enemy.SwitchTo(StateType.Confuse);
            }
        }
    }

    private class ConfuseState : StateBase
    {
        private readonly Enemy _enemy;
        private bool _isRotating;
        private int _lastRotateAngle;
        private Vector3 _faceTo;

        public ConfuseState(Enemy enemy)
        {
            _enemy = enemy;
        }

        public override void OnEnter()
        {
            base.OnEnter();
            _lastRotateAngle = 0;
            _isRotating = false;
            Debug.Log("Confuse!");
        }

        public override void Trigger()
        {
            if (_enemy.IsAngry)
            {
                _enemy.SwitchTo(StateType.Angry);
            }
            else if (_isRotating)
            {
                _isRotating = !_enemy.FaceTo(_faceTo);
            }
            else if (RunTime < 3f)
            {
                int angle;
                if (_lastRotateAngle <= 0)  // turn right
                {
                    angle = Random.Range(40, 50);
                }
                else                        // turn left
                {
                    angle = Random.Range(-40, -50);
                }
                _faceTo = Quaternion.AngleAxis(-_lastRotateAngle + angle, _enemy.transform.up) * _enemy.transform.forward;
                _isRotating = true;
                _lastRotateAngle = angle;
                // Debug.Log("Inspect Angle: " + angle);
            }
            else
            {
                _enemy.SwitchTo(StateType.Patrol);
            }
        }
    }

    private class PatrolState : StateBase
    {
        private readonly Enemy _enemy;
        private bool _isMoving;
        private Vector3 _moveTo;
        private int _lastUsed = 0;

        public PatrolState(Enemy enemy) {
            _enemy = enemy;
        }

        public override void OnEnter()
        {
            base.OnEnter();
            _isMoving = true;
            {
                List<int> filter;
                if (_enemy.PrevState is AngryState || _enemy.Location.Count < 2)
                {
                    filter = Enumerable.Range(0, _enemy.Location.Count)
                                       .ToList();
                }
                else
                {
                    filter = Enumerable.Range(0, _enemy.Location.Count)
                                       .Where(i => i != _lastUsed)
                                       .ToList();
                }
                int index = filter[Random.Range(0, filter.Count)];
                _moveTo = _enemy.Location[index];
                _lastUsed = index;
            }
            Debug.Log("Patrol! (" + _moveTo + ")");
        }

        public override void Trigger()
        {
            if (_enemy.IsAngry)
            {
                _enemy.SwitchTo(StateType.Angry);
            }
            else if (_isMoving)
            {
                // Debug.Log("_isMoving: " + _isMoving + ", IsTracking: " + _enemy.IsTracking);
                _isMoving = !_enemy.MoveTo(_moveTo);
                _enemy.FaceTo(_enemy.DesiredVelocity);
            }
            else
            {
                _enemy.Move(Vector2.zero);
                // Debug.Log(_enemy.DesiredVelocity);
                _enemy.SwitchTo(StateType.Idle);
            }
            
        }
    }

    [Header("Behaviours")]
    public List<Vector3> Location;
    [SerializeField][Range(0.01f, .20f)]
    private float _patrolChance = .1f;

    private Dictionary<StateType, StateBase> _stateTable;

    protected override void Start()
    {
        base.Start();
        _stateTable = new Dictionary<StateType, StateBase>();
        _stateTable.EnsureCapacity(4);
        _stateTable.Add(StateType.Idle, new IdleState(this));
        _stateTable.Add(StateType.Angry, new AngryState(this));
        _stateTable.Add(StateType.Patrol, new PatrolState(this));
        _stateTable.Add(StateType.Confuse, new ConfuseState(this));

        SwitchTo(StateType.Idle);
    }

    public void SwitchTo(StateType stateType)
    {
        _stateTable.TryGetValue(stateType, out StateBase state);
        PrevState = CurrState;
        NextState = state;
    }
}