using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ProductValueButton : MonoBehaviour
{
    [SerializeField] HarvestButton harvestButton;
    [SerializeField] private UnityEngine.UI.Button button;
    [SerializeField] private TextMeshProUGUI UpgradePriceText;
    [SerializeField] private TextMeshProUGUI UpgradeCountText;
    [SerializeField] private TextMeshProUGUI UpgradeRateText;
    [SerializeField] private FarmManager farmManager;
    [SerializeField] private string dataName;
    [SerializeField] private List<int> Prices = new();
    [SerializeField] private float addedValue;
    private float temporaryValue;
    private int count;


    private void Awake()
    {
        switch (farmManager.FarmTypeInt)
        {
            case 0:
                for (int i = 1; i < 252; i++)
                {
                    Prices.Add(i * 50);
                }
                break;
            case 1:
                for (int i = 1; i < 252; i++)
                {
                    Prices.Add(i * 75);
                }
                break;
            case 2:
                for (int i = 1; i < 252; i++)
                {
                    Prices.Add(i * 100);
                }
                break;
            case 3:
                for (int i = 1; i < 252; i++)
                {
                    Prices.Add(i * 200);
                }
                break;
        }
        button.interactable = false;
        Load();
        ButtonUpdateValue();
        harvestButton.PricesProp[0] += temporaryValue;
        harvestButton.PricesProp[1] += temporaryValue * 2;
        harvestButton.PricesProp[2] += temporaryValue * 3;
    }
    private void OnEnable()
    {
        Load();
        ButtonUpdateValue();
        Invoke(nameof(ButtonUpdateValue), 0.1f);
        if (PlayerPrefs.GetInt("FirstOpen", 0) == 1)
            Invoke(nameof(ButtonUpdateValue), InGameManager._staticDuration);
        GameManager.Instance.ButtonUpdate.AddListener(ButtonUpdateValue);
        button.onClick.AddListener(UpgradeProductGrowRate);
    }
    private void OnDisable()
    {
        Save();
        button.onClick.RemoveListener(UpgradeProductGrowRate);
        GameManager.Instance?.ButtonUpdate.RemoveListener(ButtonUpdateValue);
    }
    void UpgradeProductGrowRate()
    {

        temporaryValue += addedValue;
        harvestButton.PricesProp[0] += addedValue;
        harvestButton.PricesProp[1] += addedValue * 2;
        harvestButton.PricesProp[2] += addedValue * 3;
        GameManager.Instance.PlayerMoney -= Prices[count];
        count++;
        ButtonUpdateValue();
        Save();
        GameManager.Instance.ButtonUpdate.Invoke();
    }
    void ButtonUpdateValue()
    {
        
        if (GameManager.Instance.PlayerMoney < Prices[count] || (count + 1) >= Prices.Count)
        {
            button.interactable = false;
        }
        else button.interactable = true;
        TextUpdate();
    }
    void TextUpdate()
    {
        if (button.interactable)
        {
            UpgradePriceText.text = Prices[count].ToString();
            UpgradeCountText.text = (count+1) + " / " + (Prices.Count - 1);
            UpgradeRateText.text = "+" + addedValue.ToString() + " Increase";
        }
        else
        {
            if (GameManager.Instance.PlayerMoney < Prices[count])
            {
                UpgradePriceText.text = " ";
                UpgradeCountText.text = "Need Money";
                UpgradeRateText.text = " ";
            }
            else
            {
                UpgradePriceText.text = " ";
                UpgradeCountText.text = "MAX";
                UpgradeRateText.text = " ";
            }
            
        }
        
    }
    void Load()
    {
        count = PlayerPrefs.GetInt($"ValueCount{dataName}", 0);
        temporaryValue = PlayerPrefs.GetFloat($"ProductValue{dataName}", 0f);
    }
    void Save()
    {
        PlayerPrefs.SetInt($"ValueCount{dataName}", count);
        PlayerPrefs.SetFloat($"ProductValue{dataName}", temporaryValue);
    }
}
