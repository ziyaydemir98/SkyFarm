using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.UIElements;
using TMPro;
using UnityEngine;
using System;

public class GrowRateButton : MonoBehaviour
{
    [SerializeField] private UnityEngine.UI.Button button;
    [SerializeField] private TextMeshProUGUI UpgradePriceText;
    [SerializeField] private TextMeshProUGUI UpgradeCountText;
    [SerializeField] private TextMeshProUGUI UpgradeRateText;
    [SerializeField] private FarmManager farmManager;
    [SerializeField] private string dataName;
    [SerializeField] private List<int> Prices = new();
    [SerializeField] private float temporaryTime;

    private float removedTime;
    float percentFloat;
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
        ButtonUpdateRate();
    }
    private void OnEnable()
    {
        Load();
        ButtonUpdateRate();
        Invoke(nameof(ButtonUpdateRate), 0.1f);
        if (PlayerPrefs.GetInt("FirstOpen", 0) == 1)
            Invoke(nameof(ButtonUpdateRate), InGameManager._staticDuration);
        GameManager.Instance.ButtonUpdate.AddListener(ButtonUpdateRate);
        button.onClick.AddListener(UpgradeProductGrowRate);
    }
    private void OnDisable()
    {
        Save();
        button.onClick.RemoveListener(UpgradeProductGrowRate);
        GameManager.Instance?.ButtonUpdate.RemoveListener(ButtonUpdateRate);
    }
    void UpgradeProductGrowRate()
    {
        removedTime = temporaryTime / 1000;
        temporaryTime -= removedTime;
        farmManager.HarvestTimer = temporaryTime;
        GameManager.Instance.PlayerMoney -= Prices[count];
        count++;
        ButtonUpdateRate();
        Save();
        GameManager.Instance.ButtonUpdate.Invoke();
    }
    void ButtonUpdateRate()
    {
        if (GameManager.Instance.PlayerMoney < Prices[count] || (count+1) >= Prices.Count)
        {
            button.interactable = false;
        }
        else button.interactable = true;
        TextUpdate();
    }
    void TextUpdate()
    {
        float temp;
        if (button.interactable)
        {
            UpgradePriceText.text = Prices[count].ToString();
            UpgradeCountText.text = (count+1) + " / " + (Prices.Count - 1);
            temp = removedTime / temporaryTime;
            percentFloat = (float)Math.Round(temp, 3);
            UpgradeRateText.text = "%" + (percentFloat*100).ToString() +" Shorter";
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
        count = PlayerPrefs.GetInt($"RateCount{dataName}", 0);
        temporaryTime = PlayerPrefs.GetFloat($"GrowTime{dataName}",temporaryTime);
        removedTime = temporaryTime / 1000;
        farmManager.HarvestTimer = temporaryTime;
    }
    void Save()
    {
        PlayerPrefs.SetInt($"RateCount{dataName}", count);
        PlayerPrefs.SetFloat($"GrowTime{dataName}", temporaryTime);
    }

}