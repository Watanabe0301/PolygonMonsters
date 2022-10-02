using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameSetting : SingletonMonoBehaviour<GameSetting>
{
    public enum Type { cameraYspeed }
    [SerializeField]
    Type type = 0;
    Slider slider ;
    bool Axis = true;

    CamerController camerController;
    GameManager gameManager;

    private void Start()
    {
        DontDestroyOnLoad(gameObject);
        slider = GetComponent<Slider>();
        gameManager = FindObjectOfType<GameManager>();
    }

    public void OnDpiValueChanged()
    {
        switch (type)
        {
            case Type.cameraYspeed:
                gameManager.cameraYspeedSet = slider.value * 100;
                //camerController.m_RotateYspeed =   - slider.value * 100;
                break;
           
        }
    }
    
    public void OnAxis()
    {
        Axis = !Axis;
        gameManager.cameraAxisSet = Axis;
        //camerController.CameraAxis(Axis);
    }

}
