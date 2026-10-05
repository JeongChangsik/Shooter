using System.Collections;
using UnityEngine;

public enum MonsterState
{
    Idle,
    Trace,
    Attack,
    Die,
    Patrol,
}

public class MonsterController : MonoBehaviour
{
    [SerializeField] private MonsterState _state = MonsterState.Idle;
    [SerializeField, Range(5f, 15f)] private float _traceDist = 8f;
    [SerializeField, Range(0f, 3f)] private float _attackDist = 2f;

    private Transform _mosterTr;
    private Transform _playerTr;

    public bool IsDead = false;

    private WaitForSeconds _stateCheckingTime = new WaitForSeconds(0.3f);
    
    private void Start()
    {
        _mosterTr = transform;
        _playerTr = GameObject.FindGameObjectWithTag("PLAYER")?.transform;
        StartCoroutine(CheckMonsterState());
    }

    private IEnumerator CheckMonsterState()
    {
        while (!IsDead)
        {
            // 공격 사정거리 이내인 경우
            if ((_mosterTr.position - _playerTr.position).sqrMagnitude <= _attackDist * _attackDist)
            {
                _state = MonsterState.Attack;
            }
            // 추적 사정거리 이내인 경우
            else if ((_mosterTr.position - _playerTr.position).sqrMagnitude <= _traceDist * _traceDist)
            {
                _state = MonsterState.Trace;
            }
            else
            {
                _state = MonsterState.Idle;
            }

            yield return _stateCheckingTime;
        }
    }
}
