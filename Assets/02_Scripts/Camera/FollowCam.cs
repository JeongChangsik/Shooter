using System.Numerics;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;

public class FollowCam : MonoBehaviour
{
    public Transform target;
    [Range(3f, 15f)] public float distance = 8.0f;
    [Range(-3f, 10f)] public float height = 3.0f;
    [Range(-3f, 5f)] public float yOffset = 1.8f;
    [Range(-3f, 3f)] public float xOffset = 0f;

    private void LateUpdate()
    {
        Vector3 targetPos = target.position + (Vector3.up * yOffset) + (target.right * xOffset);
        transform.position = targetPos - (target.forward * distance) + (Vector3.up * height);
        transform.LookAt(targetPos);
    }

}
