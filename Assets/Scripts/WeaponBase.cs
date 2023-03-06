using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//This is a abstract base class for weapons.
public abstract class WeaponBase : MonoBehaviour
{
    private GameObject _bullet;                    //bullet prefab
    private Sprite _weaponImage;                  //bullet image
    private int    _bulletCount;                 //bullet count
    private float  _nextFireDelay;              //delay between next fire
    private float  _fireRate;                  //fire rate
    private int    _damage;                   //damage to target
    private float  _speed;                   //speed of bullet
    private Weapon _parentWeapon;           //parent instance who fires the weapon

    public GameObject Bullet
    {
        get { return _bullet; }
        set { _bullet = value; }
    }

    public Sprite image
    {
        get { return _weaponImage; }
        set { _weaponImage = value; }
    }

    public int bulletCount
    {
        get { return _bulletCount; }
        set { _bulletCount = value; }
    }

    public Weapon parentType
    {
        get { return _parentWeapon; }
        set { _parentWeapon = value; }
    }

    public float nextFireDelay
    {
        get { return _nextFireDelay; }
        set { _nextFireDelay = value; }
    }

    public float bulletSpeed
    {
        get { return _speed; }
        set { _speed = value; }
    }

    public float fireRate
    {
        get { return _fireRate; }
        set { _fireRate = value; }
    }

    public int damage
    {
        get { return _damage; }
        set { _damage = value; }
    }

    //Method to populate the weapon properties with weapon config data
    public virtual void SetWeaponConfigData(WeaponConfig weaponConfigData,Weapon parentObj)
    {
        bulletSpeed        = weaponConfigData.GetBulletSpeed;
        image              = weaponConfigData.GetWeaponSprite;
        Bullet             = weaponConfigData.GetBulletPrefab;
        bulletCount        = weaponConfigData.GetBulletCount;
        nextFireDelay      = weaponConfigData.GetNextFireTime;
        fireRate           = weaponConfigData.GetFireRate;
        damage             = weaponConfigData.GetDamage;
        parentType         = parentObj;
    }

    //general method to instantiate bullet and set resp properties
    private void SpawnBullet(Vector2 dir, Transform position)
    {
        //get bullet from object pooler
        GameObject bulletPrefab = ObjectPooler.SharedInstance.GetPooledObject(Bullet);
        bulletPrefab.transform.position = position.position;
        bulletPrefab.transform.rotation = Quaternion.identity;
        bulletPrefab.SetActive(true);

        //set bullets image, damage, speed and parent type
        bulletPrefab.GetComponent<Projectile>().UpdateProjectileData(dir, image, damage, bulletSpeed, parentType);
    }

    //Default weapon type funtion
    public void DefaultFireFunct(Vector2 dir, Transform position) // default fire weapon
    {
        AudioManager.Instance.PlayAudioClip(0, "Single");  //play single fire audio clip
        SpawnBullet(dir, position);
    }

    public void CrescentFire(Vector2 dir, Transform trans)  // crescent moon fire weapon
    {
        AudioManager.Instance.PlayAudioClip(0, "Single");  //play single fire audio clip
        SpawnBullet(dir, trans);
    }

    // Method for spread fire weapon
    public void SpreadFire(Vector2 dir, Transform position)
    {
        // Divide 360 degrees by number of bullets to get angle between each bullet
        float angleStep = 360f / bulletCount;
        float angle = 0f;

        // Loop through number of bullets
        for (int i = 0; i < bulletCount; i++)
        {
            // Calculate the position of each bullet based on angle between bullets
            float projectileDirXposition = position.position.x + Mathf.Sin((angle * Mathf.PI) / 180) * 10;
            float projectileDirYposition = position.position.y + Mathf.Cos((angle * Mathf.PI) / 180) * 10;

            Vector3 projectileVector = new Vector2(projectileDirXposition, projectileDirYposition);
            Vector2 projectileMoveDirection = (projectileVector - position.position).normalized * 1f;

            // Spawn each bullet and give it a movement direction
            SpawnBullet(new Vector2(projectileMoveDirection.x, projectileMoveDirection.y),position);

            // Increment angle for next bullet
            angle += angleStep;
        }
        AudioManager.Instance.PlayAudioClip(1, "Spread");  // Play audio clip for spread weapon
    }
}
