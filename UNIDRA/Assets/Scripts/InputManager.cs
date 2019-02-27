using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour {

    //input 클래스 키보드,조이스틱,마우스-기능이름 세팅을 토대로 입력 상태를 받아올 수 있음.
    //입력 장치별로 동일한 기능 이름을 사용할 수 있다는 장점이 있는듯.

    Vector2 slideStartPosition; // 왜 얘는 delta처럼 초기화 안시켜줌?
    Vector2 prevPosition;
    Vector2 delta = Vector2.zero;
    bool moved = false;

	// Use this for initialization
	void Start () {
		
	}

    // Update is called once per frame
    void Update() {

        //슬라이드 시작 지점 받아와서 이동량이 화면의 10% 이상이면 슬라이드 된 걸로 판단

        //fire1 키 누르면 일단 시작 좌표로 저장
        if (Input.GetButtonDown("Fire1"))
        {
            slideStartPosition = GetCursorPosition();
        }

        //계속 누르고 있는거면 처음 눌렀을 때 좌표랑 비교해서 슬라이드중인지 판단해보자
        if(Input.GetButton("Fire1"))
        {
            if(Vector2.Distance(slideStartPosition, GetCursorPosition()) >=(Screen.width * 0.1f))
            {
                moved = true;
            }
        }

        // 클릭이 끝났을때도 슬라이드 끝난 걸로 판정하면 안되니까 getbuttonup일때는 제외
        if(!Input.GetButtonUp("Fire1") && !Input.GetButton("Fire1"))
        {
            moved = false;
        }

        // 프레임당 슬라이드 커서 이동량 계산
        if(moved)
        {
            delta = GetCursorPosition() - prevPosition;
        }
        else
        {
            delta = Vector2.zero;
        }

        // 커서 위치 갱신
        prevPosition = GetCursorPosition();
     }

    // 클릭되었는지 여부
    public bool Clicked()
    {
        if(!moved && Input.GetButtonUp("Fire1"))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    // 슬라이드중인지 여부 
    public bool Moved()
    {
        return moved;
    }

    // 슬라이드할 때 프레임당 커서 이동량 받아오는 함수
    public Vector2 GetDeltaPosition()
    {
        return delta;
    }

    //현재 마우스가 위치한 화면 좌표 받아오는 함수
    public Vector2 GetCursorPosition()
    {
        return Input.mousePosition;
    }
}
