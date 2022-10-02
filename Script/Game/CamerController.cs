using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class CamerController : MonoBehaviour
{
    public Transform m_Target;
    Vector3 m_target;
    public float m_Radius;
    [SerializeField] public float m_RotateYspeed;
    [SerializeField]
    [Tooltip("ç≈è¨äpìx(-180Å`180")]
    private float MinAngle;

    [SerializeField]
    [Tooltip("ç≈ëÂäpìx(-180Å`180")]
    private float MaxAngle;
    public float m_RotateX;
    public float m_RotateY;

    Transform LockOn;
    //public Vector3 m_LockOnRadius;
    public float m_LockOnRadius;
    bool Axis = true;
    bool isLockOnMode = false;
    GameObject LockOnTarget;
    public Enemysearch enemysearch;
    GameManager gameManager;
    private void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
    }

    void Update()
    {
        if (SceneManager.GetActiveScene().name != "PlayScene")
            return;
        
        if (Input.GetButtonDown("LockOn"))
        {
            if(enemysearch.GetNowTarget() != null || isLockOnMode)
            {
                isLockOnMode = !isLockOnMode;
                LockOnTarget = enemysearch.GetNowTarget();
            }
        }

        if (isLockOnMode)
        {
            m_RotateY = 0;
            enemyLockOn(LockOnTarget);
        }
        else
        {
            CameraController();
        }

    }

    //ìGÇÉçÉbÉNÉIÉì
    void enemyLockOn(GameObject Target)
    {
        if (LockOn == null)
        {
            isLockOnMode = !isLockOnMode;
        }

        
        //É^Å[ÉQÉbÉgà íu
        LockOn = Target.transform;

        Vector3 pos = m_Target.position - LockOn.position;
        pos = pos.normalized;
        transform.position = Vector3.Lerp(transform.position , m_Target.position + pos * m_LockOnRadius, Time.deltaTime * 10.0f);
        transform.LookAt(m_Target);

    }

    //ÉJÉÅÉâêßå‰
    void CameraController()
    {
        Axis = gameManager.cameraAxisSet;
        
        if (!Axis)
        {
            m_RotateY -= Input.GetAxis("CameraHorizontal") * gameManager.cameraYspeedSet * Time.deltaTime;
        }
        else if (Axis)
        {
            m_RotateY += Input.GetAxis("CameraHorizontal") * gameManager.cameraYspeedSet * Time.deltaTime;
        }
        
        transform.position = m_Target.position + Quaternion.Euler(m_RotateX, m_RotateY, 0) * Vector3.back * m_Radius;
        transform.LookAt(m_Target);
    }
    
    
    //ÉJÉÅÉâîΩì]
    public void CameraAxis()
    {
        Axis = gameManager.cameraAxisSet;
    }
}
