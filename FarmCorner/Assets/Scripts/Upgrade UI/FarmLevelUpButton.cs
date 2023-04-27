using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FarmLevelUpButton : MonoBehaviour
{
    [SerializeField] ParticleSystem _particleSystem;
    [SerializeField] private Button button;
    [SerializeField] private TextMeshProUGUI FarmUpgradeButtonPriceText;
    [SerializeField] private TextMeshProUGUI FarmCapacityCountText;
    [SerializeField] private TextMeshProUGUI FarmLevelCountText;
    [SerializeField] private FarmManager farmManager;
    [SerializeField] private string dataName;
    [SerializeField] private List<GameObject> farmList = new();
    [SerializeField] private List<int> Prices = new();
    [SerializeField] public List<int> FarmCapacity = new();
    private int price, localCapacity = new();
    private int count;
    public int Count
    {
        get
        {
            return count;
        }
        set
        {
            count = value;
        }
    }
    private void Awake()
    {
        Load();
        ButtonUpdate();
        farmManager.animalPool.AddPool(FarmCapacity[count]);
    }
    private void OnEnable()
    {
        
        button.onClick.AddListener(FarmUpgrade);
        Load();
        ButtonUpdate();
        
        GameManager.Instance.ButtonUpdate.AddListener(ButtonUpdate);
    }
    private void OnDisable()
    {
        button.onClick.RemoveListener(FarmUpgrade);
        Save();
        GameManager.Instance?.ButtonUpdate.RemoveListener(ButtonUpdate);
    }
    private void FarmUpgrade()
    {
        farmList[count].SetActive(false);
        //GameManager.Instance.PlayerMoney -= price;
        count++;
        ButtonUpdate();
        farmList[count].SetActive(true);
        Save();
        _particleSystem.Play();
        farmManager.animalPool.poolsize = FarmCapacity[count];
        farmManager.animalPool.AddPool(1);
        GameManager.Instance.ButtonUpdate.Invoke();
    }

    void ButtonUpdate()
    {
        
        DataUpdate();
        TextUpdate();
        if (GameManager.Instance.PlayerMoney < price || count+1 == farmList.Count) button.interactable = false;
        else button.interactable = true;
    }
    
    void DataUpdate()
    {    
        price = Prices[count];
        localCapacity = FarmCapacity[count];
    }
    void TextUpdate()
    {
        FarmUpgradeButtonPriceText.text = "Level Up " + price.ToString();
        FarmCapacityCountText.text = "Capacity " + localCapacity.ToString();
        FarmLevelCountText.text = "Farm Level " + (count+1).ToString();
        if (GameManager.Instance.PlayerMoney < price) FarmUpgradeButtonPriceText.text = "Need Money";
        else if (count+1 == farmList.Count) FarmUpgradeButtonPriceText.text = "MAX";
    }
    void Load()
    {
        
        count = PlayerPrefs.GetInt($"FarmLevelCount{dataName}", 0);
        price = PlayerPrefs.GetInt($"FarmUpgradePrice{dataName}", Prices[count]);
        localCapacity = PlayerPrefs.GetInt($"FarmCapacity{dataName}", FarmCapacity[count]);
        farmList[0].SetActive(false);
        farmList[count].SetActive(true);
    }
    void Save()
    {
        PlayerPrefs.SetInt($"FarmLevelCount{dataName}", count);
        PlayerPrefs.SetInt($"FarmUpgradePrice{dataName}", price);
        PlayerPrefs.SetInt($"FarmCapacity{dataName}", localCapacity);
    }
}
