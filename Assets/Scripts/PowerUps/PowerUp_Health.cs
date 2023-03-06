using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Health Powerup script
public class PowerUp_Health : PowerUp
{
    PowerUpProperties data;                           // health power config data

    [Range(5,100)] [SerializeField] private int _healthDrop = 20; //set effect value of haelth powerup

    private void Awake()
    {
         data  = GetComponent<PowerUpProperties>();
    }

    // Start is called before the first frame update
    void OnEnable()
    {
        try //try catch is used because of object pooling conflict
        {
            SetDataOnEnable(data); // try to set data on abstract parent class on enable
        }
        catch
        {
            //Debug.Log("Object Pooled");
        }
    }

    //Apply effect to player
    protected override void ApplyPowerUp(PlayerStats player)
    {
        player.UpdateHealthData(true, _healthDrop);  //add healtth value to player current health level
        NotifyPowerStatus(false);                   // this is a permanent powerup so notify about power status
        gameObject.SetActive(false);               //disable the object
    }

    //If Player did not pick up the power set expire call
    public override void PowerUpHasExpired()
    {
        NotifyPowerStatus(false);
        gameObject.SetActive(false);
    }
}
