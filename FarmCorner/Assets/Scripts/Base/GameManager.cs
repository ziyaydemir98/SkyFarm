using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GameManager : Singleton<GameManager>
{
    [HideInInspector] public UnityEvent GameStart = new();
    [HideInInspector] public UnityEvent GameReady = new();
    [HideInInspector] public UnityEvent GameEnd = new();
    [HideInInspector] public UnityEvent LevelSuccess = new();
    [HideInInspector] public UnityEvent LevelFail = new();
    [HideInInspector] public UnityEvent OnMoneyChange = new();

    [HideInInspector] public UnityEvent isWalk = new();
    [HideInInspector] public UnityEvent isIdle = new();
    [HideInInspector] public UnityEvent HarvestWools = new();
    [HideInInspector] public UnityEvent ButtonUpdate = new();
    [HideInInspector] public UnityEvent HarvestButtonUpdate = new();
    [HideInInspector] public UnityEvent<int> OnOpenUnlockedFarm = new();
    [HideInInspector] public UnityEvent<int> OnOpenNewFarm = new();
    [HideInInspector] public UnityEvent<AnimalManager> ReturnChickenPool = new();
    [HideInInspector] public UnityEvent<AnimalManager> ReturnDuckPool = new();
    [HideInInspector] public UnityEvent<AnimalManager> ReturnSheepPool = new();
    [HideInInspector] public UnityEvent<AnimalManager> ReturnCowPool = new();
    [HideInInspector] public UnityEvent<GameObject> ReturnChickenHarvestPool = new();
    [HideInInspector] public UnityEvent<GameObject> ReturnDuckHarvestPool = new();
    [HideInInspector] public UnityEvent<GameObject> ReturnCowHarvestPool = new();


    private float playerMoney = 1000000;
    public float PlayerMoney
    {
        get
        {
            return playerMoney;
        }
        set
        {
            playerMoney = value;
            OnMoneyChange.Invoke();
        }
    }

    private bool hasGameStart;
    public bool HasGameStart
    {
        get
        {
            return hasGameStart;
        }
        set
        {
            hasGameStart = value;
        }
    }

    private void Awake()
    {
        LoadData();
    }

    public void LevelState(bool value)
    {
        if (value) LevelSuccess.Invoke();
        else LevelFail.Invoke();
    }

    private void OnEnable()
    {
        GameStart.AddListener(() => hasGameStart = true);
        GameEnd.AddListener(() => hasGameStart = false);
    }

    private void OnDisable()
    {
        SaveData();
    }

    void LoadData()
    {
        playerMoney = PlayerPrefs.GetFloat("PlayerMoney", 1000000);
    }

    void SaveData()
    {
        PlayerPrefs.SetFloat("PlayerMoney", playerMoney);
    }
}
