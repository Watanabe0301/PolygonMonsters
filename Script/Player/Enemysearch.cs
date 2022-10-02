using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemysearch : MonoBehaviour
{
    //　サーチした敵を入れる
    [SerializeField]
    private List<GameObject> enemyList;
    //　現在標的にしている敵
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
        //　敵を登録する
        if (col.tag == "Enemy"
            && !enemyList.Contains(col.gameObject)
        )
        {
            enemyList.Add(col.gameObject);
        }
    }

    void OnTriggerExit(Collider col)
    {
        //　敵がサーチエリアを抜けたらリストから削除
        if (col.tag == "Enemy"
            && enemyList.Contains(col.gameObject)
        )
        {
            //　ターゲットになっていたらターゲットを解除
            if (col.gameObject == enemyTarget)
            {
                enemyTarget = null;
            }
            enemyList.Remove(col.gameObject);
        }
    }

    //　現在のターゲットを返す
    public GameObject GetNowTarget()
    {
        return enemyTarget;
    }

    //　敵が死んだ時に呼び出して敵をリストから外す
    public void DeleteEnemyList(GameObject obj)
    {
        if (enemyTarget == obj)
        {
            enemyTarget = null;
        }
        enemyList.Remove(obj);
    }

    //　ターゲットを設定
    public void EnemyTargetSet()
    {

        //　一番近い敵を標的に設定する
        foreach (var enemy in enemyList)
        {
            //　ターゲットがいなくて敵との間に壁がなければターゲットにする
            if (enemyTarget == null)
            {
                if (!Physics.Linecast(transform.parent.position + Vector3.up, enemy.transform.position + Vector3.up, LayerMask.GetMask("Field")))
                {
                    enemyTarget = enemy;
                }
                //　ターゲットがいる場合で今の敵の方が近ければ今の敵をターゲットにする
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
