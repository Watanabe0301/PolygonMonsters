using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public abstract class Enemy : MonoBehaviour
{

    //�X�e�[�^�X
    [SerializeField] public string enemyName;
    [SerializeField] public int enemyLv;
    [SerializeField] public float HP;
    [SerializeField] public float MaxHP;
    [SerializeField] public float ATK;
    [SerializeField] public float MoveSpeed;
    //�t���O
    protected bool isDeath = false , IsAttack = false;
    
    protected GameObject m_sound;
    protected SoundManager soundManager;
    protected GameObject m_cSVLoader;

    protected GameObject m_Player;
    protected NavMeshAgent m_agent;
    protected Animator m_animator;

    public float m_ViewingDistance;
    public float m_viewingAngle;
    public float m_AttackRange;
    public Transform m_PlayerLookPoint;
    public Transform m_EyePoint;
    public GameObject Hitattackparticle;
    public Collider attackCollider;
    GameObject m_enemysearchObject;

    private float angle;
    public Vector3 origin;
    protected Vector3 m_Movepoint;

    protected Enemysearch m_enemysearch;
    public void Awake()
    {
        m_cSVLoader = GameObject.Find("Csv");
    }
    protected virtual void Start()
    {
        m_sound = GameObject.FindGameObjectWithTag("SoundManager");

        m_enemysearchObject = GameObject.Find("SearchPoint");
        m_enemysearch = m_enemysearchObject.GetComponent<Enemysearch>();


        soundManager = m_sound.GetComponent<SoundManager>();
        m_Player = GameObject.FindGameObjectWithTag("Player");
        m_PlayerLookPoint = GameObject.Find("PlayerEyePoint").transform;
        m_agent = GetComponent<NavMeshAgent>();
        m_animator = GetComponent<Animator>();
        LevelSet();
    }
    

    //�_���[�W����
    public virtual void Damege(int EnemyDamage , Vector3 hitpos)
    {
        if (isDeath)
            return;

        HP -= EnemyDamage;
        soundManager.PlaySeByName("���Ŏa��2");
        if (HP <= 0 && !isDeath)
        {
            m_enemysearch.DeleteEnemyList(gameObject);
            Death();
        }
          
        

        if (!isDeath)
        {
            Instantiate(Hitattackparticle, hitpos , Quaternion.identity);
            m_animator.SetBool("Damage", true);
            m_animator.SetTrigger("Damage");
        }
    }

    /// <summary>
    /// ���S������
    /// </summary>
    public virtual void Death()
    {
        m_enemysearch.DeleteEnemyList(gameObject);
        isDeath = true;
        MoveSpeed = 0;
        m_animator.SetTrigger("Death");
        Destroy(gameObject,5f);
    }
     
    //�ڐG����
    void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.CompareTag("Player") && IsAttack)
        {
            collision.gameObject.GetComponent<PlayerMotion>().Damage(ATK);
            IsAttack = false;
        }
    }

    //�U������
    void AttackStart()
    {
        IsAttack = true;
        attackCollider.enabled = true;
    }
    void AttackEnd()
    {
        IsAttack = false;
        attackCollider.enabled = false;
    }


    /// <summary>
    /// �v���C���[�Ƃ̋����𔻒肷��
    /// </summary>
    /// <returns></returns>
    public bool IsPlayerViewDistance()
    {
        float distanceToPlayer = Vector3.Distance(m_PlayerLookPoint.position, m_EyePoint.position);

        return distanceToPlayer <= m_ViewingDistance;
    }

    //����p�Ƀv���C���[�����邩
    public bool IsPlayerInViewingAngle()
    {
        Vector3 directionToPlayer = m_PlayerLookPoint.position - m_EyePoint.position;
        float angleToplayer = Vector3.Angle(m_EyePoint.forward, directionToPlayer);
        return angleToplayer <= m_viewingAngle;
    }

    /// <summary>
    /// �U���͈͓��ɂ��邩��Ԃ�
    /// </summary>
    /// <returns></returns>
    public bool IsAttackDistance()
    {
        float AttackToPlayer = Vector3.Distance(m_PlayerLookPoint.position, m_EyePoint.position);
        return AttackToPlayer <= m_AttackRange;
    }

    //�v���C���[�Ƃ̊ԂɏՓ˔��肪���邩�H
    public bool CanHitRayPlayer()
    {
        Vector3 directionToPlayer = m_PlayerLookPoint.position - m_EyePoint.position;
        RaycastHit hitInfo;
        Debug.DrawRay(m_EyePoint.position, directionToPlayer, Color.red);
        bool hit = Physics.Raycast(m_EyePoint.position, directionToPlayer, out hitInfo);
        return hit && hitInfo.collider.CompareTag("Player");
    }


    /// <summary>
    /// �v���C���[�������邩��Ԃ�
    /// </summary>
    /// <returns></returns>
    public bool CanSeePlayer()
    {
        if (!IsPlayerViewDistance())
            return false;
        if (!IsPlayerInViewingAngle())
            return false;
        //if (!CanHitRayPlayer())
            //return false;

       
        return true;
    }
    /// <summary>
    /// ���x�������߂�
    /// </summary>
    public void LevelSet()
    {
        enemyLv = Random.RandomRange(1, 10);
    }

    /// <summary>
    /// �X�e�[�^�X��Ԃ�
    /// </summary>
    /// <param name="states">�����X�^�[��,���x��</param>
    public void statusSet(Dictionary<string, string> states)
    {
        HP = float.Parse(states["HP"]);
        ATK = float.Parse(states["ATK"]);
        MoveSpeed = float.Parse(states["SPEED"]);
        MaxHP = HP;
    }

    public IEnumerator Movepoint()
    {
        angle = Random.Range(1, 360);
        m_Movepoint = transform.position + Quaternion.Euler(0, angle, 0) * origin;
        
        yield return new WaitForSeconds(10f);
        StartCoroutine(Movepoint());
    }

    

}
