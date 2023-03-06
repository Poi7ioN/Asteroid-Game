using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//This script holds the reference for power up spawn config data. it is a necessary script for powerups.
public class PowerUpProperties : MonoBehaviour
{
    private PowerSpawnerConfig _configData;

    public PowerSpawnerConfig configData
    {
        get { return _configData;  }
        set { _configData = value; }
    }
}
