using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ModeMenuScript : MonoBehaviour
{
    [SerializeField] private Toggle soloToggle;
    [SerializeField] private Toggle interviewToggle;
    
    [SerializeField] private Image image;
    [SerializeField] private Sprite soloPicture;
    [SerializeField] private Sprite interviewPicture;
    
    [SerializeField] private Canvas nextMenu;
    [SerializeField] private GameObject button;
    
    private Canvas _canvas;
    private void Start()
    {
        _canvas = GetComponent<Canvas>();
    }

    /// <summary>
    /// Changes the picture based on the chosen mode
    /// </summary>
    public void ChangeMode()
    {
        button.SetActive(true);
        if (soloToggle.isOn)
        {
            image.sprite = soloPicture;
        }
        else if(interviewToggle.isOn)
        {
            image.sprite = interviewPicture;
        }
    }

    /// <summary>
    /// Activates next menu
    /// </summary>
    public void NextMenu()
    {
        if (soloToggle.isOn)
        {
            MainMenuManager.Instance.SetMode(MainMenuManager.SpeechMode.Solo);
        }
        else
        {
            MainMenuManager.Instance.SetMode(MainMenuManager.SpeechMode.Interview);
        }
        _canvas.enabled = false;
        nextMenu.enabled = true;
    }
}
