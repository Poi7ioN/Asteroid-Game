using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//This scriptable object file holds list of sprites for asteroid and ememies.
[CreateAssetMenu(menuName = "Sprite Config Holder")]
public class SpriteConfigFile: ScriptableObject
{
    [Header("List of Sprite")]

    [Tooltip("List of sprites which are randomly selected")]
    [SerializeField] List<Sprite> _Sprites;

    public Sprite GetRandomSprites { get { return _Sprites[Random.Range(0, _Sprites.Count)]; } }
}
