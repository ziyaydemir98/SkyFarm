using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class FarmBuy : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI priceText;
    [SerializeField] Button FarmBuyButton;
    [SerializeField] FarmManager farmManager;
    [SerializeField] int Price;
    [SerializeField] string farmName;
    private int isPurchased;
    public int IsPurchased
    {
        get
        {
            return isPurchased;
        }
        private set
        {
        }
    }
    private void Awake()
    {
        Load();
    }
    private void OnEnable()
    {
        IsFarmOwn();
        priceText.text = Price.ToString();
    }
    private void OnDisable()
    {
        Save(); 
        FarmBuyButton.onClick.RemoveListener(FarmOwn);
        FarmBuyButton.interactable = false;
        CancelInvoke(nameof(OnInter));
    }
    void FarmOwn()
    {
        isPurchased = 1;
        GameManager.Instance.PlayerMoney -= Price;
        GameManager.Instance.OnOpenUnlockedFarm.Invoke(farmManager.FarmTypeInt);
        this.gameObject.SetActive(false);
    }

    void IsFarmOwn()
    {
        if (isPurchased == 0)
        {
            this.gameObject.SetActive(true);
            FarmBuyButton.onClick.AddListener(FarmOwn);
            if (GameManager.Instance.PlayerMoney >= Price)
                Invoke(nameof(OnInter), InGameManager._staticDuration);
        }

    }

    void OnInter()
    {
        FarmBuyButton.interactable = true;
    }
    
    private void Save()
    {
        PlayerPrefs.SetInt($"Farm{farmName}", isPurchased);
    }
    private void Load()
    {
        PlayerPrefs.GetInt($"Farm{farmName}", 0);
    }
}
