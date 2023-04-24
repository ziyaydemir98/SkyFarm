using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HarvestButton : MonoBehaviour
{
    [SerializeField] private Button button;
    [SerializeField] private FarmManager farmManager;
    [SerializeField] private string dataName;
    [SerializeField] private List<float> Prices = new();
    public List<float> PricesProp
    {
        get
        {
            return Prices;
        }
        set
        {
            Prices = value;
        }
    }
    float money;
    private void Awake()
    {
        if (farmManager.FarmTypeInt != 2)
        {
            if (farmManager.HarvestList.Count > 0)
            {
                button.interactable = true;
            }
            else
            {
                button.interactable = false;
            }
        }
        else button.interactable = false;
    }
    private void OnEnable()
    {
        //ButtonUpdateHarvest();
        //Invoke(nameof(ButtonUpdateHarvest), InGameManager._staticDuration);
        button.onClick.AddListener(HarvestProduct);
        //GameManager.Instance.ButtonUpdate.AddListener(ButtonUpdateHarvest);
        GameManager.Instance.HarvestButtonUpdate.AddListener(ButtonUpdateHarvest);
    }
    private void OnDisable()
    {
        button.onClick.RemoveListener(HarvestProduct);
        //GameManager.Instance?.ButtonUpdate.RemoveListener(ButtonUpdateHarvest);
        GameManager.Instance?.HarvestButtonUpdate.RemoveListener(ButtonUpdateHarvest);
    }
    private void HarvestProduct()
    {
        DataUpdate();

        if (farmManager.FarmTypeInt!=2)
        {
            farmManager.HarvestObject();
        }
        else
        {
            farmManager.CutSkin();
        }
        
        button.interactable = false;
        GameManager.Instance.ButtonUpdate.Invoke();
    }
    void ButtonUpdateHarvest()
    {
        button.interactable = true;
    }

    void DataUpdate()
    {
        
        if (farmManager.FarmTypeInt != 2)   
        {
            foreach (var product in farmManager.HarvestList)
            {
                switch (product.tag)
                {
                    case "level1":
                        GameManager.Instance.PlayerMoney += Prices[0];
                        money += Prices[0];
                        break;
                    case "level2":
                        GameManager.Instance.PlayerMoney += Prices[1];
                        money += Prices[1];
                        break;
                    case "level3":
                        GameManager.Instance.PlayerMoney += Prices[2];
                        money += Prices[2];
                        break;
                }
            }
            money = 0;
        }
        else
        {
            foreach (var product in farmManager.AnimalsList)
            {
                if (product.GetComponent<SheepSkinManager>().GrowComplete)
                {
                    switch (product.Level)
                    {
                        case 1:
                            GameManager.Instance.PlayerMoney += Prices[0];
                            break;
                        case 2:
                            GameManager.Instance.PlayerMoney += Prices[1];
                            break;
                        case 3:
                            GameManager.Instance.PlayerMoney += Prices[2];
                            break;
                    }
                }
            }
        }
    }
}
