using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrcScript : Enemy
{
    public enum EnemyState
    {
        WAIT,            //行動を一旦停止
        MOVE,            //移動
        ATTACK,        //弱攻撃
        POWERATTACK,    //強攻撃
        IDLE,            //待機
    }
    [SerializeField]
    private Transform target;
    [SerializeField]
    private float rotateSpeed = 180f;
    [SerializeField]
    private Vector3 distanceFromTarget;

    private float angle;
    public EnemyState enemystate = EnemyState.WAIT;
    EnemyState nextstate = EnemyState.IDLE;
    float m_Timer;
    Vector3 targetPos;
    bool isAiStateRunning = false , isAttackCoolTime = false;
    

    Dictionary<EnemyState, float> attackInfo;

    protected override void Start()
    {
        base.Start();
        statusSet(m_cSVLoader.GetComponent<CsvReader>().GetMonsterStatusData("Orc", enemyLv));
        targetPos = transform.position;
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
            case EnemyState.POWERATTACK:
                powerAttack();
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
        aiMain();
        enemystate = nextstate;
        InitAi();
        StartCoroutine("AiTimer");

    }
    //設定の初期化
    void InitAi()
    {
        m_agent.destination = transform.position;
        AnimaitionReset();
        isAttackCoolTime = false;
    }
    //アニメーションの初期化
    void AnimaitionReset()
    {
        m_animator.SetBool("Attack", false);
        m_animator.SetBool("MoveAttack", false);
        m_animator.SetBool("Idle", false);
        m_animator.SetBool("Walk", false);
        m_animator.SetBool("Damage", false);
    }

    void aiMain()
    {
        if (isAttackCoolTime)
        {
            nextstate = EnemyState.IDLE;
            return;
        }

        if (IsAttackDistance())
        {
            randomAttackSet();
            attackSelect();
            return;
        }

        if (CanSeePlayer())
        {
            nextstate = EnemyState.MOVE;
            return;
        }
    }

    //
    private IEnumerator AiTimer()
    {
        isAiStateRunning = true;
        yield return new WaitForSeconds(3f);
        isAiStateRunning = false;
    }
    //プレイヤーの元に移動する
    //攻撃距離に入ったら攻撃する
    void wait()
    {
        transform.position = target.position + Quaternion.Euler(0, angle, 0) * distanceFromTarget;
        transform.LookAt(m_Player.transform.position);
        angle += rotateSpeed * Time.deltaTime;
        angle = Mathf.Repeat(angle, 360f);

    }
    void move()
    {

        if (!IsAttackDistance())
        {
            m_agent.speed = MoveSpeed;
            m_agent.destination = m_Player.transform.position;
        }
        else
        {
            m_agent.speed = 0;
        }
        m_animator.SetBool("Walk", true);
    }
    //通常攻撃
    void attack()
    {
        m_agent.speed = 0;
        m_animator.SetBool("Attack", true);
        isAttackCoolTime = true;
        isAiStateRunning = false;
    }

    //強攻撃
    void powerAttack()
    {
        m_agent.speed = 0;
        m_animator.SetBool("MoveAttack", true);
        isAttackCoolTime = true;
        //nextstate = EnemyState.IDLE;
    }
    

    void idle()
    {
        m_Timer++;
        //transform.LookAt(m_PlayerLookPoint.position);
        if (m_Timer >= 300)
        {
            m_Timer = 0;
            //nextstate = EnemyState.MOVE;
        }
    }

    //各攻撃パターンの確率を決める
    void randomAttackSet()
    {
        attackInfo = new Dictionary<EnemyState, float>();
        attackInfo.Add(EnemyState.ATTACK, 70f);
        attackInfo.Add(EnemyState.POWERATTACK, 30f);
    }

    //ランダムに攻撃パターンを決める
    void attackSelect()
    {
        float total = 0;

        foreach (KeyValuePair<EnemyState, float> elem in attackInfo)
        {
            total += elem.Value;
        }

        float randomPoint = Random.value * total;

        foreach (KeyValuePair<EnemyState, float> elem in attackInfo)
        {
            if (randomPoint < elem.Value)
            {
                nextstate = elem.Key;
            }
            else
            {
                randomPoint -= elem.Value;
            }
        }
    }

    private void OnTriggerEnter(Collider collision)
    {

        if (collision.gameObject.CompareTag("Player") )
        {
            collision.gameObject.GetComponent<PlayerMotion>().Damage(ATK);
        }
    }

}
