using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BatScript : Enemy
{
    public enum EnemyState
    {
        WAIT,            //行動を一旦停止
        MOVE,            //移動
        ATTACK,        //弱攻撃
        IDLE,            //待機
    }
    public EnemyState enemystate = EnemyState.WAIT;
    EnemyState nextstate = EnemyState.IDLE;
    float m_Timer;
    bool isAiStateRunning = false;
    bool stopAiTimer = false;
    Vector3 dis;
    public GameObject shotObject;
    protected override void Start()
    {
        base.Start();
        statusSet(m_cSVLoader.GetComponent<CsvReader>().GetMonsterStatusData("Bat", enemyLv));
    }
    void Update()
    {
        setAi();
        switch (enemystate)
        {
            case EnemyState.WAIT:
                wait();
                break;
            case EnemyState.MOVE:
                move();
                break;
            case EnemyState.ATTACK:
                attack();
                break;
            case EnemyState.IDLE:
                idle();
                break;
        }
    }
    void setAi()
    {
        if (isAiStateRunning)
        {
            return;
        }
        InitAi();
        aiMain();
        enemystate = nextstate;
        StartCoroutine("AiTimer");
    }
    //設定の初期化
    void InitAi()
    {

        m_agent.destination = transform.position;
        StopCoroutine("AiTimer");
        AnimaitionReset();
    }
    //アニメーションの初期化
    void AnimaitionReset()
    {
        m_animator.SetBool("Move", false);
        m_animator.SetBool("Attack", false);
        m_animator.SetBool("Idle", false);
        m_animator.SetBool("Damage", false);
    }
    void aiMain()
    {
        switch (enemystate)
        {
            case EnemyState.IDLE:

                if (IsPlayerViewDistance())
                {
                    nextstate = EnemyState.MOVE;
                }

                break;
            case EnemyState.MOVE:

                if (IsAttackDistance())
                {
                    nextstate = EnemyState.ATTACK;
                }
                break;
        }

    }
    private IEnumerator AiTimer()
    {
        isAiStateRunning = true;

        yield return new WaitForSeconds(3f);

        isAiStateRunning = false;
    }
    void wait()
    {

    }
    void move()
    {
        if (!IsAttackDistance())
        {
            m_animator.SetBool("Move", true);
            m_agent.speed = MoveSpeed;
            m_agent.destination = m_Player.transform.position;

        }
        else
        {
            m_agent.speed = 0;
            isAiStateRunning = false;
        }

    }
    void attack()
    {
        m_animator.SetBool("Attack", true);
        isAiStateRunning = false;
        nextstate = EnemyState.IDLE;
    }
    void AttackEvent()
    {
        Instantiate(shotObject, m_EyePoint.transform.position, m_EyePoint.transform.rotation);
    }
    void idle()
    {
        m_Timer++;
        
        m_animator.SetBool("Idle", true);

        if (m_Timer >= 300)
        {
            transform.LookAt(m_PlayerLookPoint.position);
            //nextstate = EnemyState.MOVE;
            m_Timer = 0;
        }
    }
}
