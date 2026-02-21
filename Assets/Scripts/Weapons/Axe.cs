using UnityEngine;

public class Axe : Weapon
{
    public float axeRange = 1.5f;

    protected override void Attack()
    {
        Debug.Log("Axe swings!");

        Collider2D[] hits = Physics2D.OverlapCircleAll(weaponTransform.position, axeRange);
        foreach (Collider2D hit in hits)
        {
            if (hit.CompareTag("Enemy"))
            {
                Debug.Log("Hit an enemy!");
            }
        }
    }
}