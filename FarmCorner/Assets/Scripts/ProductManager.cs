using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProductManager : MonoBehaviour
{
    // Start is called before the first frame update
    private void OnEnable()
    {
        switch (this.gameObject.tag)
        {
            case "level1":
                this.gameObject.transform.GetChild(0).gameObject.SetActive(true);
                break;
            case "level2":
                this.gameObject.transform.GetChild(1).gameObject.SetActive(true);
                break;
            case "level3":
                this.gameObject.transform.GetChild(2).gameObject.SetActive(true);
                break;
        }
    }
    private void OnDisable()
    {
        for (int i = 0; i < this.gameObject.transform.childCount; i++)
        {
            this.gameObject.transform.GetChild(i).gameObject.SetActive(false);
        }
    }
}
