using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using DG.Tweening;
using UnityEngine.EventSystems;

public class UIManager : MonoBehaviour
{
    [SerializeField] GameObject TitleUI;
    [SerializeField] GameObject GamePlayUI;
    [SerializeField] GameObject PauseUI;
    [SerializeField] GameObject SettingUI;
    [SerializeField] GameObject QuitGameCheckUI;

    [SerializeField] GameObject SettingTitleBack;
    [SerializeField] GameObject SettingPauseBack;

    public Button StartSelectButton;
    
    public Slider SettingSelectButton;
    public Button PauseSelectButton; 
    public Button QuitButton;
    private bool IsGamePause = false;
    private bool IsActive = false;
    private bool IsSetting = false;
    void Start()
    {
        StartSelectButton.Select();
        
    }

    void Update()
    {
        if (Input.GetKey(KeyCode.Escape))
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }

        if (SceneManager.GetActiveScene().name == "PlayScene"
            && Input.GetButtonDown("Option") && !IsGamePause)
        {
            IsGamePause = true;
            Pause();
        }
        
        

    }

    private void Awake()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        UIReset();
        TitleUI.SetActive(true);
    }
    public void UIReset()
    {
        TitleUI.SetActive(false);
        GamePlayUI.SetActive(false);
        PauseUI.SetActive(false);
        SettingUI.SetActive(false);
        QuitGameCheckUI.SetActive(false);
        SettingTitleBack.SetActive(false);
        SettingPauseBack.SetActive(false);
    }


    public void Pause()
    {
        IsSetting = false;
        PauseSelectButton.Select();
        Time.timeScale = 0;
        UIReset();
        PauseUI.SetActive(true);
    }

    public void Setting()
    {
        IsSetting = true;
        SettingSelectButton.Select();
        UIReset();
        SettingUI.SetActive(true);
    }

    public void QuitGame()
    {
        QuitButton.Select();
        UIReset();
        QuitGameCheckUI.SetActive(true);
    }
    public void GamePlay()
    {
        Time.timeScale = 1;
        UIReset();
        IsGamePause = false;
        GamePlayUI.SetActive(true);
    }
    public void TitleBack()
    {
        SettingTitleBack.SetActive(true);
        SettingPauseBack.SetActive(false);
    }
    public void PauseBack()
    {
        SettingTitleBack.SetActive(false);
        SettingPauseBack.SetActive(true);
    }
    public void Title()
    {
        IsSetting = false;
        UIReset();
        TitleUI.SetActive(true);
        StartSelectButton.Select();
    }
     
}
