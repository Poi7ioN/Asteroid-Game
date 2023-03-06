using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Define a class to hold information about the objects to pool
[System.Serializable]
public class ObjectPoolItem
{
    public int amountToPool;          // The number of objects to pool
    public GameObject objectToPool;  // The prefab of the object to pool
    public bool shouldExpand;       // Whether the pool should be expanded if all objects are used
}

// Define a class for the object pooler
public class ObjectPooler : MonoBehaviour
{
    public List<ObjectPoolItem> itemsToPool;    // A list of objects to pool
    public static ObjectPooler SharedInstance; // A static reference to the object pooler
    public List<GameObject> pooledObjects;    // A list of pooled objects
    public GameObject PooledObjects;         // The parent object for the pooled objects in hierarchy

    void Awake()
    {
        SharedInstance = this;
        GameManager.OnCheckGameState += CheckGameStart;  // Subscribe to the game state event for receving game start event
    }

    // Start is called before the first frame update
    void Start()
    {
        pooledObjects = new List<GameObject>();
        foreach (ObjectPoolItem item in itemsToPool)
        {
            for (int i = 0; i < item.amountToPool; i++)
            {
                GameObject obj = Instantiate(item.objectToPool) as GameObject; // Instantiate the object
                obj.SetActive(false);                                         // Deactivate the object
                obj.transform.parent = PooledObjects.transform;              // Set the object's parent to the pooled objects parent
                pooledObjects.Add(obj);                                     // Add the object to the pooled objects list
            }
        }
    }

    // Get a pooled object of a specific type
    public GameObject GetPooledObject(GameObject pooledObj)
    {
        // loop through all pooled objects to find an inactive object with the same name as the requested object
        for (int i = 0; i < pooledObjects.Count; i++)
        {
            if (!pooledObjects[i].activeInHierarchy && pooledObjects[i].gameObject.name  == pooledObj.gameObject.name + "(Clone)")
            {
                return pooledObjects[i];
            }
        }

        // if no inactive object with the same name is found, create a new object if expansion is allowed
        foreach (ObjectPoolItem item in itemsToPool)
        {
            if (item.objectToPool.gameObject.name == pooledObj.gameObject.name)
            {
                if (item.shouldExpand)
                {
                    GameObject obj = Instantiate(item.objectToPool) as GameObject;
                    obj.SetActive(false);
                    obj.transform.parent = PooledObjects.transform;
                    pooledObjects.Add(obj);
                    return obj;
                }
            }
        }
        return null;
    }

    // Deactivate all pooled objects on game restart
    public void ResetAllPooledObjects()
    {
        for (int i = 0; i < pooledObjects.Count; i++)
        {
            if (pooledObjects[i].activeInHierarchy)
            {
                pooledObjects[i].SetActive(false);
            }
        }
    }

    // Unsubscribe to game state event on disable
    private void OnDisable()
    {
        GameManager.OnCheckGameState -= CheckGameStart;
    }

    // Check if the game is starting and reset all pooled objects if it is
    public void CheckGameStart(bool state)
    {
        if(!state) { ResetAllPooledObjects(); }
    }
}
