using System.Collections.Generic;
using UnityEngine;

public class HarvestPool : MonoBehaviour
{
    private enum HarvestPools {ChickenHarvestPool = 0, DuckHarvestPool = 1, CowHarvestPool = 2 }

    [SerializeField] private HarvestPools harvestPools;

    private Queue<GameObject> poolHarvestObject = new();

    [SerializeField] private int poolSize;
    [SerializeField] private GameObject poolHarvestGameObject;

    private void Awake()
    {
        FillPool();
    }

    private void Start()
    {
        switch ((int)harvestPools)
        {
            case 0:
                GameManager.Instance.ReturnChickenHarvestPool.AddListener(ReturnHarvestPool);
                break;
            case 1:
                GameManager.Instance.ReturnDuckHarvestPool.AddListener(ReturnHarvestPool);
                break;
            case 2:
                GameManager.Instance.ReturnCowHarvestPool.AddListener(ReturnHarvestPool);
                break;
        }

    }

    public GameObject GetPooledHarvestObject()
    {
        GameObject harvest = poolHarvestObject.Dequeue();
        return harvest;
    }

    private void FillPool()
    {
        for (int i = 0; i < poolSize; i++)
        {

            GameObject harvest = Instantiate(poolHarvestGameObject, Vector3.zero, Quaternion.identity);
            harvest.SetActive(false);
            harvest.transform.parent = gameObject.transform;
            poolHarvestObject.Enqueue(harvest);
        }
    }
    public void ReturnHarvestPool(GameObject obj)
    {
        obj.SetActive(false);
        poolHarvestObject.Enqueue(obj);
    }
}
