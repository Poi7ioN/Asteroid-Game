using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//This scriptable object file holds config data for weapon type.
[CreateAssetMenu(menuName = "Weapon Config")]
public class WeaponConfig : ScriptableObject
{
    [Header("Weapon Type Info")]

    [Tooltip("Common Bullet Prefab")]
    [SerializeField] GameObject _bulletPrefab;

    [Tooltip("Weapon Sprite Holder")]
    [SerializeField]  Sprite _weaponImage;

    [Tooltip("Number of bullets fired at once")]
    [SerializeField]  int _bulletCount;

    [Range(0.5f, 20f)]
    [Tooltip("Delay between next burst of fire")]
    [SerializeField]  float _nextFireDelay;

    [Range(50f, 150f)]
    [Tooltip("Speed of projectile")]
    [SerializeField] float _bulletSpeed;

    [Range(0.01f,5f)] [Tooltip("Delay between bullets in one burst")]
    [SerializeField] float  _fireRate;

    [Range(1,5)] [Tooltip("Damage to Enemies/Asteroid")]
    [SerializeField] int  _damage;

    
    //Getter and setter properties for private varibles.
    public GameObject GetBulletPrefab { get { return _bulletPrefab; } }

    public Sprite GetWeaponSprite { get { return _weaponImage; } }

    public int GetBulletCount { get { return _bulletCount; } }

    public float GetNextFireTime { get { return _nextFireDelay; } }

    public float GetBulletSpeed { get { return _bulletSpeed; } }

    public float GetFireRate { get { return _fireRate; } }

    public int GetDamage { get { return _damage; } }
    
}
