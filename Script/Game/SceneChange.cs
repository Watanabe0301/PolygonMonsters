using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChange : MonoBehaviour
{
    //ÉTÉEÉìÉhä«óù
    GameObject m_sound;
    SoundManager soundManager;

    public string ChageSceneName, SetScene;
    public bool isBossDie = true;
    // Start is called before the first frame update
    void Start()
    {
        m_sound = GameObject.FindGameObjectWithTag("SoundManager");
        soundManager = m_sound.GetComponent<SoundManager>();
        soundManager.PlayBgmByName(SceneManager.GetActiveScene().name);
    }

    // Update is called once per frame
    void Update()
    {
        if (!isBossDie && SceneManager.GetActiveScene().name != "GameclearScene")
            StartCoroutine(DeliriaSceneChage("GameclearScene"));
    }
    public void NextScene(string nextSceneName)
    {
        SceneManager.LoadScene(nextSceneName);
        PlayBGM(nextSceneName);
    }
    public void OnPlayer()
    {
        NextScene(ChageSceneName);
    }

    public void PlayBGM(string playBgm)
    {
        soundManager.PlayBgmByName(playBgm);
    }

    public IEnumerator DeliriaSceneChage(string nextSceneName)
    {
        isBossDie = true;
        yield return new WaitForSeconds(3f);
        NextScene(nextSceneName);
    }
}
