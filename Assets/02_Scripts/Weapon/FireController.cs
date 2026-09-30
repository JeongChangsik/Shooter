using UnityEngine;
using UnityEngine.InputSystem;

public class FireController : MonoBehaviour
{
    [SerializeField] private Transform _firePos;
    [SerializeField] private GameObject _bulletPrefab;
    
    // InputAction으로 가져오는 방법 외
    // 유니티에서 InputSystem_Actions 하위에 Player/Action을 드래그 앤 드롭으로 연결
    [SerializeField] private InputActionReference _fireAction;

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
    }

    // 옛날 방식
    // private void Update()
    // {
    //     if (Input.GetMouseButton(0)) Fire();
    // }
}
