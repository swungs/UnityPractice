using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarController : MonoBehaviour {

    float speed = 0;
    float swipeLength = 0;
    Vector2 startPos;
    Vector2 endPos;

	// Use this for initialization
	void Start () {
		
	}

    // Update is called once per frame
    void Update() {

        if (Input.GetMouseButtonDown(0))
        {
            this.startPos = Input.mousePosition;
        }
        else if (Input.GetMouseButtonUp(0))
        {
            this.endPos = Input.mousePosition;

            swipeLength = (this.endPos.x - this.startPos.x);
            this.speed = swipeLength / 500.0f;

            GetComponent<AudioSource>().Play();
        }

        transform.Translate(this.speed, 0, 0);
        this.speed *= 0.98f;
	}
}
