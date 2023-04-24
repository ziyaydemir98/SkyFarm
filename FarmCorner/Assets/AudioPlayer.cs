using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AudioPlayer : MonoBehaviour
{
    // Start is called before the first frame update
    public AudioSource mySound;
    Button button;
    
    private void Awake()
    {
        mySound = this.gameObject.GetComponent<AudioSource>();
        button = this.gameObject.GetComponent<Button>();
    }

    // Update is called once per frame
    private void OnEnable()
    {
        button.onClick.AddListener(Play);
    }
    private void OnDisable()
    {
        button.onClick.RemoveListener(Play);
    }
    public void Play()
    {
        mySound.Play();
    }
}
