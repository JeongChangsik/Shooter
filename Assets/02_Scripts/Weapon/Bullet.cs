using UnityEngine;

public class Bullet : MonoBehaviour
{
    // [SerializeField]
    [SerializeField] private float _force = 800.0f;
    private Rigidbody _rb;

    private void Start()
    {
        _rb = GetComponent<Rigidbody>();
        _rb.AddRelativeForce(Vector3.forward * _force); // _force = 800 -> 뉴튼(Newton) 단위(1kg의 물체를 1m 이동시키는 힘)
    }

}
