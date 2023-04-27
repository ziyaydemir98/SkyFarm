using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SheepSkinManager : MonoBehaviour
{
    #region Variables
    [SerializeField] private AnimalManager sheepManager;
    private FarmManager farmManager;
    public FarmManager FarmManager
    {
        get
        {
            return farmManager;
        }
        set
        {
            farmManager = value;
        }
    }
    private GameObject _currentChild;
    Vector3 maxScale = Vector3.one * 140f;
    float currentGrowTime, horizontalF, verticalF;
    bool growComplete = false;
    public bool GrowComplete
    {
        get
        {
            return growComplete;
        }
        set
        {
            growComplete = value;
        }
    }
    bool skinBreak = true;

    #endregion

    #region Functions
    private void Awake()
    {
        //SetVisualObject();
    }
    private void OnEnable()
    {
        currentGrowTime = farmManager.HarvestTimer;
        DefaultValues();

        SetVisualObject();
        sheepManager.OnLevelChanged.AddListener(() =>
        {
            if (GrowComplete)
            {
                SetVisualObject();
                _currentChild.transform.localScale = Vector3.one * 100;
                _currentChild.transform.localPosition = new Vector3(_currentChild.transform.localPosition.x, 0.2f, _currentChild.transform.localPosition.z);
                GrowComplete = false;
            }
            
        });

        GameManager.Instance.HarvestWools.AddListener(HarvestWool);
    }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Correctness", "UNT0008:Null propagation on Unity objects", Justification = "< bekleyen >")]
    private void OnDisable()
    {
        SetVisualObject();
        GameManager.Instance?.HarvestWools.RemoveListener(HarvestWool);
        skinBreak = true;
    }

    void FixedUpdate()
    {
        if (_currentChild!=null)
        {
            GrowSheepSkin();
        }
        
    }
    void DefaultValues()
    {
        horizontalF = 0.8f;
        farmManager.HarvestTimer = currentGrowTime;
        horizontalF = horizontalF / farmManager.HarvestTimer;
        verticalF = horizontalF / 800;
    }
    private void SetVisualObject()
    {
        _currentChild = sheepManager.GetCurrentAnimalObject();
    }
    private void HarvestWool()
    {
        if (GrowComplete)
        {
            _currentChild.transform.localScale = Vector3.one * 100;
            _currentChild.transform.localPosition = Vector3.zero;
            _currentChild.transform.localPosition = new Vector3(_currentChild.transform.localPosition.x, 0.2f, _currentChild.transform.localPosition.z);
            DefaultValues();
            GrowComplete = false;
        }
    }
    public void GrowSheepSkin()
    {
        if (GrowComplete) return;
        {
            Debug.Log(FarmManager.Sec);
            if (FarmManager.Sec > 0 && skinBreak)
            {
                if (FarmManager.Sec >= farmManager.HarvestTimer) // HASAT SURESINDEN UZUN BEKLENDIYSE
                {
                    horizontalF = 0.008f;
                    farmManager.HarvestTimer = 1;
                    FarmManager.Sec = 0;
                    horizontalF = horizontalF / farmManager.HarvestTimer;
                    verticalF = horizontalF / 2;
                    Debug.Log("UZUN");
                }
                else if (FarmManager.Sec < farmManager.HarvestTimer && FarmManager.Sec > 0.01f) // HASAR SURESINDEN KISA ZAMAN GECTIYSE
                {
                    horizontalF = 0.008f;
                    farmManager.HarvestTimer = farmManager.HarvestTimer - FarmManager.Sec;
                    FarmManager.Sec = 0;
                    horizontalF = horizontalF / farmManager.HarvestTimer;
                    verticalF = horizontalF / 2;
                    Debug.Log("KISA");
                }
                skinBreak = false;
            }
            if (_currentChild.transform.localScale.magnitude < maxScale.magnitude)
            {
                _currentChild.transform.localScale = new Vector3(_currentChild.transform.localScale.x + horizontalF, _currentChild.transform.localScale.y + horizontalF, _currentChild.transform.localScale.z + horizontalF);
                _currentChild.transform.localPosition = new Vector3(_currentChild.transform.localPosition.x, _currentChild.transform.localPosition.y, _currentChild.transform.localPosition.z - verticalF);
                if (_currentChild.transform.localScale.magnitude >= maxScale.magnitude)
                {
                    GrowComplete = true;
                    DefaultValues();
                    GameManager.Instance.HarvestButtonUpdate.Invoke();
                }
            }
        }
    }
    #endregion
}
