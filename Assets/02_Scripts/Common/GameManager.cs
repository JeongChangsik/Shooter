using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : Singleton<GameManager>
{
    [SerializeField] private List<Transform> _spawnPoints;
    [SerializeField] private GameObject _monsterPrefab;

    [Header("생성 옵션")]
    [SerializeField] private float _spawnRate = 3.0f;
    
    // Object Pool
    [SerializeField] private List<Transform> _monsterPool;
    [SerializeField] private int _maxPool = 10;

    // UI
    [Header("UI")]
    [SerializeField] private Image _hpBar;
    [SerializeField] private TextMeshProUGUI _scoreText;

    private int _totScore;
    private const string TOT_SCORE = "TOT_SCORE";

    public void DisplayScore(int score)
    {
        _totScore += score;
        _scoreText.text = $"SCORE : <color=#ff0000><b>{_totScore:00000}</b></color>";
        
        // 점수 저장 기능
        // 로컬에 게임 데이터를 저장할 때 PlayerPrefs는 사용하지 않음 -> 변조 위험.
        // 그래서 SQLite 같은 DB에 저장함. 하지만 이것도 좋은 방법은 아님.
        // Cloud DB(AWS DynamoDB, EC2 MySQLs, MariaDB, PostgreSQL, Supabase DB(PostgreSQL))에 저장하는 방법이 있음.
        PlayerPrefs.SetInt(TOT_SCORE, _totScore);
    }

    public void ChangeHp(float currHp, float maxHp)
    {
        _hpBar.fillAmount = currHp / maxHp;
    }

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
        
        // _STAGES 게임 오브젝트를 검색
        // 1. transform 내 Find 함수를 사용
        // var spawnPointGroup = GameObject.Find("_STAGES/SpawnPointGroup").transform.Find("SpawnPointGroup");
        // 2. 한 번에 가져오기
        var spawnPointGroup = GameObject.Find("_STAGES/SpawnPointGroup");
        spawnPointGroup.GetComponentsInChildren<Transform>(_spawnPoints);
    }

    private void Start()
    {
        // 저장된 최고 점수 불러오기
        if (PlayerPrefs.HasKey(TOT_SCORE))
        {
            _totScore = PlayerPrefs.GetInt(TOT_SCORE);
            DisplayScore(0);
        }

        // 0은 default 값
        // _totScore = PlayerPrefs.GetInt(TOT_SCORE, 0);

        // Invoke(nameof(SpawnMonster), _spawnRate);   // 1회 호출
        InvokeRepeating(nameof(SpawnMonster), 0f, _spawnRate); // 2초 기다렸다가 3초간격으로 SpawnMonster 함수 호출
        
        // Monster Pool에 몬스터 생성
        for (int i = 0; i < _maxPool; i++)
        {
            var monster = Instantiate(_monsterPrefab);
            monster.name = $"Monster_{i+1:00}";
            monster.SetActive(false);
            _monsterPool.Add(monster.transform);
        }
    }

    private void SpawnMonster()
    {
        int index = Random.Range(0, _spawnPoints.Count);
        // 기존 스폰
        // Instantiate(_monsterPrefab, _spawnPoints[index].position, _spawnPoints[index].rotation);
        
        // 오브젝트 풀을 이용한 스폰
        foreach (var monster in _monsterPool)
        {
            if (monster.gameObject.activeSelf == false)
            {
                monster.position = _spawnPoints[index].position;
                var dir = Vector3.zero - _spawnPoints[index].position;
                monster.rotation = Quaternion.LookRotation(dir);
                monster.gameObject.SetActive(true);
                break;
            }

        }
    }
}
