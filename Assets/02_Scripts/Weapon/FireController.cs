using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class FireController : MonoBehaviour
{
    [SerializeField] private Transform _firePos;
    [SerializeField] private GameObject _bulletPrefab;
    
    // InputAction으로 가져오는 방법 외
    // 유니티에서 InputSystem_Actions 하위에 Player/Action을 드래그 앤 드롭으로 연결
    [SerializeField] private InputActionReference _fireAction;

    [SerializeField] private AudioClip _fireSFX;
    private AudioSource _audio;
    
    private MeshRenderer _muzzleFlash;
    private Light _fireLight;
    

    private void Start()
    {
        if (_firePos != null)
        {
            _muzzleFlash = _firePos.GetComponentInChildren<MeshRenderer>();
            _muzzleFlash.enabled = false;   // 비활성화
            
            _fireLight =  _firePos.GetComponentInChildren<Light>();
            _fireLight.intensity = 0f;
        }
        
        _audio = GetComponent<AudioSource>();
    }

    private void OnEnable()
    {
        _fireAction.action.performed += Fire;
        _fireAction.action.Enable();
    }

    private void OnDisable()
    {
        _fireAction.action.performed -= Fire;
        _fireAction.action.Disable();
    }
    
    private void Fire(InputAction.CallbackContext ctx)
    {
        // Debug.Log("Fire");
        // Instantiate(생성할 객체, 좌표, 각도, [부모객체])
        Instantiate(_bulletPrefab, _firePos.position, _firePos.rotation);
        StartCoroutine(ShowMuzzleFlash());
        
        // 효과음 재생(사운드 조절 : 0.2f)
        _audio.PlayOneShot(_fireSFX, 0.2f);
    }

    private IEnumerator ShowMuzzleFlash()
    {
        // var offset = Random.Range(0, 2) * 0.5f;
        Vector2 offset = new Vector2(Random.Range(0,2), Random.Range(0,2)) * 0.5f;
        _muzzleFlash.material.mainTextureOffset = offset;
        
        // 스케일 변경
        float scale = Random.Range(0.8f, 2.0f);
        _muzzleFlash.transform.localScale = Vector3.one * scale;
        
        // 회전값 변경
        float angle = Random.Range(0, 360);
        _muzzleFlash.transform.localRotation = Quaternion.Euler(0, 0, angle);
        
        _fireLight.intensity = scale;
        _muzzleFlash.enabled = true;
        yield return new WaitForSeconds(0.1f);
        _muzzleFlash.enabled = false;
        _fireLight.intensity = 0f;
        
    }

    // 옛날 방식
    // private void Update()
    // {
    //     if (Input.GetMouseButton(0)) Fire();
    // }
}

/*
 * Quaternion 쿼터니언 (복소수 사차원 벡터) (x,y,z,w)
 * - 유니티에서는 오일러각을 사용하지만 내부적으로는 쿼터니언으로 처리함 (확인하려면 Inspector 창을 Debug 모드로 변경)
 * - 하지만, 오일러각에는 치명적인 문제가 있음. 바로, 짐벌락(Gimbal Lock)
 * - 이것을 방지하려고 쿼터니언 방식을 사용하게 됨
 *
 * 사용 함수
 * - Quaternion.LookRotation(vector3)
 * - Quaternion.Euler(x,y,z)
 */
