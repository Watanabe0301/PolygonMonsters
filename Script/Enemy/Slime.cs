using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Slime : Enemy
{
    public enum EnemyState
    {
        WAIT,            //行動を一旦停止
        MOVE,            //移動
        ATTACK,        //弱攻撃
        IDLE,            //待機
    }
    public EnemyState enemystate = EnemyState.IDLE;
    EnemyState nextstate = EnemyState.IDLE;
    float m_Timer;
    bool isAiStateRunning = false;
    bool stopAiTimer = false , isAttackCoolTime = false;
    Vector3 dis;

    protected override void Start()
    {
        base.Start();
        statusSet(m_cSVLoader.GetComponent<CsvReader>().GetMonsterStatusData("Slime" , enemyLv));
        StartCoroutine(Movepoint());
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

    //次の行動を決める
    void aiMain()
    {
        switch (enemystate)
        {
            case EnemyState.IDLE:

                if (CanSeePlayer())
                {
                    nextstate = EnemyState.MOVE;
                }
                else
                {
                    nextstate = EnemyState.IDLE;
                }
                break;

            case EnemyState.MOVE:

                if (!CanSeePlayer())
                    nextstate = EnemyState.IDLE;
                else
                    nextstate = EnemyState.MOVE;

                if (IsAttackDistance() && !isAttackCoolTime)
                    nextstate = EnemyState.ATTACK;

                break;

            case EnemyState.ATTACK:
                if (!IsAttackDistance() || isAttackCoolTime)
                {
                    nextstate = EnemyState.MOVE;
                }
                break;
        }

    }
    
    //次の行動を決める停止
    private IEnumerator AiTimer()
    {
        isAiStateRunning = true;

        yield return new WaitForSeconds(3f);
        isAiStateRunning = false;
    }
    
    //攻撃のクールタイム
    private IEnumerator CoolTimer()
    {
        isAttackCoolTime = true;

        yield return new WaitForSeconds(10f);

        isAttackCoolTime = false;
    }
    void wait()
    {
        

    }
    void move()
    {
        m_animator.SetBool("Walk", true);
        if (!IsAttackDistance())
        {
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
        m_agent.speed = 0;
        m_animator.SetBool("Attack", true);
        IsAttack = true;
        isAttackCoolTime = true;
        isAiStateRunning = false;
        soundManager.PlaySeByName("スライムの攻撃");
    }
    

    void idle()
    {
        m_agent.destination = m_Movepoint;

        if (m_agent.destination == m_Movepoint)
        {
            isAiStateRunning = false;
        }
    }

    private void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.CompareTag("Player") && IsAttack)
        {
            collision.gameObject.GetComponent<PlayerMotion>().Damage(ATK);
        }
    }
    public override void Death()
    {
        base.Death();
    }
    public override void Damege(int EnemyDamage, Vector3 hitpos)
    {
        base.Damege(EnemyDamage, hitpos);
        enemystate = EnemyState.MOVE;
    }
}