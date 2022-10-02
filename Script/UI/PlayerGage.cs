using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class PlayerGage : MonoBehaviour
{
    [SerializeField]
    private Image GreenGauge;
    [SerializeField]
    private Image RedGauge;

    [SerializeField]
    private Image StaminaGauge;

    private PlayerMotion player;
    private Tween redGaugeDOTween, greenGaugeDOTween;
    float From;

    private void Start()
    {
        
    }

    private void FixedUpdate()
    {
       
    }

    //ダメージ処置
    public void GaugeReduce(float DamageValue)
    {
        From = player.m_PlayerHP / player.m_PlayerMaxHP;
        var To = (player.m_PlayerHP - DamageValue) / player.m_PlayerMaxHP;

        // 緑ゲージ減少
        GreenGauge.fillAmount = To;

        
        redGaugeDOTween = DOTween.To(() => From , 
                                    x => { RedGauge.fillAmount = x ; }  
                                    , To 
                                    , 1f);

        
    }

    //回復処置
    public void GaugeRecovery(float RecoveryValue)
    {
        From = player.m_PlayerHP / player.m_PlayerMaxHP;
        var To = (player.m_PlayerHP + RecoveryValue) / player.m_PlayerMaxHP;

        greenGaugeDOTween = DOTween.To(() => From,
                                    x => { GreenGauge.fillAmount = x; }
                                    , To
                                    , 3f);
    }

    //スタミナゲージ
    public void StaminaGaugeReduce(float StaminaValue)
    {

        var To = StaminaValue  / player.m_PlayerMaxStamina;

        // 緑ゲージ減少
        StaminaGauge.fillAmount = To;
    }

    public void SetPlayer(PlayerMotion player)
    {
        this.player = player;
    }

    
        //if (redGaugeDOTween != null)
        //{
        //    redGaugeDOTween.Kill();
        //}
}
