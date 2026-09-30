using UnityEngine;

public class RemoveBullet : MonoBehaviour
{
    [SerializeField] private GameObject _sparkEffect;

    /*
     * 충돌 콜백 함수 (Collision Callback Function)
     * 1. IsTrigger 언체크
     * OnCollisionEnter -> 1회 호출
     * OnCollisionStay -> n회 호출
     * OnCollisionExit -> 1회 호출
     *
     * 2. IsTrigger 체크
     * OnTriggerEnter -> 1회 호출
     * OnTriggerStay -> n회 호출
     * OnTriggerExit -> 1회 호출
     */
    private void OnCollisionEnter(Collision coll)
    {
        // 충돌한 객체의 태그 비교
        if (coll.gameObject.CompareTag("BULLET"))
        {
            // 스파크 이펙트 발생
            // 1. 충돌 좌표 (Contact Point)
            ContactPoint cp = coll.GetContact(0);
            Vector3 point = cp.point;   // 충돌 지점
            // 2. 법선 벡터(충돌한 벽에서 수직인 벡터, 노말 벡터)
            Vector3 normal = -cp.normal;
            // 벡터를 쿼터니언 타입으로 변환
            Quaternion rot = Quaternion.LookRotation(normal);
            
            // 3. Spark 설정
            var spark = Instantiate(_sparkEffect, point, rot);
            
            Destroy(coll.gameObject);
            Destroy(spark, 0.3f);
        }
    }
}
