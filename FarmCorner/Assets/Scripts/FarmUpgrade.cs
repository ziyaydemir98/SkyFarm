using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.EventSystems;

public class FarmUpgrade : MonoBehaviour
{
    #region Variables

    [SerializeField] InGameManager inGameManager;
    [SerializeField] private GameObject upgradeCanvas;
    [SerializeField] private LayerMask layerMask;
    Camera cam;
    private bool _isOpen = false;
    private bool onHit = false;
    private bool canvasStart = false;

    #endregion


    private void Awake()
    {
        cam = Camera.main;
    }
    void Update()
    {
        if (inGameManager.CanDrag)
        {
            if (Input.GetMouseButtonDown(0))
            {
                RaycastHit hit;
                Ray ray = cam.ScreenPointToRay(Input.mousePosition);

                if (Physics.Raycast(ray, out hit, Mathf.Infinity, layerMask))
                {
                    if (hit.transform.gameObject != upgradeCanvas)
                    {
                        onHit = true;  
                        canvasStart = false;
                    }
                    else
                    {
                        onHit = false;
                        canvasStart = true;
                    }
                }
                else
                {
                    onHit = false;
                    canvasStart = false;
                }
            }
            if (Input.GetMouseButtonUp(0))
            {
                RaycastHit hit;
                Ray ray = cam.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out hit, Mathf.Infinity, layerMask))
                {

                    if (onHit && hit.transform.gameObject != upgradeCanvas)
                    {
                        if (_isOpen && !inGameManager.CanButtonDown)
                        {
                            _isOpen = false;
                            CloseCanvas();
                        }
                        else
                        {
                            _isOpen = true; 
                            if (!inGameManager.CanButtonDown)    
                            {
                                OpenCanvas();
                            }
                        }
                    }
                }
                else
                {
                    if (!onHit)
                    {
                        if (_isOpen && !canvasStart && !inGameManager.CanButtonDown)
                        {
                            _isOpen = false;
                            CloseCanvas();
                        }
                    }
                }
            }
        }
        else
        {
            CloseCanvas();
        }
    }
    public void OpenCanvas()
    {
        upgradeCanvas.transform.DOScale(new Vector3(1.8f, 1.3f, 1f), 0.5f);
    }
    public void CloseCanvas()
    {
        upgradeCanvas.transform.DOScale(Vector3.zero, 0.5f);
    }
    private void OnDisable()
    {
        upgradeCanvas.transform.DOScale(Vector3.zero, 0.5f);
    }
    private void OnEnable()
    {
        upgradeCanvas.transform.DOScale(Vector3.zero, 0.5f);
    }
}
