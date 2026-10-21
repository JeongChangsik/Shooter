using UnityEngine;

public class Barrel : MonoBehaviour, IDamagable
{
    [SerializeField] private int _hitCount = 0;
    [SerializeField] private int _hitMaxCount = 3;
    [SerializeField] private float _power = 100f;
    [SerializeField] private float _explosionRadius = 5f;
    [SerializeField] private float _explosionDamage = 100f;
    [SerializeField] private GameObject _explosionEffect;
    [SerializeField] private AudioClip _explosionSFX;
    [SerializeField] private LayerMask _layerMask;
    private AudioSource _audio;

    private void Start()
    {
        _audio = GetComponent<AudioSource>();
    }

    private void OnCollisionEnter(Collision coll)
    {
        if (coll.gameObject.CompareTag("BULLET"))
        {
            if (++_hitCount >= _hitMaxCount) ExplosionBarrel();
        }
    }

    private void ExplosionBarrel()
    {
        // Debug.Log("Barrel : 폭발");
        var rb = gameObject.AddComponent<Rigidbody>();
        Vector3 point = Random.insideUnitSphere * 2.0f;
        rb.AddForceAtPosition(Vector3.up * _power, transform.position + point, ForceMode.Impulse);
        var explosionEffect = Instantiate(_explosionEffect, transform.position, Quaternion.identity);
        _audio.PlayOneShot(_explosionSFX, 0.2f);
        
        // 주변에 피해
        var colliders = Physics.OverlapSphere(transform.position, _explosionRadius, _layerMask);
        foreach (var collider in colliders)
        {
            collider.gameObject.GetComponent<IDamagable>().TakeDamage(_explosionDamage);
        }

        // 3초후에 배럴이 사라짐
        Destroy(gameObject, 3f);
        // 5초후에 폭발 효과가 사라짐
        Destroy(explosionEffect, 5f);
    }

    public void TakeDamage(float damage)
    {
        if (++_hitCount >= _hitMaxCount) ExplosionBarrel();
    }
}
