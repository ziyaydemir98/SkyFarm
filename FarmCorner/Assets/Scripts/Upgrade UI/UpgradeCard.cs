using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UpgradeCard : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI levelText,priceText;
    [SerializeField] private Button upgradeButton;
    
    [Header("Card Properties")]
    [SerializeField] private string upgradeName;
    [SerializeField] private List<float> values = new();
    [SerializeField] private List<float> prices = new();
    private int _cardLevel = 1;

    private void Awake()
    {
        upgradeName = gameObject.name;
    }

    private void Start()
    {
        ItemInitialize();
    }

    private void OnEnable()
    {
        LoadData();
        ItemInitialize();
        GameManager.Instance.OnMoneyChange.AddListener(ButtonInitialize);
        upgradeButton.onClick.AddListener(Buy);
    }

    private void OnDisable()
    {
        if (GameManager.Instance)
            GameManager.Instance.OnMoneyChange.RemoveListener(ButtonInitialize);

        upgradeButton.onClick.RemoveListener(Buy);
    }

    void ItemInitialize()
    {
        var temp = _cardLevel + 1;
        levelText.text = "Lv. " + temp.ToString();
        ButtonInitialize();
    }

    void ButtonInitialize()
    {
        if (_cardLevel >= prices.Count)
        {
            priceText.text = "MAX";
            upgradeButton.interactable = false;
            priceText.color = Color.red;
        }
        else
        {
            priceText.text = prices[_cardLevel].ToString();

            upgradeButton.interactable = GameManager.Instance.PlayerMoney >= prices[_cardLevel];
            priceText.color = upgradeButton.interactable ? Color.white : Color.red;

        }
    }

    void Buy()
    {
        GameManager.Instance.PlayerMoney -= prices[_cardLevel];
        _cardLevel++;
        ItemInitialize();
        SaveData();
    }

    void LoadData()
    {
        _cardLevel = PlayerPrefs.GetInt(upgradeName + "Level", 0);
    }

    void SaveData()
    {
        PlayerPrefs.SetFloat(upgradeName, values[_cardLevel]);
        PlayerPrefs.SetInt(upgradeName + "Level", _cardLevel);
    }
}
