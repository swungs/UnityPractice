using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundController : MonoBehaviour {

    GameObject player;
    GameObject background;
    GameObject background2;

    // Use this for initialization
    void Start () {
        this.player = GameObject.Find("cat");
        this.background = GameObject.Find("background");
        this.background2 = GameObject.Find("background2");

    }

    // Update is called once per frame
    void Update () {

        Vector3 playerPos = this.player.transform.position;
        transform.position = new Vector3(transform.position.x, playerPos.y, transform.position.z);

    }
}
