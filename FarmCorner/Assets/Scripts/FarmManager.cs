using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using System.Timers;
using UnityEngine.UIElements;
using UnityEngine.SocialPlatforms;

public class FarmManager : MonoBehaviour
{
    #region Variables
    private bool loadControl = false;
    private int[] saveData = new int[6];
    public bool LoadControl
    {
        get
        {
            return this.loadControl;
        }
        set
        {
            loadControl = value;
        }
    }
    private enum FarmType { Chicken = 0, Duck = 1, Sheep = 2, Cow = 3 }
    [SerializeField] private FarmType farmType;
    public int FarmTypeInt
    {
        get
        {
            return (int)farmType;
        }
        private set
        {

        }
    }
    [SerializeField] private InGameManager inGameManager;
    [SerializeField] public AnimalPool animalPool;
    [SerializeField] public HarvestPool harvestPool;
    [SerializeField] private GameObject AnimalSpawnPoint;
    [SerializeField] private GameObject AnimalSpawnPointX;
    public Vector3 animalSpawnPoint
    {
        get
        {
            return AnimalSpawnPoint.transform.position;
        }
        private set
        {

        }
    }
    private List<AnimalManager> animalsList = new();
    private List<GameObject> harvestList = new();
    public List<GameObject> HarvestList
    {
        get
        {
            return harvestList;
        }
        set
        {
            harvestList = value;
        }
    }
    public List<AnimalManager> AnimalsList
    {
        get
        {
            return animalsList;
        }
        set
        {
            animalsList = value;
        }
    }

    public HarvestPool HarvestPool => harvestPool;

    [SerializeField] private float harvestTimer;
    public float HarvestTimer
    {
        get
        {
            return harvestTimer;
        }
        set
        {
            harvestTimer = value;
        }
    }
    

    private bool canBuy;
    public bool CanBuy
    {
        get
        {
            return canBuy;
        }
        private set
        {
            canBuy = value;
        }
    }

  
    int farmLevel = 0;
    public int FarmLevel
    {
        get
        {
            return farmLevel;
        }
        set
        {
            farmLevel = value;
        }
    }


    System.DateTime startTime;
    System.DateTime stopTime;
    System.TimeSpan Dif;
    private float sec, min, hour;
    public float Sec
    {
        get
        {
            return sec;
        }
        set
        {
            sec = value;
        }
    }
    

    
    private bool availableFarm = false;
    private int animalCount;
    public int AnimalCount
    {
        get
        {
            return animalCount;
        }
        set
        {
            animalCount = value;
        }
    }
    int tempFirstOpenFarm;
    public int TempFirstOpenFarm
    {
        get
        {
            return tempFirstOpenFarm;
        }
        set
        {
            tempFirstOpenFarm = value;
        }
    }
    #endregion

    #region Functions
    private void Awake()
    {

        if (PlayerPrefs.GetInt("FirstFarmOpen", 0)==1 && !loadControl)
        {
            Invoke(nameof(LoadAnimal), 0.1f);
            Invoke(nameof(LoadHarvest), 0.1f);
            loadControl = true;
        }
    }

    private void OnEnable()
    {
        if (PlayerPrefs.GetInt("FirstFarmOpen", 1)==0 && !loadControl)
        {
            Invoke(nameof(LoadAnimal), InGameManager._staticDuration);
            Invoke(nameof(LoadHarvest), InGameManager._staticDuration);
            loadControl = true;
        }
        PlayerPrefs.SetInt("FirstFarmOpen", 0);

        GameManager.Instance.OnOpenNewFarm.Invoke((int)farmType);

        if (availableFarm)
        {
            stopTime = System.DateTime.Now;
            Dif = stopTime - startTime;
            sec = Dif.Seconds;
            min = Dif.Minutes;
            if (min >= 1 && min < 60)
            {
                sec += min * 60;
                min = 0;
            }
            hour = Dif.Hours;
            if (hour >= 1 && hour < 24)
            {
                sec += hour * 3600;
                hour = 0;
            }
        }
        Debug.Log(sec);

    }
    private void OnDisable()
    {
        SaveData();
        if (availableFarm)
        {
            animalCount = animalsList.Count;
            startTime = System.DateTime.Now;
        }

    }
    public void GetMergedAnimal(int level)
    {
        if (level > 2) return;
        
        int minLevel = level;

        foreach (var animal in animalsList)
        {
            if (animal.Level == minLevel)
            {
                foreach (var secAnimal in animalsList)
                {
                    if (animal != secAnimal && secAnimal.Level == animal.Level)
                    {
                        MergePhase(animal, secAnimal);
                        return;
                    }
                }
            }
        }
        GetMergedAnimal(level + 1);
    }
    private void MergePhase(AnimalManager firstAnimal, AnimalManager secondAnimal)
    { 
        firstAnimal.gameObject.SetActive(false);
        firstAnimal.Level++;
        firstAnimal.gameObject.SetActive(true);
        firstAnimal._particleSystem.Play();
        secondAnimal.CloseAnimal();
        animalPool.ReturnAnimalPool(secondAnimal);
        animalsList.Remove(secondAnimal);
    }
    public void HarvestObject()
    {
        for (int i = 0; i < harvestList.Count; i++)
        {
            harvestList[i].SetActive(false);
            harvestPool.ReturnHarvestPool(harvestList[i]);
            if (i == harvestList.Count - 1)
            {
                for (int a = 0; a < harvestList.Count;)
                {
                    harvestList.Remove(harvestList[a]);
                }
            }
        }
    }
    public void BuyAnimal()
    {
        canBuy = true;
        availableFarm = true;
        AnimalManager animal = animalPool.GetPooledAnimalObject(); // POOL CEKME NOKTASI
        animal.farmManager = this;
        animal.Level = 1;
        animalsList.Add(animal);
        animal.gameObject.transform.position = AnimalSpawnPointX.transform.position;
        animal.enabled = true;
        animal.gameObject.SetActive(true);
        canBuy = false;
        if (FarmTypeInt == 2) ///// BURAYA BAKKKKKKKKKK KOYUN SURESI POSTTTT
        {
            animal.GetComponent<SheepSkinManager>().FarmManager = this;
        }
        SaveData();
    }

    public void CloseFarm()
    {
        gameObject.SetActive(false);
    }
    public void StopAnimal()
    {
        foreach (var animal in animalsList)
        {
            animal.CloseAnimal();
        }
    }
    public void CutSkin()
    {
        GameManager.Instance.HarvestWools.Invoke();
    }

    public void SaveData()
    {
        foreach (var item in animalsList)
        {
            switch (item.Level)
            {
                case 1:
                    saveData[0]++;
                    break;
                case 2:
                    saveData[1]++;
                    break;
                case 3:
                    saveData[2]++;
                    break;
            }
        }
        if (FarmTypeInt!=2)
        {
            foreach (var item in harvestList)
            {
                switch (item.tag)
                {
                    case "level1":
                        saveData[3]++;
                        break;
                    case "level2":
                        saveData[4]++;
                        break;
                    case "level3":
                        saveData[5]++;
                        break;
                }
            }
        }
        PlayerPrefs.SetInt($"{FarmTypeInt}SavedAnimal1", saveData[0]);
        PlayerPrefs.SetInt($"{FarmTypeInt}SavedAnimal2", saveData[1]);
        PlayerPrefs.SetInt($"{FarmTypeInt}SavedAnimal3", saveData[2]);
        PlayerPrefs.SetInt($"{FarmTypeInt}SavedProduct1", saveData[3]);
        PlayerPrefs.SetInt($"{FarmTypeInt}SavedProduct2", saveData[4]);
        PlayerPrefs.SetInt($"{FarmTypeInt}SavedProduct3", saveData[5]);

        for (int i = 0; i < saveData.Length; i++)
        {
            saveData[i] = 0;
        }
    }
    void LoadAnimal()
    {
        int total1, total2, total3;
        total1 = PlayerPrefs.GetInt($"{FarmTypeInt}SavedAnimal1", saveData[0]);
        total2 = PlayerPrefs.GetInt($"{FarmTypeInt}SavedAnimal2", saveData[1]);
        total3 = PlayerPrefs.GetInt($"{FarmTypeInt}SavedAnimal3", saveData[2]);
        for (int a = 0; a < total1 + total2 + total3; a++)
        {
            canBuy = true;
            availableFarm = true;
            AnimalManager animal = animalPool.GetPooledAnimalObject(); // POOL CEKME NOKTASI
            if (a<total1)
            {
                animal.Level = 1;
            }
            else if (a < total1 + total2)
            {
                animal.Level = 2;
            }
            else if (a < total1 + total2 + total3)
            {
                animal.Level = 3;
            }

            animal.farmManager = this;
            
            animalsList.Add(animal);
            animal.gameObject.transform.position = AnimalSpawnPointX.transform.position;
            animal.enabled = true;
            animal.gameObject.SetActive(true);
            canBuy = false;
            if (FarmTypeInt == 2) ///// BURAYA BAKKKKKKKKKK KOYUN SURESI POSTTTT
            {
                animal.GetComponent<SheepSkinManager>().FarmManager = this;
            }
        }
    }
    void LoadHarvest()
    {
        int total1, total2, total3;
        total1 = PlayerPrefs.GetInt($"{FarmTypeInt}SavedProduct1", saveData[3]);
        total2 = PlayerPrefs.GetInt($"{FarmTypeInt}SavedProduct2", saveData[4]);
        total3 = PlayerPrefs.GetInt($"{FarmTypeInt}SavedProduct3", saveData[5]);

        for (int i = 0; i < total1 + total2 + total3; i++)
        {
            if (FarmTypeInt != 2)
            {
                var pointX = Random.Range(-1.8f, 1.8f);
                var pointZ = Random.Range(-0.9f, 5.4f);
                GameObject harvest = harvestPool.GetPooledHarvestObject();
                if (i < total1)
                {
                    harvest.tag = "level1";
                }
                else if (i < total1 + total2)
                {
                    harvest.tag = "level2";
                }
                else if (i < total1 + total2 + total3)
                {
                    harvest.tag = "level3";
                }
                HarvestList.Add(harvest);
                switch (FarmTypeInt)
                {
                    
                    case 0:
                        harvest.transform.localPosition = new Vector3(pointX, 0.03666667f, pointZ);
                        break;
                    case 1:
                        harvest.transform.localPosition = new Vector3(pointX, 0.03666667f, pointZ);
                        break;
                    case 2:
                        harvest.transform.localPosition = new Vector3(pointX, 0.03666667f, pointZ);
                        break;
                    case 3:
                        harvest.transform.localPosition = new Vector3(pointX, 0.03666667f, pointZ);
                        break;
                }
                
                harvest.SetActive(true);
                GameManager.Instance.ButtonUpdate.Invoke();
                
            }
        }
        //if (harvestList.Count>0)
        //{
        //    GameManager.Instance.HarvestButtonUpdate.Invoke();
        //}
    }

    #endregion


}
