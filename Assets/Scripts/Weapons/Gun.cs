using UnityEngine;

public class Gun : Weapon
{
   public Transform shotPoint;
   public GameObject projectile;
   public float timeBetweenShots;
   float nextShotTime;


   void Start()
   {
       
   }

   void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            if(Time.time >= nextShotTime)
            {
                nextShotTime = Time.time + timeBetweenShots;
                Instantiate(projectile, shotPoint.position, shotPoint.rotation);
            }
        }
    }
   

}