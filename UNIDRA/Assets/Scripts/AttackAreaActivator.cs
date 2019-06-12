using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackAreaActivator : MonoBehaviour
{
    // attackAreaColliders라는 이름의 collider배열 선언
    Collider[] attackAreaColliders;

    public AudioClip attackSeClip;
    AudioSource attackSeAudio;

    // Start is called before the first frame update
    void Start()
    {
        //AttackArea 스크립트를 자신이나 자기 자식에 추가한 오브젝트들을 찾아서 넣는다
        AttackArea[] attackAreas = GetComponentsInChildren<AttackArea>();

        // 이 수만큼 충돌체 배열 사이즈 설정
        attackAreaColliders = new Collider[attackAreas.Length];

        for(int i = 0; i < attackAreas.Length; i++)
        {
            // attackarea 스크립트 있는 오브젝트 각각의 충돌체를 배열에 넣어준다
            attackAreaColliders[i] = attackAreas[i].GetComponent<Collider>();
            // 초기 유무효값은 false
            attackAreaColliders[i].enabled = false;
        }

        //오디오 초기화
        attackSeAudio = gameObject.AddComponent<AudioSource>();
        attackSeAudio.clip = attackSeClip;
        attackSeAudio.loop = false;
        
    }

    //공격 애니메이션 타이밍에 따른 충돌체의 유효/무효 설정
    void StartAttackHit()
    {
        foreach(Collider attackAreaCollider in attackAreaColliders)
        {
            attackAreaCollider.enabled = true;
        }

        attackSeAudio.Play();
    }

    void EndAttackHit()
    {
        foreach (Collider attackAreaCollider in attackAreaColliders)
        {
            attackAreaCollider.enabled = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
