using UnityEngine;

public class MyGizmos : MonoBehaviour
{
    [SerializeField] private Color _color = Color.red;
    [SerializeField, Range(0.2f, 3.0f)] private float _radius = 0.5f;

    private void OnDrawGizmos()
    {
        Gizmos.color = _color;
        Gizmos.DrawSphere(transform.position, _radius);
    }
}
