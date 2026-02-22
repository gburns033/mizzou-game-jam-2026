using UnityEngine;

public class Gun : Weapon
{
    public Transform shotPoint;
    public GameObject projectile;
    public float timeBetweenShots;

    protected override void Attack()
    {
        if (projectile != null && shotPoint != null)
        {
            Instantiate(projectile, shotPoint.position, shotPoint.rotation);
            Debug.Log("Gun fired!");
        }
    }
}