using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine.UI;
using UnityEngine.UIElements;
using TMPro;
using UnityEngine;


public class MergeButton : MonoBehaviour
{
    [SerializeField] private UnityEngine.UI.Button button;
    [SerializeField] private FarmLevelUpButton farmLevelUpButton;
    [SerializeField] private TextMeshProUGUI MergeText;
    [SerializeField] private TextMeshProUGUI MergePriceText;
    [SerializeField] private FarmManager farmManager;
    [SerializeField] private string dataName;
    [SerializeField] private List<int> Prices = new();
    private int price, count,level1Animal,level2Animal = new ();

    private void Awake()
    {
        Load();
        ButtonUpdateMerge();
        MergePriceText.text = " ";
    }
    private void OnEnable()
    {
        button.onClick.AddListener(AnimalMerge);
        GameManager.Instance.ButtonUpdate.AddListener(ButtonUpdateMerge);
        Load();
        ButtonUpdateMerge();
        Invoke(nameof(ButtonUpdateMerge), 0.1f);
        if (PlayerPrefs.GetInt("FirstOpen", 0) == 1)
            Invoke(nameof(ButtonUpdateMerge), InGameManager._staticDuration);
    }
    private void OnDisable()
    {
        button.onClick.RemoveListener(AnimalMerge);
        GameManager.Instance?.ButtonUpdate.RemoveListener(ButtonUpdateMerge);
        Save();
    }
    private void AnimalMerge()
    {
        farmManager.GetMergedAnimal(1);
        GameManager.Instance.PlayerMoney -= price;
        ButtonUpdateMerge();
        Save();
        GameManager.Instance.ButtonUpdate.Invoke();
    }

    void ButtonUpdateMerge()
    {
        DataUpdate();
        TextUpdate();
        if (GameManager.Instance.PlayerMoney >= price)
        {
            if (farmLevelUpButton.Count > 4 && farmLevelUpButton.Count < 9)
            {
                if (level1Animal>1) button.interactable = true;
                else button.interactable = false;
            }
            else button.interactable = false;

            if (farmLevelUpButton.Count > 8)
            {
                if (level1Animal > 1 || level2Animal > 1) button.interactable = true;
                else button.interactable = false;
            }
        }
        else button.interactable = false;
        level1Animal = 0;
        level2Animal = 0;
    }
    void DataUpdate()
    {
        foreach (var animal in farmManager.AnimalsList)
        {
            switch (animal.Level)
            {
                case 1:
                    level1Animal++;
                    break;
                case 2:
                    level2Animal++;
                    break;
            }
        }
        if (farmLevelUpButton.Count > 4 && farmLevelUpButton.Count < 9)
        {
            count = 0;
            price = Prices[count];
        }
        else if (farmLevelUpButton.Count > 8)
        {
            if (level1Animal > 1)
            {
                count = 0;
                price = Prices[count];
            }
            else if (level1Animal < 2 && level2Animal > 1)
            {
                count = 1;
                price = Prices[count];
            }
        }
    }
    void TextUpdate()
    {
        if (farmLevelUpButton.Count > 4 && farmLevelUpButton.Count < 9)
        {
            if (level1Animal < 2 && level2Animal > 1 && farmManager.AnimalsList.Count < farmLevelUpButton.FarmCapacity[farmLevelUpButton.Count])
            {
                MergeText.text = "Need More Animal";
            }
            else if (level1Animal > 1)
            {
                MergeText.text = $"{dataName} up Lv 2";
                MergePriceText.text = price.ToString();
            }
            if (level1Animal < 2 && level2Animal < 2 && farmManager.AnimalsList.Count >= farmLevelUpButton.FarmCapacity[farmLevelUpButton.Count])
            {
                //MergePriceText.text = $"{dataName} Upgrade for Need Farm Lvl Up";
                MergeText.text = "Need Farm Lv up";
            }
        }
        else if (farmLevelUpButton.Count > 7)
        {
            if (level1Animal > 1)
            {
                MergeText.text = $"{dataName} up Lv 2 ";
                MergePriceText.text = price.ToString();
            }
            else if (level1Animal < 2 && level2Animal > 1)
            {
                MergeText.text = $"{dataName} up Lv 3 ";
                MergePriceText.text = price.ToString();
            }
            else if (level1Animal < 2 && level2Animal < 2)
            {
                if (farmManager.AnimalsList.Count >= farmLevelUpButton.FarmCapacity[farmLevelUpButton.Count])
                {
                    MergeText.text = "Need More Capacity";
                }
            }
        }
        else
        {
            //MergePriceText.text = $"{dataName} Upgrade for Need Farm Lvl 5";
            MergeText.text = "Need Farm Lv Up";
        }
    }
    void Load()
    {
        count = PlayerPrefs.GetInt($"MergeListCount{dataName}", 0);
        price = PlayerPrefs.GetInt($"MergePrice{dataName}", Prices[count]);
    }
    void Save()
    {
        PlayerPrefs.SetInt($"MergeListCount{dataName}", count);
        PlayerPrefs.SetInt($"MergePrice{dataName}", price);
    }
}
