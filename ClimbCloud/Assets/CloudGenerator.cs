using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloudGenerator : MonoBehaviour {

    GameObject player;

    // 프리팹 선언
    public GameObject cloudPrefab;

    // 구름 리스트 만들기
    List<GameObject> CloudList = new List<GameObject>();
    int bottomNum = 0;

    // 첫 구름 좌표
    float cloudX = 0;
    float cloudY = -0.7f;

	// Use this for initialization
	void Start () {

        GameObject cloud;

        // 구름 10개 생성
        for (int i = 0; i < 10; i++)
        {
            //인스턴스 생성
            cloud = Instantiate(cloudPrefab) as GameObject;

            //구름 리스트에 추가해줌
            CloudList.Add(cloud);

            //다음 구름 랜덤으로 만들기
            MakeNextCloud(cloud);
        }
    }

    // Update is called once per frame
    void Update()
    {

        // 플레이어와 y좌표 받아오기
        this.player = GameObject.Find("cat");
        float playerY = player.transform.position.y;

        // 제일 아래 있는 구름 y좌표 받아오기
        float bottomCloudY = CloudList[bottomNum].transform.position.y;

        // 맨 아래 오브젝트가 플레이어 기준 너무 밑에 위치하게 되면 맨 위로 좌표 옮겨준다.
        if (bottomCloudY + 10 < playerY)
        {
            // 다음 구름 랜덤으로 만들기
            MakeNextCloud(CloudList[bottomNum]);

            // 마지막 리스트까지 돌았으면 다시 처음으로 돌아가자
            if (bottomNum < 9)
            {
                bottomNum++;
            }
            else
            {
                bottomNum = 0;
            }
        }

        // 메인 화면에 충돌 박스 추가해야징
        // 플레이어 컨트롤러에 최대 고도 UI 기록
        // 효과음 넣기

    }

    void MakeNextCloud(GameObject cloud)
    {
        // 좌표 이동
        cloud.transform.position = new Vector3(cloudX, cloudY, 0);

        // 다음 구름 좌표 랜덤 지정 - 너무 멀때도 있고 너무 가까울 때도 있고..
        cloudX = Random.Range(-2.5f, 2.5f);
        cloudY = cloudY + Random.Range(2, 4);

        // 구름 가로 사이즈 랜덤 조정
        cloud.transform.localScale = new Vector3(Random.Range(0.5f,1.5f), 1, 1);

    }
}
