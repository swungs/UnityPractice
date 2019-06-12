using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class EnemyCtrl : MonoBehaviour
{
    CharacterStatus status;
    CharaAnimation charaAnimation;
    CharacterMove characterMove;
    Transform attackTarget;
    GameRuleCtrl gameRuleCtrl;

    public GameObject[] dropItemPrefab;

    public AudioClip deathSeClip;

    // 대기 시간 세팅
    public float waitBaseTime = 2.0f;
    // 남은 대기 시간
    float waitTime;
    // 이동 범위
    public float walkRange = 5.0f;
    // 좌표 세팅
    public Vector3 basePosition;

    // 스테이트 종류
    enum State
    {
        Walking,
        Chasing,
        Attacking,
        Died,
    };

    // 현재 스테이트
    State state = State.Walking;

    // 다음 스테이트
    State nextState = State.Walking;

    // Start is called before the first frame update
    void Start()
    {
        status = GetComponent<CharacterStatus>();
        charaAnimation = GetComponent<CharaAnimation>();
        characterMove = GetComponent<CharacterMove>();
        gameRuleCtrl = FindObjectOfType<GameRuleCtrl>();


        // 좌표 및 대기 시간 초기화
        basePosition = transform.position;
        waitTime = waitBaseTime;

    }

    // Update is called once per frame
    void Update()
    {
        switch (state)
        {
            case State.Walking:
                Walking();
                break;
            case State.Chasing:
                Chasing();
                break;
            case State.Attacking:
                Attacking();
                break;
        }

        if (state != nextState)
        {
            state = nextState;
            switch (state)
            {
                case State.Walking:
                    WalkStart();
                    break;
                case State.Chasing:
                    ChaseStart();
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

    // 스테이트를 변경한다.
    void ChangeState(State nextState)
    {
        this.nextState = nextState;
    }

    void WalkStart()
    {
        StateStartCommon();
    }

    void Walking()
    {
        // 아직 대기 시간이 남았다면
        if (waitTime > 0.0f)
        {
            // 지나간 시간을 대기 시간에서 감소시켜줌
            waitTime -= Time.deltaTime;

            // 지난 시간 감소시켰을 때 대기 시간 다 지났으면
            if (waitTime <= 0.0f)
            {
                // 이동 범위 내에서 랜덤한 위치 뽑기
                // insideUnitCircle => 거리가 1인 원 안에 있는 임의의 한점 반환
                // 반환한 값에 이동 범위 만큼을 곱하기 이동 범위를 원으로 표시해서 그 중 하나의 점을 받아오는 것과 같은 효과
                Vector2 randomValue = UnityEngine.Random.insideUnitCircle * walkRange;
                // 이전 좌표에서 랜덤 이동 거리 더해서 목표 좌표 구하기
                Vector3 destinationPosition = basePosition + new Vector3(randomValue.x, 0.0f, randomValue.y);
                // 이동 목적지 좌표 세팅
                SendMessage("SetDestination", destinationPosition);
            }
        }
        // 대기 시간 안 남았으면
        else
        {
            // 목적지 도착했으면
            if (characterMove.Arrived())
            {
                // 대기 시간 waitBaseTime~waitBaseTime의 2배 중 랜덤하게 설정해줌 (즉 2~4 초)
                waitTime = UnityEngine.Random.Range(waitBaseTime, waitBaseTime * 2.0f);
            }

            // 공격 대상이 존재한다면
            if(attackTarget)
            {   
                // 추적 상태로 전환
                ChangeState(State.Chasing);
            }
        }
    }

    void ChaseStart()
    {
        StateStartCommon();
    }

    void Chasing()
    {
        // 타겟의 좌표를 목적지로 지정하여 이동
        SendMessage("SetDestination", attackTarget.position);
        // 타겟과의 거리가 좁아지면
        if(Vector3.Distance(attackTarget.position, transform.position) <= 2.0f)
        {
            // 공격 스테이트로 전환
            ChangeState(State.Attacking);
        }
    }

    void AttackStart()
    {
        StateStartCommon();

        // 캐릭터 스테이터스의 attacking 플래그를 true로 바꿔준다. state와 헷갈리지 않게 주의!
        status.attacking = true;

        // 적이 있는 방향으로 돌아본다.
        Vector3 targetDirection = (attackTarget.position - transform.position).normalized;
        SendMessage("SetDirection", targetDirection);

        // 이동을 멈춘다.
        SendMessage("StopMove");
    }

    void Attacking()
    {
        // 공격을 마쳤으면
        if(charaAnimation.IsAttacked())
        {
            // 이동 상태로 전환
            ChangeState(State.Walking);
            // 대기 시간 초기화
            waitTime = UnityEngine.Random.Range(waitBaseTime, waitBaseTime * 2.0f);
            // 공격 대상 초기화
            attackTarget = null;

        }
    }

    void dropItem()
    {
        if(dropItemPrefab.Length ==0)
        {
            return;
        }

        GameObject dropItem = dropItemPrefab[Random.Range(0, dropItemPrefab.Length)];
        Instantiate(dropItem, transform.position, Quaternion.identity);

    }

    void Died()
    {

        status.died = true;
        dropItem();
        Destroy(gameObject);

        if(gameObject.tag =="Boss")
        {
            gameRuleCtrl.GameClear();
        }

        //적은 죽으면 오브젝트가 사라지므로 적이 있던 자리에서 오디오 클립을 재생하는 오브젝트를 만들어준다.
        //이 오브젝트는 클립 모두 재생하고 나면 자동으로 파기됨

        Vector3 cameraPos = new Vector3(Camera.main.transform.position.x, Camera.main.transform.position.y, Camera.main.transform.position.z);
        AudioSource.PlayClipAtPoint(deathSeClip, cameraPos, 1f);

    }

    void StateStartCommon()
    {
        // 스테이터스 초기화
        status.attacking = false;
        status.died = false;
    }

    // 공격 대상을 설정 함수
    public void SetAttackTarget(Transform target)
    {
        attackTarget = target;
    }

    // 대미지 받아서 처리해주는 함수
    void Damage(AttackArea.AttackInfo attackInfo)
    {
        // HP 스테이터스를 공격력만큼 감소시켜준다
        status.HP -= attackInfo.attackPower;


        // 0 이하가 되면 죽는 처리
        if (status.HP <= 0)
        {
            status.HP = 0;
            ChangeState(State.Died);
        }
    }

}
