using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//This script is responsible for handling bullet collision.
public class Projectile : MonoBehaviour
{
    [SerializeField] private float delayToDisableBullet = 5f;  //time delay in which this gameobject will get disabled
    private int _damage;                       //damage to target
    private float _speed;                     //move speed of bullet

    Rigidbody2D _rigidbody;
    SpriteRenderer _spriteRenderer;
    Weapon _isProjectileOfParent;

    private void Awake()  //get necessary references of gameobjects
    {
        _rigidbody      = GetComponent<Rigidbody2D>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    //Update projectile properties like direction,image,damage,speed,parent type
    public void UpdateProjectileData(Vector2 direction, Sprite image, int dmg, float speed, Weapon isMyprojectile)
    {
        _spriteRenderer.sprite = image;
        _damage = dmg;
        _speed = speed;
        _isProjectileOfParent = isMyprojectile;                 //set weapon parent instance
        _rigidbody.AddForce(direction * _speed * 10f);         //add force to bullet rigidbody
        Invoke("DisableBulletOnDelay", delayToDisableBullet); //invoke call to disable the gameobject
    }

    private void DisableBulletOnDelay()  //disable the gameobject
    {
        gameObject.SetActive(false);
    }

    //This check for the trigger among target
    private void OnTriggerEnter2D(Collider2D collision)
    {
        //get component reference of parent gameobject who fires this bullets
        if(_isProjectileOfParent != collision.GetComponent<Weapon>())
        {
            //call Idamage interface methods on target who implements damage method.
            IDamagable damage = collision.GetComponent<IDamagable>();
            if (damage != null)
            {
                damage.Damage(_damage);  //damage the target withh damage value set by the weapon.
            }
            CancelInvoke();
            gameObject.SetActive(false);   //disable the gameobject on hit
        }
    }
}
