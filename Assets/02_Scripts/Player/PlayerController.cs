using System;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Animator))]
public class PlayerController : MonoBehaviour, IDamagable
{
    // InputAction
    private InputSystem_Actions _inputActions;
    private InputAction _moveAction;    // Move action
    private InputAction _mouseDeltaAction; // Mouse delta action
    
    // 이동 방향 벡터
    private Vector3 _moveDir;
    [SerializeField] private float _moveSpeed = 6.0f; // 이동 속도
    [SerializeField] private float _turnSpeed = 50.0f; // 마우스 회전 속도

    private float _maxHp = 100f;
    private float _currHp = 100f;
    
    // Animator
    private Animator _animator;
    private static readonly int hashStrafe = Animator.StringToHash("Strafe");
    private static readonly int hashForward = Animator.StringToHash("Forward");

    public static Action OnPlayerDie;

    #region 유니티 생명주기 
    private void Awake()
    {
        _inputActions = new InputSystem_Actions();
        _moveAction = _inputActions.Player.Move;
        _mouseDeltaAction = _inputActions.Player.Look;
    }

    private void OnEnable()
    {
        _moveAction.performed += OnMove;
        _moveAction.canceled += OnMove;
        _inputActions.Enable();
    }

    private void OnDisable()
    {
        _moveAction.performed -= OnMove;
        _moveAction.canceled -= OnMove;
        _inputActions.Disable();
    }

    private void Start()
    {
        _animator = GetComponent<Animator>();
        
        // 마우스 락처리
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        Movement();
        Rotation();
        PlayAnimation();
    }
    #endregion
    
    #region 캐릭터 조작
    private void OnMove(InputAction.CallbackContext ctx)
    {
        Debug.Log($"Move: {ctx.ReadValue<Vector2>()}");

        if (ctx.phase == InputActionPhase.Performed)
        {
            var dir = ctx.ReadValue<Vector2>(); // (x,y)
            _moveDir = new Vector3(dir.x, 0, dir.y);    // (x,y,z)
        }
        else if (ctx.phase == InputActionPhase.Canceled)
        {
            _moveDir = Vector3.zero;
        }
    }

    private void Movement()
    {
        // (이동 방향 * 이동 속도 * 시간 보정)
        transform.Translate(_moveDir * (_moveSpeed * Time.deltaTime));
    }

    private void Rotation()
    {
        // 회전 로직
        var mouseDir = _mouseDeltaAction.ReadValue<Vector2>().x;
        // transform.Rotate(xAngle,yAngle,zAngle);
        transform.Rotate(0, mouseDir * _turnSpeed * Time.deltaTime, 0);
        // Debug.Log($"Mouse Delta X: {mouseDir}");
    }

    private void PlayAnimation()
    {
        _animator.SetFloat(hashStrafe, _moveDir.normalized.x);
        _animator.SetFloat(hashForward, _moveDir.normalized.z);
    }

    #endregion

    private void OnTriggerEnter(Collider coll)
    {
        if (coll.CompareTag("PUNCH"))
        {
            TakeDamage(coll.gameObject.GetComponentInParent<MonsterController>().Damage);
        }
    }

    public void TakeDamage(float damage)
    {
        _currHp -= damage;
        Debug.Log($"{damage}의 피해를 입었습니다. {_currHp}/{_maxHp}");
        if (_currHp <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        Debug.Log("플레이어가 사망하였습니다.");
        _currHp = 0;
        GameManager.Instance.IsGameOver = true;
        
        // SendMessage 방식 -> 델리게이트로 변환
        // GameObject[] monsters =  GameObject.FindGameObjectsWithTag("MONSTER");
        // foreach (GameObject monster in monsters)
        // {
        //     monster.SendMessage("YouWin", SendMessageOptions.DontRequireReceiver);
        // }
        
        OnPlayerDie.Invoke();
    }
}
