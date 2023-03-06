using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Life Power up script
public class PowerUp_Life : PowerUp
{
    PowerUpProperties data;                                         //Life power up cofig data

    [Range(0, 10)] [SerializeField] private int _addLife = 1;       //set effect value of life powerup

    // Start is called before the first frame update
    void Awake()
    {
        data = GetComponent<PowerUpProperties>();
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
        player.UpdatePlayerLife(true, _addLife);  //add life value to player current lives
        NotifyPowerStatus(false);                //this is a permanent powerup so notify about power status
        gameObject.SetActive(false);            //disable the object
    }

    //If Player did not pick up the power set expire call
    public override void PowerUpHasExpired()
    {
        NotifyPowerStatus(false);
        gameObject.SetActive(false);
    }
}
