using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

public enum MonsterState
{
    Idle,
    Trace,
    Attack,
    Die,
    Patrol,
}

public class MonsterController : MonoBehaviour, IDamagable
{
    [SerializeField] private MonsterState _state = MonsterState.Idle;
    [SerializeField, Range(5f, 50f)] private float _traceDist = 8f;
    [SerializeField, Range(0f, 3f)] private float _attackDist = 2f;
    [SerializeField] private float _hp = 100f;
    [SerializeField] private float _damage = 10f;
    public float Damage => _damage;

    private Transform _mosterTr;
    private Transform _playerTr;
    private NavMeshAgent _agent;
    private Animator _animator;
    private CapsuleCollider _collider;

    private Collider[] _colliders; 

    private static readonly int hashIsTrace = Animator.StringToHash("IsTrace");
    private static readonly int hashIsAttack = Animator.StringToHash("IsAttack");
    private static readonly int hashHit = Animator.StringToHash("Hit");
    private static readonly int hashDie = Animator.StringToHash("Die");
    private static readonly int hashPlayerDie = Animator.StringToHash("PlayerDie");
    private static readonly int hashSpeed = Animator.StringToHash("Speed");

    public bool IsDead = false;

    private WaitForSeconds _stateCheckingTime = new WaitForSeconds(0.3f);
    
    // private void Start()
    // Start()에서 Awake()로 변경, 이유는 OnEnable은 Start 호출보다 먼저 호출됨 -> 그래서 Awake에서 찾는걸로 변경
    private void Awake()
    {
        _mosterTr = transform;
        _playerTr = GameObject.FindGameObjectWithTag("PLAYER")?.transform;
        _agent = GetComponent<NavMeshAgent>();
        _animator = GetComponent<Animator>();
        _collider = GetComponent<CapsuleCollider>();
        _colliders = GetComponentsInChildren<Collider>();
    }

    private void OnEnable()
    {
        PlayerController.OnPlayerDie += YouWin;
        
        // Respawn 시 코루틴 재가동
        StartCoroutine(CheckMonsterState());
        StartCoroutine(MonsterAction());
    }

    private void OnDisable()
    {
        PlayerController.OnPlayerDie -= YouWin;
    }

    private IEnumerator CheckMonsterState()
    {
        Debug.Log("CheckMonsterState");
        while (!IsDead)
        {
            if (_state == MonsterState.Die) yield break;
            
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

    private IEnumerator MonsterAction()
    {
        while (!IsDead)
        {
            // 몬스터 상태에 따라서 행동 패턴 분기
            switch (_state)
            {
                case MonsterState.Idle:
                    // Debug.Log("MonsterState.Idle");
                    _agent.isStopped = true;
                    _animator.SetBool(hashIsTrace, false);
                    _animator.SetBool(hashIsAttack, false);
                    break;
                case MonsterState.Trace:
                    // Debug.Log("MonsterState.Trace");
                    _agent.SetDestination(_playerTr.position);
                    _agent.isStopped = false;   // 예전에는 _agent.Resume(); 을 사용
                    _animator.SetBool(hashIsTrace, true);
                    _animator.SetBool(hashIsAttack, false);
                    break;
                case MonsterState.Attack:
                    // Debug.Log("MonsterState.Attack");
                    _agent.isStopped = true;
                    _animator.SetBool(hashIsAttack, true);
                    break;
                case MonsterState.Die:
                    // Debug.Log("MonsterState.Die");
                    IsDead = true;
                    _agent.isStopped = true;
                    // _collider.enabled = false;
                    foreach(var coll in _colliders) coll.enabled = false;
                    GameManager.Instance.DisplayScore(50);
                    Invoke(nameof(ReturnPool), 3f); // 3초 후 ReturnPool 함수 호출
                    break;
            }
            yield return _stateCheckingTime;
        }
    }

    public void TakeDamage(float damage)
    {
        _hp -= damage;
        _animator.SetTrigger(hashHit);
        
        if (_hp <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        _hp = 0;
        _state = MonsterState.Die;
        _animator.SetTrigger(hashDie);
    }

    private void OnCollisionEnter(Collision coll)
    {
        if (coll.gameObject.CompareTag("BULLET"))
        {
            Destroy(coll.gameObject);
        }
    }

    private void YouWin()
    {
        // StopCoroutine(MonsterAction());
        // StopCoroutine(CheckMonsterState());
        StopAllCoroutines();
        // _agent.isStopped = true;
        _animator.SetFloat(hashSpeed, Random.Range(0.8f, 1.2f));
        _animator.SetTrigger(hashPlayerDie);
    }

    private void ReturnPool()
    {
        // 비활성화 처리
        gameObject.SetActive(false);

        // 속성 초기화
        _hp = 100f;
        _state = MonsterState.Idle;
        foreach(var coll in _colliders) coll.enabled = true;
        _collider.enabled = true;
        IsDead = false;
    }
}
