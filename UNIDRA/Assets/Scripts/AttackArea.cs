using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackArea : MonoBehaviour

{
    CharacterStatus status;
    public AudioClip hitSeClip;
    AudioSource hitSeAudio;

    // Start is called before the first frame update
    void Start()
    {
        status = transform.root.GetComponent<CharacterStatus>();
        hitSeAudio = gameObject.AddComponent<AudioSource>();
        hitSeAudio.clip = hitSeClip;
        hitSeAudio.loop = false;
    }

    public class AttackInfo
    {
        // 공격력
        public int attackPower;

        // 공격자
        public Transform attacker;
    }

    //공격 정보 가져오기
    AttackInfo GetAttackInfo()
    {
        AttackInfo attackInfo = new AttackInfo();

        //공격력 계산
        attackInfo.attackPower = status.Power;

        if(status.powerBoost)
        {
            attackInfo.attackPower += attackInfo.attackPower;
        }
        attackInfo.attacker = transform.root;

        return attackInfo;
    }

    // 공격 처리. 공격 정보에 따라 대미지를 주고 대미지를 준 대상을 lastAttackTarget으로 설정한다
    private void OnTriggerEnter(Collider other)
    {
        // hit area 함수가 적용되어 있는 컴포넌트의 damage 함수를 호출, 뒤의 getattackinfo()는 함수에 필요한 인자값
        other.SendMessage("Damage", GetAttackInfo());
        status.lastAttackTarget = other.transform.root.gameObject;

        //타격 사운드 재생
        hitSeAudio.Play();

    }

    void OnAttack()
    {
        GetComponent<Collider>().enabled = true;
        //collider.enabled = true;
    }
    void OnAttackTermination()
    {
        GetComponent<Collider>().enabled = false;
        //collider.enabled = false;
    }
}
