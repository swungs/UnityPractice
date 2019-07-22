using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{

    public Transform areaTopLeft;
    public Transform areaBottomRight;
    public GameObject firePrefab;
    public float minTime = 1f;
    public float maxTime = 2f;

    IEnumerator Attack()
    {
        // 현재 위치를 from_position에, areaTopLeft~areaBottomRight 사이의 랜덤한 좌표를 to_position에 저장
        Vector3 from_position = transform.position;
        Vector3 to_position = new Vector3(
            Random.Range(areaTopLeft.position.x, areaBottomRight.position.x),
            Random.Range(areaTopLeft.position.y, areaBottomRight.position.y),
            transform.position.z);

        // 현재 시간을 start_time에, minTime~maxTime 사이의 랜덤한 시간을 move_time에 저장
        float start_time = Time.time;
        float move_time = Random.Range(minTime, maxTime);

        // 이동 처리
        while (true)
        {
            float t = (Time.time - start_time) / move_time;
            transform.position = Vector3.Lerp(from_position, to_position, t);
            if (t >= 1f)
                break;
            yield return null;
        }

        // 불덩이 소환
        Instantiate(firePrefab, transform.position, Quaternion.identity);

        StartCoroutine(Attack());
    }


    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(Attack());
    }
}

  