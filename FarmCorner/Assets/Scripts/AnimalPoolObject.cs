using UnityEngine;

public class AnimalPoolObject : MonoBehaviour
{
    private enum AnimalPools { ChickenPool = 0, DuckPool = 1, SheepPool = 2, CowPool = 3 }
    [SerializeField] private AnimalPools animalPools;
    [SerializeField] private AnimalManager animalManager;

    //[SerializeField] private float returnPoolTime;
    //private void OnEnable()
    //{
    //    Invoke(nameof(ReturnHome), returnPoolTime);
    //}

    private void ReturnHome()
    {
        switch ((int)animalPools)
        {
            case 0:
                GameManager.Instance.ReturnChickenPool.Invoke(animalManager);
                break;
            case 1:
                GameManager.Instance.ReturnDuckPool.Invoke(animalManager);
                break;
            case 2:
                GameManager.Instance.ReturnSheepPool.Invoke(animalManager);
                break;
            case 3:
                GameManager.Instance.ReturnCowPool.Invoke(animalManager);
                break;
        }
    }
}
