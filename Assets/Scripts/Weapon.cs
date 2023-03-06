using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//This script is the common weapon fire script on player and enemy
public class Weapon : WeaponBase
{
    [SerializeField] private WeaponConfig defaultWeapon;             // default weapon config file

    public delegate void Shooting(Vector2 dir,Transform pos);      // Declare a delegate for fire weapon type
    public Shooting GetShootingType;                              // declares a variable of type "Shooting" named "GetShootingType"
    private bool _isShooting;                                    //check if shooting active or not


    private void OnEnable()  // set default weapon type on enable
    {
        _isShooting = false;
        SetDefaultWeapon();
    }

    //Power up method that sets the respective powerup type and config data.
    public void SetWeaponType(WeaponConfig config,PowerUpTypes type)
    {
        GetShootingType = null;

        SetWeaponConfigData(config,this);

        switch(type)  // switch between weapon types
        {
            case PowerUpTypes.Moon:
                 GetShootingType += CrescentFire;
                 break;
            case PowerUpTypes.Spread:
                GetShootingType += SpreadFire;
                break;
            default:
                GetShootingType += DefaultFireFunct;
                break;
        }
        
    }

    public void SetDefaultWeapon()  // set to default weapon
    {
        GetShootingType = null;
        SetWeaponConfigData(defaultWeapon,this);
        GetShootingType += DefaultFireFunct;
    }

    public void Shoot()  //shoot method
    {
        if(!_isShooting)
        {
            StartCoroutine(Fire());
        }
    }

    IEnumerator Fire()
    {
        _isShooting = true;

        for (int i = 0; i < bulletCount; i++)   // loop through bullet count
        {
            GetShootingType(transform.up, transform);     //call shooting method with delegate pointer
            yield return new WaitForSeconds(fireRate);   //fire rate between next bullet
        }

        yield return new WaitForSeconds(nextFireDelay);  //delay between next fire 

        _isShooting = false;
    }

}
