using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class EnemyHpBar : MonoBehaviour
{

    [SerializeField]private Image EnemyGauge;
    [SerializeField] private TextMeshProUGUI NameText;
    [SerializeField] private TextMeshProUGUI LvText;
    RectTransform foreground = null;
    Enemy status = null;

    void Start()
    {
        status = transform.parent.GetComponent<Enemy>();

        foreach (RectTransform child in GetComponentsInChildren<RectTransform>())
        {
            if (child.name == "ForegroundImage")
            {
                foreground = child;
            }
        }
    }
    void Update()
    {
        transform.rotation = Camera.main.transform.rotation;
        NameText.text = "" + status.enemyName;
        LvText.text = ""+status.enemyLv;
        float enemyGauge =  status.HP  / status.MaxHP;
        EnemyGauge.fillAmount = enemyGauge;
        transform.forward = Camera.main.transform.forward;
        if (status.HP <= 0)
            Destroy(gameObject);
    }
    

}
