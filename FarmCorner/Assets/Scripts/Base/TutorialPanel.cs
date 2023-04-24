using UnityEngine;

public class TutorialPanel : MonoBehaviour
{
    // void Update()
    // {
    //     if(Input.GetMouseButtonDown(0))
    //     {
    //         GameManager.Instance.GameStart.Invoke();
    //         gameObject.SetActive(false);
    //     }
    // }

    public void StartGameBtn()
    {
        GameManager.Instance.GameStart.Invoke();
        gameObject.SetActive(false);
    }
}
