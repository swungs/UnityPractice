using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowCamera : MonoBehaviour {

    public float distance = 5.0f;
    public float horizontalAngle = 0.0f;
    public float rotAngle = 180.0f; // 화면 가로폭만큼 커서를 이동시켰을 때 몇 도 회전하는가
    public float verticalAngle = 10.0f;

    public Transform lookTarget; // 주시
    public Vector3 offset = Vector3.zero; // 주시 대상 좌표 땅에서 얼굴 쪽으로 보정해줄 y값

    InputManager inputManager;

    // Use this for initialization
    void Start () {

        inputManager = FindObjectOfType<InputManager>();
	}
	
	// Update is called once per frame
	void Update () {
		
        // 슬라이드 한 경우 회전시켜준다
        if(inputManager.Moved())
        {
            float anglePerPixel = rotAngle / (float)Screen.width;
            Vector2 delta = inputManager.GetDeltaPosition(); // 프레임당 이동량 받아오는 함수

            // x 좌표 이동량만큼 더해준 후 360이 넘으면 0~360 사이 값으로 보정
            horizontalAngle += delta.x * anglePerPixel;
            horizontalAngle = Mathf.Repeat(horizontalAngle, 360.0f);

            // y 좌표 이동량만큼 빼준다. -60~60 사이 값으로 보정, 아래로 긁으면 카메라는 위를 올려다봐야 하니까 빼준다.
            verticalAngle -= delta.y * anglePerPixel;
            verticalAngle = Mathf.Clamp(verticalAngle, -60.0f, 60.0f);
        }

        if(lookTarget !=null)
        {
            Vector3 lookPosition = lookTarget.position + offset; // offset값은 어디서 입력하지?
            
            // 원점 기준 distance 만큼 떨어진 거리에서 회전한 카메라 좌표 구하기
            Vector3 relativePos = Quaternion.Euler(verticalAngle, horizontalAngle, 0) * new Vector3(0, 0, -distance);

            // 카메라 좌표를 주시 대상 좌표 기준 상대 좌표로 실제 이동 시키기
            transform.position = lookPosition + relativePos;

            // 카메라가 주시 대상을 바라보게 한다
            transform.LookAt(lookPosition);

            RaycastHit hitInfo;
            // 라인 캐스트는 시작점과 끝점을 특정할 수 있을 때 사용하는 레이캐스트
            // ground 레이어와 충돌이 있다면
            if(Physics.Linecast(lookPosition, transform.position, out hitInfo, 1 << LayerMask.NameToLayer("Ground")))
            {
                // 카메라 위치를 충돌 지점으로 옮겨준다
                transform.position = hitInfo.point;
                //transform.Translate(transform.position.x, transform.position.y, transform.position.z);
            }
        }
	}
}
