using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitArea : MonoBehaviour
{
    // attackarea에서 sendmessage로 호출될 함수
   void Damage(AttackArea.AttackInfo attackInfo)
   {

        // 피격 대상에 damage 와 attackinfo 정보 전달
        transform.root.SendMessage("Damage", attackInfo);
    }
}
