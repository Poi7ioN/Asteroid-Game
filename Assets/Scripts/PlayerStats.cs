using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//This script is responsible for handling player stats like health, fuel etc.
public class PlayerStats : MonoBehaviour , IDamagable
{
    public PlayerConfigFile playerConfigData;              //player config file
    [HideInInspector] public bool _isPlayerAlive;         //check player alive status
    [SerializeField]  private GameObject shield;         //shield gameobject reference
    [SerializeField]  private GameObject trailEffect;   // trail particle gameobject reference
    private bool _isgameStarted = false;               //check game start
    private int _maxPlayerLives;                      //maximum playes chances
    private int _maxPlayerHealth;                    //maximum player ship health
    private int _currentPlayerHealth;               //current player ship health
    private float _maxPlayerFuel;                  //maximum player ship fuel
    private float _fuelDepletionRate;             //fuel depletion rate on movement
    private float _currentPlayerFuel;            //current player ship fuel 
    public float GetCurrentFuel { get { return _currentPlayerFuel; } }  //Property for current fuel level
    private int _shieldHits;                  //count of hits shield can withstand.
    private int originalLayer = 7;           //original layer of player to detect collision

    SpriteRenderer _spriteRenderer;
    BoxCollider2D _collider; 
    PowerUp_Shield _shieldRef;

    public static event Action<int, int> OnUpdateHealthBar;    //static event to update player health, observed in UI
    public static event Action<float, float> OnUpdateGasBar;  //static event to update player fuel, observed in UI
    public static event Action<int> OnUpdatePlayerLife;      //static event to update player chances, observed in UI
    public static event Action OnGameOver;                  //static event to send game over

    // Get references to the necessary components on this game object
    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _collider       = GetComponent<BoxCollider2D>();
        GameManager.OnCheckGameState += CheckGameStart;   //subscribe to game start event
    }

    //Update the player ship properties from config file.
    void ResetPlayerData()
    {
        _isPlayerAlive          = true;
        _maxPlayerHealth        = playerConfigData.GetMaxPlayerHealth;
        _maxPlayerFuel          = playerConfigData.GetMaxPlayerFuel;
        _fuelDepletionRate      = playerConfigData.GetFuelDepletionRate;
        _currentPlayerHealth    = _maxPlayerHealth;
        _currentPlayerFuel      = _maxPlayerFuel;
        _spriteRenderer.enabled = true;
        _collider.enabled       = true;
        trailEffect.SetActive(true);
        UpdateHealthData(true, _currentPlayerHealth);
        UpdateFuelData(true, _currentPlayerFuel);
        OnUpdatePlayerLife?.Invoke(_maxPlayerLives);
    }

    public void DepleteFuel()  //method to call in update to deplete fuel based on movement.
    {
        OnUpdateGasBar?.Invoke(_currentPlayerFuel -= _fuelDepletionRate * Time.deltaTime, _maxPlayerFuel); // deplete fuel with rate 
    }

    IEnumerator Blink(float duration, float blinkTime) // blink the player sprite when collision.
    {
        gameObject.layer = LayerMask.NameToLayer("IgnoreCollisions");  //assign to collision ignore layer

        while (duration > 0f)
        {
            duration -= Time.deltaTime;

            //toggle renderer
            _spriteRenderer.enabled = !_spriteRenderer.enabled;

            //wait for a bit
            yield return new WaitForSeconds(blinkTime);
        }
        gameObject.layer = originalLayer;  //revert back to original layer
        _spriteRenderer.enabled = true;
    }

    //Shiled power up method, which upadated no of hit and shield status when power up collected.
    public void ActivateShieldPower(int hits,bool status,PowerUp_Shield activeShield)
    {
        _shieldHits = hits;
        shield.SetActive(status);
        _shieldRef = activeShield;
    }

    //Update player health method
    public void UpdateHealthData(bool add,int value)
    {
        if (add)
        {
            if (_currentPlayerHealth + value <= _maxPlayerHealth)  // if less than max health add to current health
            {
                _currentPlayerHealth += value;
            }
            else
            {// if current health exceeds max health
                _currentPlayerHealth = _maxPlayerHealth;
            }
        }
        else
        {
            _currentPlayerHealth -= value;  // reduce from current health
        }

        // clamp current health to avoid negative values
        _currentPlayerHealth = Mathf.Clamp(_currentPlayerHealth, 0, _maxPlayerHealth);

        //invoke call observed on ui to update health
        OnUpdateHealthBar?.Invoke(_currentPlayerHealth, _maxPlayerHealth);
    }

    public void UpdateFuelData(bool add, float value)
    {
        if (add)
        {
            if (_currentPlayerFuel + value <= _maxPlayerFuel)  // if less than max fuel add to current fuel
            {
                _currentPlayerFuel += value;
            }
            else
            {// if current fuel exceeds max fuel
                _currentPlayerFuel = _maxPlayerFuel;
            }
        }
        else
        {
            _currentPlayerFuel -= value;  // reduce from current fuel
        }
        // clamp current fuel to avoid negative values
        _currentPlayerFuel = Mathf.Clamp(_currentPlayerFuel, 0f, _maxPlayerFuel);

        //invoke call observed on ui to update fuel
        OnUpdateGasBar?.Invoke(_currentPlayerFuel, _maxPlayerFuel);
    }

    //udpate player chances
    public void UpdatePlayerLife(bool add, int value)
    {
        if (add)
        {
            _maxPlayerLives += value;                               // add to max lives when powerup collecte
        }
        else
        {
            _maxPlayerLives -= value;                         // reduce max lives when player dies.
            _maxPlayerLives = Mathf.Max(_maxPlayerLives, 0); // ensure max lives is not negative
        }

        OnUpdatePlayerLife?.Invoke(_maxPlayerLives);
    }

    //This is a interface implemented function, which is called when player takes damage
    public void Damage(int dmg)
    {
        PlayerHit(dmg);
    }

    private void PlayerHit(int dmg)
    {
        if (_shieldHits != 0) // avoid damage when shield active
        {
            _shieldHits--;    // reduce shield hits
            if (_shieldHits == 0)
            {
                //call expire shield powerup
                try { _shieldRef.PowerUpHasExpired(); }catch { Debug.Log("Shield Already Destroyed"); } 
            }
            return;
        }

        UpdateHealthData(false, dmg);  

        if (_currentPlayerHealth <= 0)
        {
            //player dead
            StartCoroutine(PlayerDead());
        }
        else
        {
            StartCoroutine(Blink(0.05f, 0.12f));  //if taken damage blink player
        }
    }

    //Player dead cortoutine
    IEnumerator PlayerDead()
    {
        GameManager.Instance.DisableAndSpawnParticleEffect(transform.position,3f,3,"Ship"); //spawn particle effect
        _isPlayerAlive = false;
        _spriteRenderer.enabled = false;  //disable sprite 
        trailEffect.SetActive(false);
        _collider.enabled = false;       //disable collider
        UpdatePlayerLife(false,1);      //decrement player lives

        yield return new WaitForSeconds(5f); //wait for 5seconds

        if (_maxPlayerLives == 0)
        {
            //game over              // if last life used, call game over, observed on ui
            OnGameOver?.Invoke();
        }
        else
        {
            ResetPlayerData();         // if lives remain reset player and start again
            transform.position = new Vector3(0f, 0f, 0f); // reset player position to center
        }
    }

    private void OnDisable()
    {
        //unsubscribe to game start event on gameobject disable
        GameManager.OnCheckGameState -= CheckGameStart;  
    }

    //game start method to update player properties
    public void CheckGameStart(bool state)
    {
        _isgameStarted  = state;
        _maxPlayerLives = playerConfigData.GetMaxPlayerLives;
        ResetPlayerData();
    }
}
