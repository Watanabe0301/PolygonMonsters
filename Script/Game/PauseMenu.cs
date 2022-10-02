using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] GameObject pauseUI;
    [SerializeField] GameObject GamePlayUI;
    [SerializeField] GameObject VolumeSetUI;
    [SerializeField] GameObject QuitGameCheckUI;
    
    [SerializeField] GameObject TitleSettingBack;
    [SerializeField] GameObject SettingBack;
    private bool IsGamePause = false;
    bool Isswitch = false;
    void Start()
    {
        pauseUI.SetActive(false);
        VolumeSetUI.SetActive(false);
        QuitGameCheckUI.SetActive(false);
        GamePlayUI.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Option") && !IsGamePause)
        {
            Pause();
        }
    }

    public void Pause()
    {
        if (!IsGamePause)
        {
            Time.timeScale = 0;
            GamePlayUI.SetActive(false);
            pauseUI.SetActive(true);

            IsGamePause = true;
        }
        else
        {
            Time.timeScale = 1;
            GamePlayUI.SetActive(true);
            pauseUI.SetActive(false);
            IsGamePause = false;
        }
        
    }
    public void Volmue()
    {
        pauseUI.SetActive(false);
        VolumeSetUI.SetActive(true);
        QuitGameCheckUI.SetActive(false);
        GamePlayUI.SetActive(false);
        SettingBack.SetActive(true);
    }
    public void QuitGame()
    {
        pauseUI.SetActive(false);
        VolumeSetUI.SetActive(false);
        QuitGameCheckUI.SetActive(true);
        GamePlayUI.SetActive(false);
    }
    public void Back()
    {
        pauseUI.SetActive(true);
        VolumeSetUI.SetActive(false);
        QuitGameCheckUI.SetActive(false);
        GamePlayUI.SetActive(false);
    }

    public void TitleSetting()
    {
        Isswitch = !Isswitch;
        VolumeSetUI.SetActive(Isswitch);
        TitleSettingBack.SetActive(Isswitch);
        SettingBack.SetActive(false);
    }
}
