using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class UIController : MonoBehaviour
{
    [SerializeField] private GameObject WinPanel, LosePanel, InGamePanel;
    [SerializeField] private TextMeshProUGUI moneyText;
    [SerializeField] private List<string> moneyMulti = new();
    [SerializeField] private GameObject coin, money;

    [Header("Upgrade Panels")]
    [SerializeField] private List<GameObject> FarmPanels = new();
    [SerializeField] private List<GameObject> BuyPanels = new();

    //[SerializeField] private GameObject buyButton;
    //[SerializeField] private TextMeshProUGUI buyButtonText;
    private bool canBuy = false;
    public bool CanBuy
    {
        get
        {
            return canBuy;
        }
        set
        {
            canBuy = value;
        }
    }

    //private FarmBuy farmBuy;

    private Canvas UICanvas;

    private Button Next, Restart;

    private LevelManager levelManager;

    private Settings settings;
    int firstOpen;
    int _intTemp;

    private void Awake()
    {

        ScriptInitialize();
        ButtonInitialize();
        firstOpen = PlayerPrefs.GetInt("FarmCount", 0);
    }


    private void Start()
    {
        GameManager.Instance.OnMoneyChange.Invoke();

    }

    void ScriptInitialize()
    {
        levelManager = FindObjectOfType<LevelManager>();
        settings = FindObjectOfType<Settings>();
        UICanvas = GetComponentInParent<Canvas>();
    }

    void ButtonInitialize()
    {
        Next = WinPanel.GetComponentInChildren<Button>();
        Restart = LosePanel.GetComponentInChildren<Button>();

        Next.onClick.AddListener(() => levelManager.LoadLevel(1));
        Restart.onClick.AddListener(() => levelManager.LoadLevel(0));
    }

    void ShowPanel(GameObject panel, bool canvasMode = false)
    {
        panel.SetActive(true);
        GameObject panelChild = panel.transform.GetChild(0).gameObject;
        panelChild.transform.localScale = Vector3.zero;
        panelChild.SetActive(true);
        panelChild.transform.DOScale(Vector3.one, 0.5f);

        UICanvas.worldCamera = Camera.main;
        UICanvas.renderMode = canvasMode ? RenderMode.ScreenSpaceCamera : RenderMode.ScreenSpaceOverlay;
    }

    void GameReady()
    {
        WinPanel.SetActive(false);
        LosePanel.SetActive(false);
        InGamePanel.SetActive(true);
    }

    void SetMoneyText()
    {
        if (coin.activeSelf)
        {
            coin.transform.DORewind();
            coin.transform.DOPunchScale(Vector3.one, 0.5f, 2, 1);
        }


        if (money.activeSelf)
        {
            money.transform.DORewind();
            money.transform.DOPunchScale(Vector3.one, 0.5f, 2, 1);
        }

        int moneyDigit = GameManager.Instance.PlayerMoney.ToString().Length;
        int value = (moneyDigit - 1) / 3;
        if (value < 1)
        {
            moneyText.text = GameManager.Instance.PlayerMoney.ToString();
        }
        else
        {
            float temp = GameManager.Instance.PlayerMoney / Mathf.Pow(1000, value);
            moneyText.text = temp.ToString("F2") + " " + moneyMulti[value];
        }
    }

    private void OnEnable()
    {
        
        GameManager.Instance.LevelFail.AddListener(() => ShowPanel(LosePanel, true));
        GameManager.Instance.LevelSuccess.AddListener(() => ShowPanel(WinPanel, true));
        GameManager.Instance.GameReady.AddListener(GameReady);
        GameManager.Instance.OnMoneyChange.AddListener(SetMoneyText);
        GameManager.Instance.OnOpenNewFarm.AddListener(OpenUpgradePanel);
        GameManager.Instance.OnOpenUnlockedFarm.AddListener(OwnedOpenUpgradePanel);
    }

    private void OnDisable()
    {
        if (GameManager.Instance)
        {
            GameManager.Instance.LevelFail.RemoveListener(() => ShowPanel(LosePanel, true));
            GameManager.Instance.LevelSuccess.RemoveListener(() => ShowPanel(WinPanel, true));
            GameManager.Instance.GameReady.RemoveListener(GameReady);
            GameManager.Instance.OnOpenNewFarm.RemoveListener(OpenUpgradePanel);
            GameManager.Instance.OnOpenUnlockedFarm.RemoveListener(OwnedOpenUpgradePanel);
        }
    }


    void CloseAllUpgradePanel()
    {
        foreach (var item in FarmPanels)
        {
            item.SetActive(false);
            for (int i = 0; i < item.transform.childCount; i++)
            {
                item.transform.GetChild(i).GetComponent<Button>().gameObject.SetActive(false);
            }
        }
        foreach (var item in BuyPanels)
        {
            item.SetActive(false);
            for (int i = 0; i < 1; i++)
            {
                item.transform.GetChild(0).GetComponent<Button>().gameObject.SetActive(false);
            }
        }
    }
    void SetActiveOn()
    {
        for (int i = 0; i < FarmPanels[_intTemp].transform.childCount; i++)
        {
            FarmPanels[_intTemp].transform.GetChild(i).GetComponent<Button>().gameObject.SetActive(true);
        }
        if (_intTemp != 0)
        {
            for (int i = 0; i < BuyPanels.Count; i++)
            {
                BuyPanels[i].transform.GetChild(0).GetComponent<Button>().gameObject.SetActive(true);
            }
        }

    }
    void OpenUpgradePanel(int index)
    {
        int temp = new();
        string name = "a";
        CloseAllUpgradePanel();
        switch (index)
        {
            case 0:
                _intTemp = 0; // UI button panels count
                break;
            case 1:
                _intTemp = 1; // UI button panels count
                temp = 0; // buy panels count
                name = "Duck";
                break;
            case 2:
                _intTemp = 2; // UI button panels count
                temp = 1; // buy panels count
                name = "Sheep";
                break;
            case 3:
                _intTemp = 3; // UI button panels count
                temp = 2; // buy panels count
                name = "Cow";
                break;
        }
        int farmBuy = PlayerPrefs.GetInt($"Farm{name}", 0);
        if (index !=0 && farmBuy==1) FarmPanels[index].SetActive(true);
        else if (index==0) FarmPanels[index].SetActive(true);
        if (index != 0 && farmBuy == 0) BuyPanels[temp].SetActive(true);
        else if (index != 0 && farmBuy == 1) BuyPanels[temp].SetActive(false);

        if (firstOpen == index)
        {
            SetActiveOn();
            firstOpen = 4;
        }
        else Invoke(nameof(SetActiveOn), InGameManager._staticDuration);
    }
    void OwnedOpenUpgradePanel(int index)
    {
        CloseAllUpgradePanel();

        switch (index)
        {
            case 0:
                _intTemp = 0;
                break;
            case 1:
                _intTemp = 1;
                break;
            case 2:
                _intTemp = 2;
                break;
            case 3:
                _intTemp = 3;
                break;
        }
        FarmPanels[index].SetActive(true);
        SetActiveOn();
    }

}
