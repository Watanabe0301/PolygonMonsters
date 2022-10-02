using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Demon : Enemy
{
    public enum EnemyState
    {
        WAIT,//çsìÆÇàÍíUí‚é~
        MOVE,//à⁄ìÆ
        RandomMove,//ÉâÉìÉ_ÉÄÇ»çsìÆ
        NormalAttack,//ÉpÉìÉ`
        FlameBreath,//âäçUåÇ
        Summons,//è¢ä´
        IDLE,//ë“ã@
    }
    
    [SerializeField]
    private Transform target;
    [SerializeField]
    private float rotateSpeed = 180f;
    [SerializeField]
    private Vector3 distanceFromTarget , sumonsTarget;

    GameObject SceneManager;
    SceneChange sceneChange;
    public GameObject[] sumonsEnemy;
    private float angle;
    public EnemyState enemystate = EnemyState.WAIT;
    EnemyState nextstate = EnemyState.IDLE;
    float m_Timer;
    Vector3 targetPos;
    bool isAiStateRunning = false, isAttackCoolTime = false , isSummons = false;
    public int sumonsEnemyCount = 0;
    public float Range;
    Dictionary<EnemyState, float> actionInfo;
    public GameObject shotObject;

    protected override void Start()
    {
        base.Start();
        SceneManager = GameObject.Find("SceneManager");
        sceneChange = SceneManager.GetComponent<SceneChange>();
        targetPos = transform.position;
        Summons();
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
            case EnemyState.RandomMove:
                randomMove();
                break;
            case EnemyState.NormalAttack:
                NormalAttack();
                break;
            case EnemyState.FlameBreath:
                FlameBreath();
                break;
            case EnemyState.Summons:
                Summons();
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
    //ê›íËÇÃèâä˙âª
    void InitAi()
    {
        m_agent.destination = transform.position;
        AnimaitionReset();
        isAttackCoolTime = false;
    }
    //ÉAÉjÉÅÅ[ÉVÉáÉìÇÃèâä˙âª
    void AnimaitionReset()
    {
        m_animator.SetBool("Attack1", false);
        m_animator.SetBool("Attack2", false);
        m_animator.SetBool("Attack3", false);
        m_animator.SetBool("Idle", false);
        m_animator.SetBool("Walk", false);
        m_animator.SetBool("Damage", false);
    }

    void aiMain()
    {
        if (!CanSeePlayer())
        {
            nextstate = EnemyState.IDLE;
            return;
        }


        if (IsAttackDistance())
        {
            setAcution();
            acutionSelect();
        }
        else
        {
            nextstate = EnemyState.MOVE;
        }
    }

    //
    private IEnumerator AiTimer()
    {
        isAiStateRunning = true;
        yield return new WaitForSeconds(3f);
        //yield return null;
        isAiStateRunning = false;
    }
    //ÉvÉåÉCÉÑÅ[ÇÃå≥Ç…à⁄ìÆÇ∑ÇÈ
    //çUåÇãóó£Ç…ì¸Ç¡ÇΩÇÁçUåÇÇ∑ÇÈ
    //çUåÇÇ≈Ç´Ç»Ç¢Ç»ÇÁâ°Ç…à⁄ìÆÇ∑ÇÈ
    void wait()
    {
        transform.position = target.position + Quaternion.Euler(100, 100, 100) * distanceFromTarget;
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
    
    void randomMove()
    {
        m_agent.speed = MoveSpeed;
        transform.LookAt(m_Player.transform.position);
        angle = Random.Range(0, 360);
        Vector3 position = m_Player.transform.position + Quaternion.Euler(0, angle, 0) * distanceFromTarget;
        m_agent.destination = position;
        Debug.Log(position);
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

    void setAcution()
    {
        actionInfo = new Dictionary<EnemyState, float>();

        if (isSumonsEnemy())
        {
            actionInfo.Add(EnemyState.NormalAttack, 10f);
            actionInfo.Add(EnemyState.FlameBreath, 10f);
            actionInfo.Add(EnemyState.Summons, 80f);
            return;
        }

        if (Distance() <= Range)
        {
            actionInfo.Add(EnemyState.NormalAttack, 80f);
            actionInfo.Add(EnemyState.FlameBreath, 0f);
            actionInfo.Add(EnemyState.RandomMove, 20f);
        }
        else
        {
            actionInfo.Add(EnemyState.NormalAttack, 0f);
            actionInfo.Add(EnemyState.FlameBreath, 70f);
            actionInfo.Add(EnemyState.RandomMove, 30f);
        }
    }




    void acutionSelect()
    {
        float total = 0;

        foreach (KeyValuePair<EnemyState, float> elem in actionInfo)
        {
            total += elem.Value;
        }

        float randomPoint = Random.value * total;

        foreach (KeyValuePair<EnemyState, float> elem in actionInfo)
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

    void NormalAttack()
    {
        m_animator.SetBool("Attack1", true);
    }

    void FlameBreath()
    {
        m_animator.SetBool("Attack2", true);
    }

    void Summons()
    {
        
        m_animator.SetBool("Attack3", true);
        
        StartCoroutine(SumonsCount());

    }

    void AttackEvent()
    {
        Instantiate(shotObject, m_EyePoint.transform.position, m_EyePoint.transform.rotation);
    }


    //ìGÇè¢ä´Ç∑ÇÈ
    private IEnumerator SumonsCount()
    {
        if (!isSumonsEnemy())
            yield break;

        for (int i = 0; i < sumonsEnemy.Length; i++)
        {
            sumonsEnemyCount += 1;
            yield return new WaitForSeconds(0.5f);
            angle = Random.Range(1, 360);
            Vector3 point = transform.position + Quaternion.Euler(0, angle, 0) * sumonsTarget;
            Instantiate(sumonsEnemy[i], point, Quaternion.identity);
        }
    }

    //ãﬂÇ≠Ç…è¢ä´ÇµÇΩìGÇÕÇ¢ÇÈÇ©ÅH
   bool isSumonsEnemy()
    {
        if(sumonsEnemyCount > 0)
            return false;

        return true;

    }

    float Distance()
    {
        return Vector3.Distance(m_PlayerLookPoint.position, m_EyePoint.position) ;
    }

    //éÄñSéû
    public override void Death()
    {
        base.Death();
        sceneChange.isBossDie = false;

    }

    private void OnTriggerEnter(Collider collision)
    {

        if (collision.gameObject.CompareTag("Player"))
        {
            collision.gameObject.GetComponent<PlayerMotion>().Damage(ATK);
        }
    }
}
