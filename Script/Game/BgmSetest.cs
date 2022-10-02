using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BgmSetest : MonoBehaviour
{
    SoundManager soundManager;

    private void Start()
    {
        soundManager = GetComponent<SoundManager>();
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.J))
        {
            print("‰Ÿ‚³‚ê‚½");
            soundManager.PlayBgmByName("Šô–]‚ÌŒŽ");
        }
    }
}
