using System.Collections.Generic;
using UnityEngine;

public class AnimalPool : MonoBehaviour
{
    private enum AnimalPools { ChickenPool = 0, DuckPool = 1, SheepPool = 2, CowPool = 3}

    [SerializeField] private AnimalPools animalPools;

    private Queue<AnimalManager> poolAnimalObject = new();

    [SerializeField] private int poolSize;
    [SerializeField] private AnimalManager poolAnimalGameObject;

    private void Awake()
    {
        FillPool();
    }

    private void Start()
    {
        switch ((int)animalPools)
        {
            case 0:
                GameManager.Instance.ReturnChickenPool.AddListener(ReturnAnimalPool);
                break;
            case 1:
                GameManager.Instance.ReturnDuckPool.AddListener(ReturnAnimalPool);
                break;
            case 2:
                GameManager.Instance.ReturnSheepPool.AddListener(ReturnAnimalPool);
                break;
            case 3:
                GameManager.Instance.ReturnCowPool.AddListener(ReturnAnimalPool);
                break;
        }

    }

    public AnimalManager GetPooledAnimalObject()
    {
        AnimalManager animal = poolAnimalObject.Dequeue();
        return animal;
        
    }

    private void FillPool()
    {
        for (int i = 0; i < poolSize; i++)
        {
            AnimalManager animal = Instantiate(poolAnimalGameObject, Vector3.zero, Quaternion.identity);
            animal.gameObject.SetActive(false);
            animal.transform.parent = gameObject.transform;
            poolAnimalObject.Enqueue(animal);
        }
    }

    public void ReturnAnimalPool(AnimalManager obj)
    {
        obj.enabled = false;
        obj.gameObject.SetActive(false);
        poolAnimalObject.Enqueue(obj);
    }
}
