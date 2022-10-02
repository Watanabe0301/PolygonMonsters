using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemysearch : MonoBehaviour
{
    //�@�T�[�`�����G������
    [SerializeField]
    private List<GameObject> enemyList;
    //�@���ݕW�I�ɂ��Ă���G
    [SerializeField]
    public GameObject enemyTarget;
    
    void Start()
    {
       
        enemyList = new List<GameObject>();
        enemyTarget = null;
    }

    void Update()
    {
        
        EnemyTargetSet();
    }

    public void TargetEnemy()
    {

    }

    void OnTriggerStay(Collider col)
    {
        Debug.DrawLine(transform.parent.position + Vector3.up, col.gameObject.transform.position + Vector3.up, Color.blue);
        //�@�G��o�^����
        if (col.tag == "Enemy"
            && !enemyList.Contains(col.gameObject)
        )
        {
            enemyList.Add(col.gameObject);
        }
    }

    void OnTriggerExit(Collider col)
    {
        //�@�G���T�[�`�G���A�𔲂����烊�X�g����폜
        if (col.tag == "Enemy"
            && enemyList.Contains(col.gameObject)
        )
        {
            //�@�^�[�Q�b�g�ɂȂ��Ă�����^�[�Q�b�g������
            if (col.gameObject == enemyTarget)
            {
                enemyTarget = null;
            }
            enemyList.Remove(col.gameObject);
        }
    }

    //�@���݂̃^�[�Q�b�g��Ԃ�
    public GameObject GetNowTarget()
    {
        return enemyTarget;
    }

    //�@�G�����񂾎��ɌĂяo���ēG�����X�g����O��
    public void DeleteEnemyList(GameObject obj)
    {
        if (enemyTarget == obj)
        {
            enemyTarget = null;
        }
        enemyList.Remove(obj);
    }

    //�@�^�[�Q�b�g��ݒ�
    public void EnemyTargetSet()
    {

        //�@��ԋ߂��G��W�I�ɐݒ肷��
        foreach (var enemy in enemyList)
        {
            //�@�^�[�Q�b�g�����Ȃ��ēG�Ƃ̊Ԃɕǂ��Ȃ���΃^�[�Q�b�g�ɂ���
            if (enemyTarget == null)
            {
                if (!Physics.Linecast(transform.parent.position + Vector3.up, enemy.transform.position + Vector3.up, LayerMask.GetMask("Field")))
                {
                    enemyTarget = enemy;
                }
                //�@�^�[�Q�b�g������ꍇ�ō��̓G�̕����߂���΍��̓G���^�[�Q�b�g�ɂ���
            }
            else if (Vector3.Distance(transform.parent.position, enemy.transform.position) < Vector3.Distance(transform.parent.position, enemyTarget.transform.position)
              && !Physics.Linecast(transform.parent.position + Vector3.up, enemy.transform.position + Vector3.up, LayerMask.GetMask("Field"))
          )
            {
                enemyTarget = enemy;
            }
        }
    }

}
