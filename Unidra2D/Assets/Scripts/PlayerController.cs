using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    bool grounded = false;      // 땅에 닿아 있는지 여부
    new Rigidbody2D rigidbody2D;


    // Start is called before the first frame update
    void Start()
    {
        rigidbody2D = gameObject.GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
    }

    // Physics engine Updates
    // 정확한 시간 단위로 업데이트된다고 하는데..? 잘 모르겠다
    void FixedUpdate()
    {
        GroundedUpdater();
        Move();
        Jump();
    }

    // 땅에 닿았는지 체크
    void GroundedUpdater()
    {
        grounded = false; //Set to false every frame by default
        RaycastHit2D[] hit;
        hit = Physics2D.RaycastAll(transform.position, Vector2.down, 0.2f);
        // you can increase RaycastLength and adjust direction for your case
        foreach (var hited in hit)
        {
            if (hited.collider.gameObject == gameObject) //Ignore my character
                continue;
            // Don't forget to add tag to your ground
            if (hited.collider.gameObject.tag == "Ground")
            { //Change it to match ground tag
                grounded = true;
            }
        }
    }

    void Move()
    {      
        // 방향키 입력하면 오른쪽으로 움직임
        if (Input.GetKey(KeyCode.RightArrow))
        {
            transform.Translate(Vector3.right * PlayerStats.Instance.MoveSpeed * Time.deltaTime);
            Vector3 local_scale = transform.localScale;
            if (local_scale.x < 0)
            {
                local_scale.x *= -1;
                transform.localScale = local_scale;
            }
        }

        // 방향키 입력하면 왼쪽으로 움직임
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            transform.Translate(Vector3.left * PlayerStats.Instance.MoveSpeed * Time.deltaTime);
            Vector3 local_scale = transform.localScale;
            if (local_scale.x > 0)
            {
                local_scale.x *= -1;
                transform.localScale = local_scale;
            }
        }

    }

    void Jump()
    {
        if (!grounded)
            return;

        // 스페이스키 입력하면 점프
        if (Input.GetKeyDown(KeyCode.Space))
        {
            // prevent velocity amplification(?)
            rigidbody2D.velocity = Vector2.zero;

            Vector2 jumpVelocity = new Vector2(0, PlayerStats.Instance.JumpPower);
            rigidbody2D.AddForce(jumpVelocity, ForceMode2D.Impulse);

            grounded = false;
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        // 부딪힌 녀석에 fire 태그가 있다면 플레이어 애니메이터 컴포넌트의 대미지 트리거를 불러준다.
        if (collision.gameObject.tag == "Fire")
        {
            Animator myAnimator = GetComponent<Animator>();
            myAnimator.SetTrigger("Damage");

            PlayerStats.Instance.TakeDamage(1);

            if (PlayerStats.Instance.Health <= 0)
            {
                // gameover
            }
        }
    }

    // hp 추가, 회복, 대미지 처리
    //PlayerStats.Instance.AddHealth();
    //PlayerStats.Instance.Heal(health);
    //PlayerStats.Instance.TakeDamage(dmg);

}
