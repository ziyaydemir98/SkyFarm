using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

public class AnimalManager : MonoBehaviour
{

    #region Variables
    public delegate void Animation();
    public event Animation IsWalkEvent;
    public event Animation IsIdleEvent;
    [SerializeField] public ParticleSystem _particleSystem;
    private enum AnimalType { Chicken = 0, Duck = 1, Sheep = 2, Cow = 3 }
    [SerializeField] private AnimalType animalType;
    public int animalTypeInt
    {
        get
        {
            return (int)animalType;
        }
        private set
        {

        }
    }
    [SerializeField] private SheepSkinManager sheepSkinManager;
    [HideInInspector] public UnityEvent OnLevelChanged = new();
    [SerializeField] private List<GameObject> levelVisuals = new();
    private NavMeshAgent _agent;
    public FarmManager farmManager { get; set; }
    public InGameManager inGameManager { get; set; }

    [Header("Animation Times")]
    [SerializeField] float IdleTimeMin;
    [SerializeField] float IdleTimeMax;
    private int _level = 1;
    public int Level
    {
        get => _level;
        set
        {
            _level = value;
            SetVisual();
            OnLevelChanged.Invoke();
        }
    }
    bool start = true;
    bool waitIdle;
    float random, delayCount;

    #endregion

    #region Functions
    void Awake()
    {
        random = Random.Range(IdleTimeMin, IdleTimeMax);
        _agent = GetComponent<NavMeshAgent>();

    }
    private void OnEnable()
    {
        if (sheepSkinManager)
        {
            Invoke(nameof(InvokedSkinManager), 0.1f);
        }
        InvokeRepeating(nameof(HarvestSpawner), farmManager.HarvestTimer, farmManager.HarvestTimer);
        DelayHarvest();

        if (start == true || farmManager.CanBuy == true)
        {
            StartAnimal();
            start = false;
        }
        else
        {
            OpenAnimal();
        }
        SetVisual();
    }

    private void OnDisable()
    {
        CancelInvoke(nameof(HarvestSpawner));
    }
    void HarvestSpawner()
    {
        SpawnCount(this.gameObject.transform.position);
        
    }
    private void SpawnCount(Vector3 pos, bool isLocal = false)
    {

        if (farmManager.FarmTypeInt != 2)
        {
            GameObject harvest = farmManager.harvestPool.GetPooledHarvestObject();

            switch (Level)
            {
                case 1:
                    harvest.tag = "level1";
                    break;
                case 2:
                    harvest.tag = "level2";
                    break;
                case 3:
                    harvest.tag = "level3";
                    break;
            }

            farmManager.HarvestList.Add(harvest);

            if (isLocal)
                harvest.transform.localPosition = pos;
            else
                harvest.transform.position = pos;

            harvest.SetActive(true);
            GameManager.Instance.HarvestButtonUpdate.Invoke();
        }
    }
    private void SetRandomDestination()
    {
        _agent.SetDestination(GetRandomPoint());


        if (_agent.hasPath)
        {
            
            CancelInvoke(nameof(SetRandomDestination));
            IsWalkEvent.Invoke();
            InvokeRepeating(nameof(Check), 0.2f, 0.01f);
        }
    }
    private void Check()
    {
        if (_agent.velocity.magnitude < 0.05f)
        {
            CancelInvoke(nameof(Check));
            IsIdleEvent.Invoke();
            StartCoroutine(IdleTiming());

        }
    }
    IEnumerator IdleTiming()
    {
        yield return new WaitForSeconds(random);
        InvokeRepeating(nameof(SetRandomDestination), 0f, 0.01f);
    }
    private Vector3 GetRandomPoint()
    {
        var pointX = Random.Range(farmManager.animalSpawnPoint.x + -1.8f, farmManager.animalSpawnPoint.x + 1.8f);
        var pointZ = Random.Range(farmManager.animalSpawnPoint.z + -3.2f, farmManager.animalSpawnPoint.z + 3.2f);
        return new Vector3(pointX, 0, pointZ);
    }
    public void StartAnimal()
    {
        _agent.enabled = true;
        _agent.isStopped = false;
        InvokeRepeating(nameof(SetRandomDestination), 0f, 0.01f);
    }
    private void OpenAnimal()
    {
        Invoke(nameof(SetRandomDestinationInvokeRepating), InGameManager._staticDuration);
    }
    public void CloseAnimal()
    {
        StopAllCoroutines();
        CancelInvoke(nameof(Check));
        CancelInvoke(nameof(SetRandomDestination));
        _agent.isStopped = true;
        _agent.enabled = false;
    }
    public GameObject GetCurrentAnimalObject()
    {
        
        if (this.animalTypeInt != 2)
        {
            return levelVisuals[Level - 1];
        }
        else
        {
            return levelVisuals[Level -1].transform.GetChild(2).transform.gameObject;
        }
    }
    private void SetVisual()
    {
        foreach (var visual in levelVisuals)
        {
            visual.SetActive(false);
        }
        levelVisuals[Level - 1].SetActive(true);
    }
    private void SetRandomDestinationInvokeRepating()
    {
        _agent.enabled = true;
        _agent.isStopped = false;
        InvokeRepeating(nameof(SetRandomDestination), 0f, 0.01f);
    }
    private void DelayHarvest()
    {
        if (farmManager.Sec >= farmManager.HarvestTimer && farmManager.Sec > 0.01f) // HASAT SURESINDEN UZUN BEKLENDIYSE
        {
            delayCount = farmManager.Sec / farmManager.HarvestTimer;
            int tempDelayCount = (int)delayCount * farmManager.AnimalCount;
            OneBiggerDelayHarvestCreated(tempDelayCount);
            farmManager.Sec = 0;
        }
        else if (farmManager.Sec < farmManager.HarvestTimer && farmManager.Sec > 0.01f) // HASAR SURESINDEN KISA ZAMAN GECTIYSE
        {
            float tempTimer;
            tempTimer = farmManager.HarvestTimer - farmManager.Sec; ;
            Invoke(nameof(OneLowerDelayHarvestCreated), tempTimer);
            farmManager.Sec = 0;
        }
    }
    void OneBiggerDelayHarvestCreated(int index)
    {
        for (int i = 0; i < index; i++)
        {
            var pointX = Random.Range(-1.8f, 1.8f);
            var pointZ = Random.Range(-0.9f, 5.4f);
            SpawnCount(new Vector3(pointX, 0.03666667f, pointZ), true);
        }
    }
    void OneLowerDelayHarvestCreated()
    {
        SpawnCount(this.gameObject.transform.position);
    }
    void InvokedSkinManager()
    {
        sheepSkinManager.FarmManager = farmManager;
        sheepSkinManager.enabled = true;
    }

    #endregion

}
