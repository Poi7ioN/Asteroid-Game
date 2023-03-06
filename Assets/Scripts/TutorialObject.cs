using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//This is a tutorial script on asteroid in the cutscene
public class TutorialObject : MonoBehaviour, IDamagable
{
    public void Damage(int dmg)
    {
        GameManager.Instance.DisableAndSpawnParticleEffect(transform.position,3f,2, "Asteroid");
        gameObject.SetActive(false);
    }
}
