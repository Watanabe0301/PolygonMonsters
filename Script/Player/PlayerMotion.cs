using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class PlayerMotion : MonoBehaviour
{
    enum PlayerState
    {
        Move,
        Attack,
        defense,
        Damage,
        Wall,
        Death
    }
    PlayerState p_state;

    [SerializeField] public float m_PlayerHP;
    [SerializeField] public float m_PlayerMaxHP;
    [SerializeField] public float m_PlayerStamina;
    [SerializeField] public float m_PlayerMaxStamina;
    [SerializeField] public float m_CharacterSpeed;
    [SerializeField] public float m_CharacterWalkSpeed;
    [SerializeField] public float m_CharacterRunSpeed;
    [SerializeField] public float m_recovery = 20;
    [SerializeField] public float m_CharacterWallSpeed = 1.5f;
    [SerializeField] public float m_Gravity = -3;
    [SerializeField] public float m_JumpPower = -3;
    [SerializeField] public int m_PotionNum = 0;


    private float WallDistance;
    Vector3 WallPoint;

    public GameObject sword;
    public GameObject shield;
    public GameObject weaponPod;
    public GameObject swordOn;


    //�T�E���h�Ǘ�
    GameObject m_sound;
    SoundManager soundManager;

    [SerializeField] GameObject swordEffect;
    [SerializeField] GameObject dustEffect;
    [SerializeField] GameObject grassEffect;
    [SerializeField] GameObject healingEffect;

    public GameObject shildOn;
    public GameObject shieldEffect;
    //���̈ʒu�p���i�[�p�x�N�g��
    Vector3 swordOnPos;
    Vector3 swordOnRot;
    //���̈ʒu�p���i�[�p�x�N�g��
    Vector3 shieldOnPos;
    Vector3 shieldOnRot;

    //���̃R���C�_�[
    private Collider shildCollider;
    //���̃R���C�_�[
    private Collider swordCollider;

    //static int s_AttackStateHash = Animator.StringToHash("Attack");
    static int s_MoveStateHash = Animator.StringToHash("Locomotion");
    static int s_DamageStateHash = Animator.StringToHash("Damage");
    static int s_AttackStateHash = Animator.StringToHash("AttackState");
    int shortnameHash;

    //�v���C���[�̃{�[���ʒu
    public Transform m_Player_RHand;
    public Transform m_Player_LHand;
    public Transform m_Player_Eye;

    //�L�����N�^�[�R���g���[�����Q��
    CharacterController m_characterController;
   
    //�A�j���[�V�������Q��
    Animator m_Animator; //�A�j���[�V����
    //�e��t���O
    bool isAttack = false, isBlock = false, isWallGrab = false, isDeath = false, isWeapon = false , isHeal = false , isDirt = false;
    //�ړ�Y
    float m_VelocityY = 0;
    //�v���C���[�Q�[�W
    protected PlayerGage playerGage;
    Object Attackeffect;
    [SerializeField] private Text PotionText;
    [SerializeField] private Canvas WallGrabUI;

    //�V�[���}�l�[�W���[
    GameObject SceneManager;
    SceneChange sceneChange;

    void Start()
    {


        SceneManager = GameObject.Find("SceneManager");
        sceneChange = SceneManager.GetComponent<SceneChange>();

        WallGrabUI.enabled = false;
        
        m_sound = GameObject.FindGameObjectWithTag("SoundManager");
        soundManager = m_sound.GetComponent<SoundManager>();

        // CharacterController�̎Q�Ƃ��擾
        m_characterController = GetComponent<CharacterController>();
        // Animator�̎Q�Ƃ��擾
        m_Animator = GetComponent<Animator>();

        //PotionText = GetComponentInChildren<Text>();

        swordCollider = GameObject.Find("sword").GetComponent<BoxCollider>();

        swordOn = sword.transform.parent.gameObject;
        shildOn = shield.transform.parent.gameObject;

        //������Ԃ̈ʒu���L�����Ă���
        swordOnPos = sword.transform.localPosition;
        swordOnRot = sword.transform.localEulerAngles;
        //���������
        shieldOnPos = shield.transform.localPosition;
        shieldOnRot = shield.transform.localEulerAngles;


        //swordPod��Ԃ̂Ƃ��̐e�q�֌W�ɕύX���[�߂���Ԃɂ���
        sword.gameObject.transform.parent = weaponPod.gameObject.transform;
        sword.gameObject.transform.localPosition = Vector3.zero;
        sword.gameObject.transform.localEulerAngles = Vector3.zero;
        //shieldPod��Ԃ̂Ƃ��̐e�q�֌W�ɕύX���[�߂���Ԃɂ���
        shield.gameObject.transform.parent = weaponPod.gameObject.transform;
        shield.gameObject.transform.localPosition = Vector3.zero;
        shield.gameObject.transform.localEulerAngles = Vector3.zero;


        playerGage = GameObject.FindObjectOfType<PlayerGage>();
        playerGage.SetPlayer(this);



    }

    void FixedUpdate()
    {
        
        PotionText.text = ""+m_PotionNum; 
        RayWall();

        shortnameHash = m_Animator.GetCurrentAnimatorStateInfo(0).shortNameHash;

        ActionChange();
        
        //RayWallDistat();

        if (Input.GetButtonDown("Fire4") && shortnameHash == s_MoveStateHash)
        {

            WeaponArmed();
        }

        if (Input.GetButtonDown("Fire5"))
        {
            usePotion();
        }
        
        GrandRayChack();
        
        

    }


    //�s���J��
    void ActionChange()
    {
        switch (p_state)
        {
            case PlayerState.Move:
                Move();
                RayWall();


                break;

            case PlayerState.Wall:
                RayWall();
                WallClimbing();
                break;
        }
    }


    //�ʏ�ړ�����
    void Move()
    {
        if (Input.GetButtonDown("Fire1") && isWeapon && PlayerAction())
        {
            isAttack = true;
            m_Animator.SetTrigger("Attack");
            Attack();
        }
        else
            isAttack = false;
        
        if (Input.GetButtonDown("Fire3") || m_PlayerStamina < m_PlayerMaxStamina)
        useStamina();


        Vector3 forward = Camera.main.transform.forward;

        // y�����𖳎����Đ����ɂ���
        forward.y = 0;

        // ���K���i�x�N�g���̒�����1�ɂ���j
        forward.Normalize();

        // ���x�����߂�i1�b������̈ړ��ʁj
        Vector3 velocity = Vector3.zero;

        if (PlayerAction() && shortnameHash == s_MoveStateHash)
        {
            velocity = forward * Input.GetAxis("Vertical") * m_CharacterSpeed
            + Camera.main.transform.right * Input.GetAxis("Horizontal") * m_CharacterSpeed;
        }


        m_Animator.SetFloat("Speed", velocity.magnitude);

        // �O����������
        if (velocity.magnitude > 0)
        {
            transform.LookAt(transform.position + velocity);
        }

        // �ڒn���Ă���Ȃ�
        if (m_characterController.isGrounded)
        {
            m_VelocityY = 0;
        }

        // �d�͂ŉ�����
        m_VelocityY += m_Gravity * Time.deltaTime;

        // �ڒn���ɁA�W�����v�{�^���ŃW�����v����
        if (m_characterController.isGrounded && Input.GetButtonDown("Jump"))
        {
            m_Animator.SetTrigger("Jump");
            m_VelocityY = m_JumpPower;
        }
        // y�����̑��x��K�p
        velocity.y = m_VelocityY;

        // ���t���[���̈ړ��ʂ����߂�
        Vector3 movement = velocity * Time.deltaTime;

        // CharacterController�ɖ��߂��Ĉړ�����
        m_characterController.Move(movement);
        m_Animator.SetBool("IsGrounded", m_characterController.isGrounded);

    }

    void GrapWall()//�ǂ�͂ޏ���
    {
        if (isWallGrab && Input.GetButtonDown("Jump"))
        {
            p_state = PlayerState.Wall;
            m_Animator.SetBool("Grabwall", true);
        }
        else if (!isWallGrab)
        {
            p_state = PlayerState.Move;
            m_Animator.SetBool("Grabwall", false);
        }

    }

    //�v���C���[�̏���
    void StateChange()
    {

        switch (p_state)
        {
            case PlayerState.Move:

                if (isWallGrab)
                    p_state = PlayerState.Wall;

                if (isDeath)
                    p_state = PlayerState.Death;

                if (isAttack)
                    Attack();

                if (isBlock)
                    Block();

                break;

            case PlayerState.Wall:
                if (!isWallGrab)
                    p_state = PlayerState.Move;
                break;

            case PlayerState.Death:

                break;
        }

    }



    //�h�䏈��
    public void Block()
    {
    }
    
    //�U������
    public void Attack()
    {
        
    }
    //�U������
    void AttackStart(Object effect)
    {
        swordCollider.enabled = true;
        
        Attackeffect = Instantiate(effect, sword.transform.position, gameObject.transform.rotation);
        Destroy(Attackeffect,1f);

        
    }
    void AttackEnd(int x)
    {
        swordCollider.enabled = false;
        
    }

    void WalkingSE(int value)
    {
        if (isDirt)
        {
            soundManager.PlaySeByName("footstep_dirt_land_0" + value);
            return;
        }
        soundManager.PlaySeByName("footstep_grass_run_0" + value);
    }


    //�_���[�W����
    public void Damage(float damage)
    {
        if (shortnameHash != s_DamageStateHash && !isBlock && !isDeath)
        {
            playerGage.GaugeReduce(damage);
            m_PlayerHP -= damage;
            soundManager.PlaySeByName("voice_male_grunt_pain_01");
            m_Animator.SetTrigger("Damage");
        }

        if (m_PlayerHP <= 0 && !isDeath)
        {
            isDeath = true;
            soundManager.PlaySeByName("voice_male_grunt_pain_death_04");
            m_Animator.SetTrigger("Death");
            StartCoroutine(sceneChange.DeliriaSceneChage("GameoverScene"));
        }

    }



    //�v���C���[���s�����Ă邩�H
    bool PlayerAction()
    {
        if (isAttack)
            return false;
        if (isBlock)
            return false;
        if (isDeath)
            return false;

        return true;
    }

    
    

    //�����̒n�ʂ̊m�F
    public void GrandRayChack()
    {

        RaycastHit hitInfo;
        bool hit = Physics.Raycast(transform.position, new Vector3(0, -1, 0), out hitInfo);
        
        if(hit == false)
        {
            dustEffect.SetActive(false);
            grassEffect.SetActive(false);
        }

        if (hitInfo.collider.CompareTag("Dust")) {
            dustEffect.SetActive(true);
            grassEffect.SetActive(false);
            isDirt = true;
        }
        else if (hitInfo.collider.CompareTag("Grass"))
        {
            dustEffect.SetActive(false);
            grassEffect.SetActive(true);
            isDirt = false;
        }

    }

    //�X�^�~�i����
    public void useStamina()
    {

        if(Input.GetButton("Fire3") && m_PlayerStamina >= 0)
        {
            if (isWeapon)
            {
                isBlock = true;
                shieldEffect.active = true;

                m_Animator.SetBool("Block", true);
            }
            else
            {
                m_CharacterSpeed = m_CharacterRunSpeed;
            }
            m_PlayerStamina -= Time.deltaTime * 3;
        }
        else
        {
            shieldEffect.active = false;
            isBlock = false;
            m_Animator.SetBool("Block", false);
            m_CharacterSpeed = m_CharacterWalkSpeed;
            m_PlayerStamina += Time.deltaTime * 4;
        }
        playerGage.StaminaGaugeReduce(m_PlayerStamina);


    }


    //�񕜏���
    public void usePotion()
    {
        if (m_PotionNum >= 0 && m_PlayerHP < m_PlayerMaxHP )
        {
            isHeal = true;
        }

        if(isHeal)
            StartCoroutine("Heal");
    }

    public void takePotion()
    {
        m_PotionNum += 1;
    }

    private IEnumerator Heal()
    {
        isHeal = false;
        soundManager.PlaySeByName("potion_heal_flask_spell_02");
        m_PlayerHP += m_recovery;

        if (m_PlayerHP >= m_PlayerMaxHP)
            m_PlayerHP = m_PlayerMaxHP;
        
        playerGage.GaugeRecovery(m_recovery);

        healingEffect.SetActive(true);

        

        yield return new WaitForSeconds(3f);
        m_PotionNum -= 1;
        healingEffect.SetActive(false);
        isHeal = false;
    }

    //����𑕔�����
    public void WeaponArmed()
    {
        m_Animator.SetTrigger("ArmedWepon");
        StartCoroutine("ArmedWeaponMetod");


    }
    private IEnumerator ArmedWeaponMetod()
    {

        if (isWeapon == false)
        {
            
            yield return new WaitForSeconds(0.5f);
            //����������̎q�I�u�W�F�N�g�֕ύX
            sword.gameObject.transform.parent = swordOn.gameObject.transform;
            //�I�u�W�F�N�g�𔲓���Ԃ̃|�W�V�����ֈړ�
            sword.gameObject.transform.localPosition = swordOnPos;
            sword.gameObject.transform.localEulerAngles = swordOnRot;

            soundManager.PlaySeByName("���𔲂�");

            yield return new WaitForSeconds(0.6f);
            //����������̎q�I�u�W�F�N�g�֕ύX
            shield.gameObject.transform.parent = shildOn.gameObject.transform;
            //�I�u�W�F�N�g�𔲓���Ԃ̃|�W�V�����ֈړ�
            shield.gameObject.transform.localPosition = swordOnPos;
            shield.gameObject.transform.localEulerAngles = swordOnRot;
            isWeapon = true;
        }
        else if (isWeapon == true)
        {
            yield return new WaitForSeconds(0.5f);
            //����Pod�̎q�I�u�W�F�N�g�֕ύX
            sword.gameObject.transform.parent = weaponPod.gameObject.transform;
            // ����Pod�̎q�I�u�W�F�N�g�֕ύX
            sword.gameObject.transform.localPosition = Vector3.zero;
            sword.gameObject.transform.localEulerAngles = Vector3.zero;
            soundManager.PlaySeByName("������ɂ��܂�");
            yield return new WaitForSeconds(0.5f);
            shield.gameObject.transform.parent = weaponPod.gameObject.transform;
            shield.gameObject.transform.localPosition = Vector3.zero;
            shield.gameObject.transform.localEulerAngles = Vector3.zero;
            isWeapon = false;
        }


        
    }

    /// <summary>
    /// Ray���g�p���ǂ̔���E�����𒲂ׂ�
    /// </summary>
    public void RayWall()
    {
        WallGrabUI.enabled = false;
        Ray ori = new Ray(m_Player_Eye.position�@, m_Player_Eye.transform.forward);
        Debug.DrawRay(m_Player_Eye.position, m_Player_Eye.transform.forward , Color.red);
        RaycastHit hit1;
        if (Physics.Raycast(ori, out hit1))
        {
            if (hit1.collider.CompareTag("GrabWall"))
            {
                WallPoint = hit1.point;
                WallDistance = Vector3.Distance(hit1.point, m_Player_Eye.position);
            }
            else
            {
                WallDistance = 5;
            }
        }

        if (WallDistance <= 0.26)
        {
            isWallGrab = true;
            WallGrabUI.enabled = true;
        }

        Ray ray = new Ray(m_Player_Eye.position + new Vector3(0, 1, 1), new Vector3(0,-1,0));
        Debug.DrawRay(m_Player_Eye.position + new Vector3(0, 1, 1), new Vector3(0, -1, 0) , Color.blue);
        RaycastHit hit2;
        if (Physics.Raycast(ray, out hit2))
        {
            
        }

        //�W�����v�ɕǂɒ���t��
        if (isWallGrab && Input.GetButtonDown("Jump"))
        {
            p_state = PlayerState.Wall;
            m_Animator.SetBool("Grabwall", true);
        }

        //�R�m�F
        if ((hit2.point.y - hit1.point.y) <= 0.17 && (hit2.point.y - hit1.point.y) >= 0)
        {
            StartCoroutine("ClimeEnd");
        }
    }
    

    

    //�ǂ�o�鏈��
    public void WallClimbing()
    {
        WallGrabUI.enabled = false;
        Vector3 Up = this.transform.up;
        Vector3 Right = this.transform.right;
        Up.z = 0;

        Vector3 velocity = Up * Input.GetAxis("Vertical") * m_CharacterWallSpeed
         + Right * Input.GetAxis("Horizontal") * m_CharacterWallSpeed;
        

        Vector3 movement = velocity * Time.deltaTime;
        m_characterController.Move(movement);
        

        m_Animator.SetFloat("Wallspeed", velocity.y);
        //�ǂ̂ڂ蕔��
    }

    private IEnumerator ClimeEnd()
    {
        isWallGrab = false;
       
        m_Animator.SetBool("Grabwall", false);
        m_Gravity = 0;
        yield return new WaitForSeconds(1f);
        m_Gravity = -3;
        p_state = PlayerState.Move;
    }
}
