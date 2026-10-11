using UnityEngine;

public abstract class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T _instance;

    public static T Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindAnyObjectByType<T>();
            }
            
            if (_instance == null)
            {
                // 없으면(씬에 존재하지 않으면) 새로 만들어줌 (편의성, 하지만 남용은 주의)
                GameObject obj = new GameObject(typeof(T).Name);
                _instance = obj.AddComponent<T>();
            }
            
            return _instance;
        }
    }

    protected virtual void Awake()
    {
        // 싱글턴 인스턴스가 이미 존재한 상태에서 처리해야할 것
        if (_instance == null)
        {
            _instance = this as T;
            DontDestroyOnLoad(gameObject);
        }
        else if (_instance != this)
        {
            // instance != this 추가하는 이유
            // 씬을 왔다갔을 때, Entity ID가 서로 다르게 생성됨. 
            // 하지만 instance의 값은 static이라 값이 유지되어 버릴 수 있음
            Destroy(gameObject); // 중복 생성 방지
            return;
        }
    }
    
    protected virtual void OnDestroy()
    {
        // 싱글턴 인스턴스가 파괴될 때 null 초기화
        if(_instance == this) _instance = null;
    }
}
