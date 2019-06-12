using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCtrl : MonoBehaviour {

    const float RayCastMaxDistance = 100.0f;

    CharacterStatus status;
    CharaAnimation charaAnimation;
    Transform attackTarget;

    InputManager inputManager;
    GameRuleCtrl gameRuleCtrl;

    public float attackRange = 1.5f;

    public AudioClip deathSeClip;
    AudioSource deathSeAudio;

    // 스테이트 종류
    enum State
    {
        Walking,
        Attacking,
        Died,
    };

    // 현재 스테이트
    State state = State.Walking;

    // 다음 스테이트
    State nextState = State.Walking;

	// Use this for initialization
	void Start () {

        status = GetComponent<CharacterStatus>();
        charaAnimation = GetComponent<CharaAnimation>();
        inputManager = FindObjectOfType<InputManager>();
        gameRuleCtrl = FindObjectOfType<GameRuleCtrl>();

        //오디오 초기화
        deathSeAudio = gameObject.AddComponent<AudioSource>();
        deathSeAudio.clip = deathSeClip;
        deathSeAudio.loop = false;

    }

    // Update is called once per frame
    void Update () {

        // 현재 스테이트를 받아와서 플레이어를 스테이트에 따른 컨트롤 함수를 호출해준다
        switch(state)
        {
            case State.Walking:
                walking();
                break;
            case State.Attacking:
                Attacking();
                break;
        }

        // 다음 스테이트가 현재 스테이트와 달라졌다면
        if(state != nextState)
        {
            // 현재 스테이트를 다음 스테이트로 변경해주고 스테이트 변경 시 각각 불리는 함수를 호출해준다
            state = nextState;
            switch(state)
            {
                case State.Walking:
                    WalkStart();
                    break;
                case State.Attacking:
                    AttackStart();
                    break;
                case State.Died:
                    Died();
                    break;

            }
        }
	}

    // 스테이트 변경 함수
    void ChangeState(State nextState)
    {
        this.nextState = nextState;
    }

    // 걷기로 전환 및 걷는중 함수
    void WalkStart()
    {
        StateStartCommon();
    }

    void walking()
    {
        if (inputManager.Clicked())
        {
            Vector2 clickPos = inputManager.GetCursorPosition();

            Ray ray = Camera.main.ScreenPointToRay(clickPos);
            RaycastHit hitInfo;

            //레이케스트에 걸린 레이어가 ground거나 enemyhit(적의 충돌체에 설정되어 있는 레이어)일때
            if (Physics.Raycast(ray, out hitInfo, RayCastMaxDistance, 
                                                (1 << LayerMask.NameToLayer("Ground"))|
                                                (1 << LayerMask.NameToLayer("EnemyHit"))))
            {
                //부딪힌 것의 충돌체의 게임오브젝트의 레이어가 땅이면
                if(hitInfo.collider.gameObject.layer == LayerMask.NameToLayer("Ground"))
                {
                    SendMessage("SetDestination", hitInfo.point);
                }

                if(hitInfo.collider.gameObject.layer == LayerMask.NameToLayer("EnemyHit"))
                {
                    //히트 지점 구해오기
                    Vector3 hitPoint = hitInfo.point;

                    //y좌표는 플레이어와 동일하게 맞추기(수평으로 거리만 구하기 위해)
                    hitPoint.y = transform.position.y;

                    //플레이어 - 히트 지점 좌표 사이 거리 구하기
                    float distance = Vector3.Distance(hitPoint, transform.position);

                    // 공격 범위 안이면
                    if(distance < attackRange)
                    {
                        // 충돌체 가지고 있는 애를 타겟으로 설정해주고 공격중 스테이트로 전환
                        attackTarget = hitInfo.collider.transform;
                        ChangeState(State.Attacking);
                    }
                    // 공격 범위 안이 아니면 그냥 이동
                    else
                    {
                        SendMessage("SetDestination", hitInfo.point);
                    }
                }


            }
        }
    }

    private void AttackStart()
    {
        StateStartCommon();

        status.attacking = true;

        // 타겟을 향한 방향 구하기
        Vector3 targetDirection = (attackTarget.position - transform.position).normalized;
        //charactermove.cs의 setdirection 함수 호출. 강제로 방향 바꿔준다
        SendMessage("SetDirection", targetDirection);
        //charactermove.cs의 stopmove 함수 호출. 멈춰준다.
        SendMessage("StopMove");
    }

    void Attacking()
    {
        //charaanimation에서 endattack 함수에서 attacked가 true일 때 true
        if(charaAnimation.IsAttacked())
        {
            //공격 끝났으면 walking state로 바꿔준다
            ChangeState(State.Walking);
        }
    }

    private void Died()
    {
        status.died = true;
        gameRuleCtrl.GameOver();
        deathSeAudio.Play();

    }

    // 대미지 받아서 처리해주는 함수
    void Damage(AttackArea.AttackInfo attackInfo)
    {
        // HP 스테이터스를 공격력만큼 감소시켜준다
        status.HP -= attackInfo.attackPower;

        // 0 이하가 되면 죽는 처리
        if(status.HP <= 0)
        {
            status.HP = 0;
            ChangeState(State.Died);
        }
    }

    private void StateStartCommon()
    {
        // 스테이터스 초기화
        status.attacking = false;
        status.died = false;
    }
}
