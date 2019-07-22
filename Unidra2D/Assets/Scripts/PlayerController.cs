using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    //변수 선언
    public float moveStartDistance = 10f;
    public float moveForce = 150f;
    public float maxSpeed = 100f;
    float targetPointX;
    bool facingRight = true;

    // Start is called before the first frame update
    void Start()
    {
        // 플레이어 월드 좌표를 스크린의 좌표로 바꿔서 일단 타겟 포지션으로 초기화
        Vector3 screen_point = Camera.main.WorldToScreenPoint(transform.position);
        targetPointX = screen_point.x;

    }

    // Update is called once per frame
    void Update()
    {
        //마우스 좌클릭이 아니면
        if(!Input.GetMouseButtonDown(0))
        {
            // 반환
            return;
        }

        // 아니면 마우스 포지션을 타겟 포지션으로 넣어준다
        targetPointX = Input.mousePosition.x;
    }

    void FixedUpdate()
    {
        // 플레이어 위치
        Vector3 screen_point = Camera.main.WorldToScreenPoint(transform.position);
        // rigidbody2d 컴포넌트 받아오기
        Rigidbody2D rigidbody2D = GetComponent<Rigidbody2D>();

        // Mathf.Abs => 절대값 구하기
        // 이동 해야 할 거리가 일정(moveStartDistance) 이하면 이동 처리 하지 않는다.
        if (Mathf.Abs(targetPointX - screen_point.x) <=moveStartDistance)
        {
            return;
        }

        // 이동할 방향으로 이동하는 힘 더해준다
        // Mathf.Sign => 음수면 -1, 0이면 0, 양수면 1 return;
        float horizontal = Mathf.Sign(targetPointX - screen_point.x);
        rigidbody2D.AddForce(Vector2.right * horizontal * moveForce);

        // 속도가 일정(maxSpeed)를 넘으면 maxSpeed로 제한
        if(Mathf.Abs(rigidbody2D.velocity.x) > maxSpeed)
        {
            rigidbody2D.velocity = new Vector2(Mathf.Sign(rigidbody2D.velocity.x) * maxSpeed, rigidbody2D.velocity.y);
        }

        // 가려는 방향과 플레이어 캐릭터가 향한 방향이 반대면
        if((horizontal > 0 && !facingRight) || (horizontal < 0 && facingRight) )
        {
            // 캐릭터 방향 플래그 반전
            facingRight = !facingRight;

            // 로컬 스케일 받아와서 반대로 바꿔주기
            Vector3 local_scale = transform.localScale;
            local_scale.x *= -1;
            transform.localScale = local_scale;
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        // 부딪힌 녀석에 fire 태그가 있다면 플레이어 애니메이터 컴포넌트의 대미지 트리거를 불러준다.
        if(collision.gameObject.tag == "Fire")
        {
            Animator myAnimator = GetComponent<Animator>();
            myAnimator.SetTrigger("Damage");
        }
    }
}
