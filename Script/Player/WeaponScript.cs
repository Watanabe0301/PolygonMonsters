using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponScript : MonoBehaviour
{
    GameObject m_Player;
    
    public GameObject Hitattackparticle;
    //public GameObject AttackHit;
    [SerializeField] public int WeaponPower = 5;
    bool IsAttackHit = false;
    private void Start()
    {
        m_Player = GameObject.FindWithTag("Player");
    }
    
    


    void OnTriggerEnter(Collider other)
    {
        
        Vector3 hitPos = other.ClosestPointOnBounds(this.transform.position);
        //UŒ‚‚µ‚½‘Šè‚ªEnemy‚Ìê‡
        if (other.gameObject.CompareTag("Enemy") )
        {
            //&& !IsAttackHit
            //Instantiate(Hitattackparticle, collision.gameObject.transform.localPosition , Quaternion.identity);

            other.gameObject.GetComponent<Enemy>().Damege(WeaponPower , hitPos);
            IsAttackHit = true;
        }
        

    }
    void OnTriggerExit(Collider other)
    {

        //UŒ‚‚µ‚½‘Šè‚ªEnemy‚Ìê‡
        if (other.gameObject.CompareTag("Enemy") && IsAttackHit)
        {
            
            IsAttackHit = false;
        }
    }
}
