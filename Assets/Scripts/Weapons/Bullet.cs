using UnityEngine;

public class Bullet : MonoBehaviour
{
    public int damage = 10;
    public float lifetime = 2f;

    private void Start()
    {
        Destroy(gameObject, lifetime);
    }
}