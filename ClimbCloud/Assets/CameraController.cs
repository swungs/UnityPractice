using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {

    GameObject player;
    GameObject background;


    // Use this for initialization
    void Start () {
        this.player = GameObject.Find("cat");
        this.background = GameObject.Find("background");

    }
	
	// Update is called once per frame
	void Update () {

        Vector3 playerPos = this.player.transform.position;
        transform.position = new Vector3(transform.position.x, playerPos.y, transform.position.z);

        Vector3 backgroundPos = this.background.transform.position;
        background.transform.position = new Vector3(backgroundPos.x, playerPos.y, backgroundPos.z);
    }
}
