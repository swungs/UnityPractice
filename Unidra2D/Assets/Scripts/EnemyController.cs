using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    IEnumerator Attack()
    {
        // 위치 랜덤
        // 이동 시간 랜덤
        // 불덩이 개수 1개

        // 현재 위치를 from_position에, areaTopLeft~areaBottomRight 사이의 랜덤한 좌표를 to_position에 저장
        Vector3 from_position = transform.position;
        Vector3 to_position = new Vector3(
            Random.Range(areaTopLeft.position.x, areaBottomRight.position.x),
            Random.Range(areaTopLeft.position.y, areaBottomRight.position.y),
            transform.position.z);

        // 현재 시간을 start_time에, minTime~maxTime 사이의 랜덤한 시간을 move_time에 저장
        float start_time = Time.time;
        float move_time = Random.Range(minTime, maxTime);

        if (i <= fireNum)
        {
            minTime = 0.1f;
            maxTime = 0.2f;
            to_position = new Vector3(
            Random.Range(transform.position.x - 1f, transform.position.x + 1f),
            Random.Range(transform.position.y - 1f, transform.position.y + 1f),
            transform.position.z);
        }
        else
        {
            fireNum = Random.Range(1, 5);
            i = 1;
        }

        // 이동 처리
        while (true)
        {
            float t = (Time.time - start_time) / move_time;
            transform.position = Vector3.Lerp(from_position, to_position, t);
            if (t >= 1f)
                break;
            yield return null;
        }

        Instantiate(firePrefab, transform.position, Quaternion.identity);

        // 이번 공격에서 생성될 불덩이 랜덤 1~4 결정
        i++;

        StartCoroutine(Attack());
    }

    void Start()
    {
        StartCoroutine(Attack());
    }

    // Start is called before the first frame update
    void Awake()
    {
        public Transform areaTopLeft;
        public Transform areaBottomRight;
        public GameObject firePrefab;
        public float minTime = 2f;
        public float maxTime = 5f;
        int i = 1;
        int fireNum = Random.Range(4, 5);
    }

}
  