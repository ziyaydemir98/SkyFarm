using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BuyButton : MonoBehaviour
{
    [SerializeField] private Button button;
    [SerializeField] private FarmLevelUpButton farmLevelUpButton;
    [SerializeField] private TextMeshProUGUI BuyButtonText;
    [SerializeField] private TextMeshProUGUI AnimalCountText;
    [SerializeField] private TextMeshProUGUI BuyButtonPriceText;
    [SerializeField] private FarmManager farmManager;
    [SerializeField] private string dataName;
    [SerializeField] private List<int> Prices = new();
    private int price = new();
    private int count = new();
    private int localCapacity = new();


    private void Awake()
    {
        switch (farmManager.FarmTypeInt)
        {
            case 0:
                for (int i = 1; i < 200; i++)
                {
                    Prices.Add(i * 25);
                }
                break;
            case 1:
                for (int i = 1; i < 200; i++)
                {
                    Prices.Add(i * 50);
                }
                break;
            case 2:
                for (int i = 1; i < 200; i++)
                {
                    Prices.Add(i * 100);
                }
                break;
            case 3:
                for (int i = 1; i < 200; i++)
                {
                    Prices.Add(i * 200);
                }
                break;
        }
        
        Load();
        ButtonUpdateBuy();
        
    }
    private void OnEnable()
    {
        button.onClick.AddListener(Buy);
        GameManager.Instance.ButtonUpdate.AddListener(ButtonUpdateBuy);
        Load();
        ButtonUpdateBuy();
        Invoke(nameof(ButtonUpdateBuy), 0.1f);
        if (PlayerPrefs.GetInt("FirstOpen", 0) == 1)
            Invoke(nameof(ButtonUpdateBuy), InGameManager._staticDuration);

    }
    private void OnDisable()
    {
        button.onClick.RemoveListener(Buy);
        GameManager.Instance?.ButtonUpdate.RemoveListener(ButtonUpdateBuy);
        Save();
    }
    private void Buy()
    {
        farmManager.BuyAnimal();
        GameManager.Instance.PlayerMoney -= price;
        ButtonUpdateBuy();
        Save();
        GameManager.Instance.ButtonUpdate.Invoke();
    }
    
    void ButtonUpdateBuy()
    {
        DataUpdate();
        localCapacity = PlayerPrefs.GetInt($"FarmCapacity{dataName}", farmLevelUpButton.FarmCapacity[farmLevelUpButton.Count]);
        TextUpdate();
        if (GameManager.Instance.PlayerMoney >= price && count < localCapacity) button.interactable = true;
        else button.interactable = false;
    }
    void DataUpdate()
    {
        count = farmManager.AnimalsList.Count;
        price = Prices[count];
    }
    void TextUpdate()
    {
        BuyButtonText.text = $"Buy {dataName} ";
        BuyButtonPriceText.text = price.ToString();
        AnimalCountText.text = count.ToString();
        if (GameManager.Instance.PlayerMoney < price)
        {
            BuyButtonText.text = "Need Money";
        }
        else if (count >= localCapacity)
        {
            BuyButtonText.text = "Need Capacity";
        }
    }
    void Load()
    {
        count = PlayerPrefs.GetInt($"AnimalCount{dataName}", farmManager.AnimalsList.Count);
        price = PlayerPrefs.GetInt($"AnimalPrice{dataName}", Prices[count]);
        
    }
    void Save()
    {
        PlayerPrefs.SetInt($"AnimalPrice{dataName}", price);
        PlayerPrefs.SetInt($"AnimalCount{dataName}", farmManager.AnimalsList.Count);
    }
}
