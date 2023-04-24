using UnityEngine;

public class HarvestPoolObject : MonoBehaviour
{
    private enum HarvestPools { ChickenHarvestPool = 0, DuckHarvestPool = 1, CowHarvestPool = 2 }
    [SerializeField] private HarvestPools harvestPools;
    [SerializeField] private new GameObject gameObject;

    //[SerializeField] private float returnPoolTime;
    //private void OnEnable()
    //{
    //    Invoke(nameof(ReturnHome), returnPoolTime);
    //}

    private void ReturnHome()
    {
        switch ((int)harvestPools)
        {
            case 0:
                GameManager.Instance.ReturnChickenHarvestPool.Invoke(gameObject);
                break;
            case 1:
                GameManager.Instance.ReturnDuckHarvestPool.Invoke(gameObject);
                break;
            case 2:
                GameManager.Instance.ReturnCowHarvestPool.Invoke(gameObject);
                break;
        }

    }
}
