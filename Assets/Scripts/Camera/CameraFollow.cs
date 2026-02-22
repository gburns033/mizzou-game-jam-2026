using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;
    public Vector3 offset = new Vector3(0, 0, -10);
    [Range(0, 20f)]
    public float smoothness = 5f;

    private void LateUpdate()
    {
        if (target == null) return;

        Vector3 targetPos = target.position + offset;

        transform.position = Vector3.Lerp(transform.position, targetPos, smoothness * Time.deltaTime);
    }

    public void JumpToPlayer()
    {
        if (target != null) transform.position = target.position + offset;
    }
}