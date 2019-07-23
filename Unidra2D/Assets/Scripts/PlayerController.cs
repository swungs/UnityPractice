using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    //변수 선언
    public float moveSpeed = 5f;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        // d키 입력하면 오른쪽으로 움직임
        if (Input.GetKey(KeyCode.D) == true)
        {
            transform.Translate(Vector3.right * moveSpeed * Time.deltaTime);
            Vector3 local_scale = transform.localScale;
            if (local_scale.x < 0)
            {
                local_scale.x *= -1;
                transform.localScale = local_scale;
            }
        }

        // a키 입력하면 왼쪽으로 움직임
        if (Input.GetKey(KeyCode.A) == true)
        {
            transform.Translate(Vector3.left * moveSpeed * Time.deltaTime);
            Vector3 local_scale = transform.localScale;
            if (local_scale.x > 0)
            {
                local_scale.x *= -1;
                transform.localScale = local_scale;
            }
        }
        /*
        // 스페이스키 입력하면 점프
        if (Input.GetKey(KeyCode.Space) == true)
        {
            transform.Translate(Vector3.up * moveSpeed * 2 * Time.deltaTime);
        }
        넣었더니 바닥으로 떨어짐 ㅎ.ㅠ
        */
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
