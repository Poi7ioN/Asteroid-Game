using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//fuel Powerup script
public class PowerUp_Fuel : PowerUp
{
    PowerUpProperties data;                         // fuel power config data

    [Range(0f, 100f)] [SerializeField] private float _fuelDrop = 20f;     //set effect value of fuel powerup

    void Awake()
    {
        data = GetComponent<PowerUpProperties>(); 
    }

    // Start is called before the first frame update
    void OnEnable()
    {
        try //try catch is used because of object pooling conflict
        {
            SetDataOnEnable(data);  // try to set data on abstract parent class on enable
        }
        catch
        {
            //Debug.Log("Object Pooled");
        }
    }

    //Apply effect to player
    protected override void ApplyPowerUp(PlayerStats player)
    {
        player.UpdateFuelData(true, _fuelDrop);   //add fuel value to player current fuel level
        NotifyPowerStatus(false);                // this is a permanent powerup so notify about power status
        gameObject.SetActive(false);            //disable the object
    }

    //If Player did not pick up the power set expire call
    public override void PowerUpHasExpired()  
    {
        NotifyPowerStatus(false);
        gameObject.SetActive(false);
    }
}
