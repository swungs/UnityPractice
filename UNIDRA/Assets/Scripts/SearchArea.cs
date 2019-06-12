using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SearchArea : MonoBehaviour
{
    EnemyCtrl enemyCtrl;

    // Start is called before the first frame update
    void Start()
    {
        enemyCtrl = transform.root.GetComponent<EnemyCtrl>();    
    }

    // 콜라이더에 뭔가가 있는데
    void OnTriggerStay(Collider other)
    {
        // 그것의 태그에 플레이어가 있으면
        if(other.tag == "Player")
        {
            // 그것을 공격 대상으로 설정한다
            enemyCtrl.SetAttackTarget(other.transform);
        }
    }
}
