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


    //サウンド管理
    GameObject m_sound;
    SoundManager soundManager;

    [SerializeField] GameObject swordEffect;
    [SerializeField] GameObject dustEffect;
    [SerializeField] GameObject grassEffect;
    [SerializeField] GameObject healingEffect;

    public GameObject shildOn;
    public GameObject shieldEffect;
    //剣の位置姿勢格納用ベクトル
    Vector3 swordOnPos;
    Vector3 swordOnRot;
    //盾の位置姿勢格納用ベクトル
    Vector3 shieldOnPos;
    Vector3 shieldOnRot;

    //盾のコライダー
    private Collider shildCollider;
    //剣のコライダー
    private Collider swordCollider;

    //static int s_AttackStateHash = Animator.StringToHash("Attack");
    static int s_MoveStateHash = Animator.StringToHash("Locomotion");
    static int s_DamageStateHash = Animator.StringToHash("Damage");
    static int s_AttackStateHash = Animator.StringToHash("AttackState");
    int shortnameHash;

    //プレイヤーのボーン位置
    public Transform m_Player_RHand;
    public Transform m_Player_LHand;
    public Transform m_Player_Eye;

    //キャラクターコントローラを参照
    CharacterController m_characterController;
   
    //アニメーションを参照
    Animator m_Animator; //アニメーション
    //各種フラグ
    bool isAttack = false, isBlock = false, isWallGrab = false, isDeath = false, isWeapon = false , isHeal = false , isDirt = false;
    //移動Y
    float m_VelocityY = 0;
    //プレイヤーゲージ
    protected PlayerGage playerGage;
    Object Attackeffect;
    [SerializeField] private Text PotionText;
    [SerializeField] private Canvas WallGrabUI;

    //シーンマネージャー
    GameObject SceneManager;
    SceneChange sceneChange;

    void Start()
    {


        SceneManager = GameObject.Find("SceneManager");
        sceneChange = SceneManager.GetComponent<SceneChange>();

        WallGrabUI.enabled = false;
        
        m_sound = GameObject.FindGameObjectWithTag("SoundManager");
        soundManager = m_sound.GetComponent<SoundManager>();

        // CharacterControllerの参照を取得
        m_characterController = GetComponent<CharacterController>();
        // Animatorの参照を取得
        m_Animator = GetComponent<Animator>();

        //PotionText = GetComponentInChildren<Text>();

        swordCollider = GameObject.Find("sword").GetComponent<BoxCollider>();

        swordOn = sword.transform.parent.gameObject;
        shildOn = shield.transform.parent.gameObject;

        //抜剣状態の位置を記憶しておく
        swordOnPos = sword.transform.localPosition;
        swordOnRot = sword.transform.localEulerAngles;
        //盾装備状態
        shieldOnPos = shield.transform.localPosition;
        shieldOnRot = shield.transform.localEulerAngles;


        //swordPod状態のときの親子関係に変更し納めた状態にする
        sword.gameObject.transform.parent = weaponPod.gameObject.transform;
        sword.gameObject.transform.localPosition = Vector3.zero;
        sword.gameObject.transform.localEulerAngles = Vector3.zero;
        //shieldPod状態のときの親子関係に変更し納めた状態にする
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


    //行動遷移
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


    //通常移動処理
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

        // y成分を無視して水平にする
        forward.y = 0;

        // 正規化（ベクトルの長さを1にする）
        forward.Normalize();

        // 速度を求める（1秒あたりの移動量）
        Vector3 velocity = Vector3.zero;

        if (PlayerAction() && shortnameHash == s_MoveStateHash)
        {
            velocity = forward * Input.GetAxis("Vertical") * m_CharacterSpeed
            + Camera.main.transform.right * Input.GetAxis("Horizontal") * m_CharacterSpeed;
        }


        m_Animator.SetFloat("Speed", velocity.magnitude);

        // 前を向かせる
        if (velocity.magnitude > 0)
        {
            transform.LookAt(transform.position + velocity);
        }

        // 接地しているなら
        if (m_characterController.isGrounded)
        {
            m_VelocityY = 0;
        }

        // 重力で下方向
        m_VelocityY += m_Gravity * Time.deltaTime;

        // 接地中に、ジャンプボタンでジャンプする
        if (m_characterController.isGrounded && Input.GetButtonDown("Jump"))
        {
            m_Animator.SetTrigger("Jump");
            m_VelocityY = m_JumpPower;
        }
        // y方向の速度を適用
        velocity.y = m_VelocityY;

        // 今フレームの移動量を求める
        Vector3 movement = velocity * Time.deltaTime;

        // CharacterControllerに命令して移動する
        m_characterController.Move(movement);
        m_Animator.SetBool("IsGrounded", m_characterController.isGrounded);

    }

    void GrapWall()//壁を掴む処理
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

    //プレイヤーの処理
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



    //防御処理
    public void Block()
    {
    }
    
    //攻撃処理
    public void Attack()
    {
        
    }
    //攻撃処理
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


    //ダメージ処理
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



    //プレイヤーが行動してるか？
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

    
    

    //足元の地面の確認
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

    //スタミナ処理
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


    //回復処理
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

    //武器を装備する
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
            //刀を持ち手の子オブジェクトへ変更
            sword.gameObject.transform.parent = swordOn.gameObject.transform;
            //オブジェクトを抜刀状態のポジションへ移動
            sword.gameObject.transform.localPosition = swordOnPos;
            sword.gameObject.transform.localEulerAngles = swordOnRot;

            soundManager.PlaySeByName("剣を抜く");

            yield return new WaitForSeconds(0.6f);
            //盾を持ち手の子オブジェクトへ変更
            shield.gameObject.transform.parent = shildOn.gameObject.transform;
            //オブジェクトを抜刀状態のポジションへ移動
            shield.gameObject.transform.localPosition = swordOnPos;
            shield.gameObject.transform.localEulerAngles = swordOnRot;
            isWeapon = true;
        }
        else if (isWeapon == true)
        {
            yield return new WaitForSeconds(0.5f);
            //刀をPodの子オブジェクトへ変更
            sword.gameObject.transform.parent = weaponPod.gameObject.transform;
            // 盾をPodの子オブジェクトへ変更
            sword.gameObject.transform.localPosition = Vector3.zero;
            sword.gameObject.transform.localEulerAngles = Vector3.zero;
            soundManager.PlaySeByName("剣を鞘にしまう");
            yield return new WaitForSeconds(0.5f);
            shield.gameObject.transform.parent = weaponPod.gameObject.transform;
            shield.gameObject.transform.localPosition = Vector3.zero;
            shield.gameObject.transform.localEulerAngles = Vector3.zero;
            isWeapon = false;
        }


        
    }

    /// <summary>
    /// Rayを使用し壁の判定・距離を調べる
    /// </summary>
    public void RayWall()
    {
        WallGrabUI.enabled = false;
        Ray ori = new Ray(m_Player_Eye.position　, m_Player_Eye.transform.forward);
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

        //ジャンプに壁に張り付く
        if (isWallGrab && Input.GetButtonDown("Jump"))
        {
            p_state = PlayerState.Wall;
            m_Animator.SetBool("Grabwall", true);
        }

        //崖確認
        if ((hit2.point.y - hit1.point.y) <= 0.17 && (hit2.point.y - hit1.point.y) >= 0)
        {
            StartCoroutine("ClimeEnd");
        }
    }
    

    

    //壁を登る処理
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
        //壁のぼり部分
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
