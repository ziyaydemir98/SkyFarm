using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TutorialScript : MonoBehaviour
{
    [SerializeField] Button TutorialButton;
    [SerializeField] List<Button> Buttons = new List<Button>();
    [SerializeField] List<Image> Images = new List<Image>();
    [SerializeField] List<Image> TextImages = new List<Image>();
    private void Awake()
    {
        TutorialButton.gameObject.SetActive(false);
        foreach (var image in Images)
        {
            Color color = image.color;
            color.a = 0f;
            image.color = color;
        }
        foreach (var textImage in TextImages)
        {
            textImage.gameObject.SetActive(false);
        }
        if (PlayerPrefs.GetInt("FirstOpen", 0) != 1)
        {
            Invoke(nameof(Phase1), 0.2f);
        }
        else
        {

            this.enabled = false;
        }
    }
    private void OnDisable()
    {

    }

    void Phase1()
    {
        ResetTransform();
        foreach (var button in Buttons)
        {
            button.interactable = false;
        }
        foreach (var image in Images) 
        {
            if (image.name!="BuyImage")
            {
                Color color = image.color;
                color.a = 0.5f;
                image.color = color;
            }
        }
        foreach (var textImage in TextImages)
        {
            if (textImage.name!="BuyTextBackground")
            {
                textImage.gameObject.SetActive(false);
            }
        }
        TutorialButton.gameObject.SetActive(true);
        TutorialButton.onClick.AddListener(Phase2);
    }
    void Phase2()
    {
        TutorialButton.onClick.RemoveListener(Phase2);
        ResetTransform();
        foreach (var button in Buttons)
        {
            button.interactable = false;
        }
        foreach (var image in Images)
        {
            if (image.name != "HarvestImage")
            {
                Color color = image.color;
                color.a = 0.5f;
                image.color = color;
            }
        }
        foreach (var textImage in TextImages)
        {
            if (textImage.name != "HarvestTextBackground")
            {
                textImage.gameObject.SetActive(false);
            }
        }
        TutorialButton.gameObject.SetActive(true);
        TutorialButton.onClick.AddListener(Phase3);
    }
    void Phase3()
    {
        TutorialButton.onClick.RemoveListener(Phase3);
        ResetTransform();
        foreach (var button in Buttons)
        {
            button.interactable = false;
        }
        foreach (var image in Images)
        {
            if (image.name != "MergeImage")
            {
                Color color = image.color;
                color.a = 0.5f;
                image.color = color;
            }
        }
        foreach (var textImage in TextImages)
        {
            if (textImage.name != "MergeTextBackground")
            {
                textImage.gameObject.SetActive(false);
            }
        }
        TutorialButton.gameObject.SetActive(true);
        TutorialButton.onClick.AddListener(Phase4);
    }
    void Phase4()
    {
        TutorialButton.onClick.RemoveListener(Phase4);
        ResetTransform();
        foreach (var button in Buttons)
        {
            button.interactable = false;
        }
        foreach (var image in Images)
        {
            if (image.name != "RateImage" && image.name != "ValueImage")
            {
                Color color = image.color;
                color.a = 0.5f;
                image.color = color;
            }
        }
        foreach (var textImage in TextImages)
        {
            if (textImage.name != "RateTextBackground" && textImage.name != "ValueTextBackground")
            {
                textImage.gameObject.SetActive(false);
            }
        }
        TutorialButton.gameObject.SetActive(true);
        TutorialButton.onClick.AddListener(CloseTutorial);
    }
    void ResetTransform()
    {
        foreach (var button in Buttons)
        {
            button.interactable = false;
        }
        foreach (var image in Images)
        {
            image.gameObject.SetActive(true);
            Color color = image.color;
            color.a = 0f;
            image.color = color;
        }
        foreach (var textImage in TextImages)
        {
            textImage.gameObject.SetActive(true);
        }
    }
    void CloseTutorial()
    {
        ResetTransform();
        foreach (var textImage in TextImages)
        {
            textImage.gameObject.SetActive(false);
        }
        PlayerPrefs.SetInt("FirstOpen", 1);
        GameManager.Instance.ButtonUpdate.Invoke();
        TutorialButton.gameObject.SetActive(false);
        this.enabled = false;
    }
}
