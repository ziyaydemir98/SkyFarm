
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    [Serializable]
    public struct Pool
    {
        public Queue<GameObject> pooledObject;
        public GameObject prefabObject;
        public int poolSize;
    }
    [SerializeField] public Pool[] pools = null;
    private void Awake()
    {
        for (int i = 0; i < pools.Length; i++)
        {
            pools[i].pooledObject = new Queue<GameObject>();
            for (int a = 0; a < pools[i].poolSize ; a++)    
            {
                GameObject tempObject = Instantiate(pools[i].prefabObject);
                tempObject.SetActive(false);
                pools[i].pooledObject.Enqueue(tempObject);
            }
        }
    }
    public GameObject CallPoolObject(int objectType)
    {
        if (objectType >= pools.Length) return null;
        if (pools[objectType].pooledObject.Count == 0)
        {
            AddSizePool(5, objectType);
        }           
        GameObject tempObject = pools[objectType].pooledObject.Dequeue();
        tempObject.SetActive(true);
        return tempObject;
    }    
    public void SendPoolObject(GameObject sendedObject, int objectType)
    {
        if (objectType >= pools.Length) return;
        pools[objectType].pooledObject.Enqueue(sendedObject);
        sendedObject.SetActive(false);
    }
    public void AddSizePool(int amount, int objectType)
    {
        for (int i = 0; i < amount; i++)
        {
            GameObject tempObject = Instantiate(pools[objectType].prefabObject);
            tempObject.SetActive(false);
            pools[objectType].pooledObject.Enqueue(tempObject);
        }
    }
    
}
