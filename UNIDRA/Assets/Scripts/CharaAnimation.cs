using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharaAnimation : MonoBehaviour {

    Animator animator;
    CharacterStatus status;
    Vector3 prePosition;
    bool isDown = false;
    bool attacked = false;

    public bool IsAttacked()
    {
        return attacked;
    }

    void StartAttackHit()
    {
        Debug.Log("StartAttackHit");
    }

    void EndAttackHit()
    {
        Debug.Log("EndAttackHit");
    }

    void EndAttack()
    {
        attacked = true;
    }

    // Use this for initialization
    void Start () {

        animator = GetComponent<Animator>();
        status = GetComponent<CharacterStatus>();

        // 초기화
        prePosition = transform.position;
	}
	
	// Update is called once per frame
	void Update () {

        // 이전 좌표와 현재 좌표의 차이를 구한다
        Vector3 delta_position = transform.position - prePosition;

        // 프레임 간격 당 이동 거리, 즉 속도를 계산해 애니메이터 speed 파라미터에 셋해준다
        animator.SetFloat("Speed", delta_position.magnitude / Time.deltaTime);

        // 공격 종료 되었고 공격중인 상태가 아니면 공격 종료 플래그 꺼준다 (다시 공격 종료되면 켜짐)
        if(attacked && !status.attacking)
        {
            attacked = false;
        }

        // 공격중 상태인지 반환 (공격 종료 안됐고 공격중인 상태이면 true)
        animator.SetBool("Attacking", (!attacked && status.attacking));


        // 아직 다운 아니고 died 상태면 다운으로 바꿔준다
        if(!isDown && status.died)
        {
            isDown = true;
            animator.SetTrigger("Down");
        }

        // 다음 델타 포지션 구해야하므로 이전 좌표를 현재 좌표로 초기화
        prePosition = transform.position;
	}
}
