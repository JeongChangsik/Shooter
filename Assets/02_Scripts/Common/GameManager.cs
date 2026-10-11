using System.Collections.Generic;
using System.Data.Common;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    [SerializeField] private List<Transform> _spawnPoints;
    [SerializeField] private GameObject _monsterPrefab;

    [Header("생성 옵션")]
    [SerializeField] private float _spawnRate = 3.0f;

    private bool _isGameOver = false;
    public bool IsGameOver
    {
        get => _isGameOver;
        set
        {
            _isGameOver = value;
            if(_isGameOver) CancelInvoke(nameof(SpawnMonster)); // InvokeRepeating 취소
        }
    }

    protected override void Awake()
    {
        base.Awake();
        
        Debug.Log("test");
        
        // _STAGES 게임 오브젝트를 검색
        // 1. transform 내 Find 함수를 사용
        // var spawnPointGroup = GameObject.Find("_STAGES/SpawnPointGroup").transform.Find("SpawnPointGroup");
        // 2. 한 번에 가져오기
        var spawnPointGroup = GameObject.Find("_STAGES/SpawnPointGroup");
        spawnPointGroup.GetComponentsInChildren<Transform>(_spawnPoints);
    }

    private void Start()
    {
        // Invoke(nameof(SpawnMonster), _spawnRate);   // 1회 호출
        InvokeRepeating(nameof(SpawnMonster), 0f, _spawnRate); // 2초 기다렸다가 3초간격으로 SpawnMonster 함수 호출
    }

    private void SpawnMonster()
    {
        int index = Random.Range(0, _spawnPoints.Count);
        Instantiate(_monsterPrefab, _spawnPoints[index].position, _spawnPoints[index].rotation);
    }
}
