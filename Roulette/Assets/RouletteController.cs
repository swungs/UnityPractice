using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RouletteController : MonoBehaviour {

    float rotSpeed = 0;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {

        //클릭하면 회전 속도를 설정한다.
        if (Input.GetMouseButtonDown(0))
        {
            this.rotSpeed = 20;
        }

        //회전 속도만큼 룰렛을 회전시킨다.
        transform.Rotate(0, 0, rotSpeed);

        //룰렛을 감속시킨다.
        this.rotSpeed *= 0.98f;

	}
}
