using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BamsongiController : MonoBehaviour {

    // Shoot 메서드 정의. 받아온 방향으로 힘을 준다
    public void Shoot(Vector3 dir)
    {
        GetComponent<Rigidbody>().AddForce(dir);
    }

    private void OnCollisionEnter(Collision other)
    {
        GetComponent<Rigidbody>().isKinematic = true;
        GetComponent<ParticleSystem>().Play();
    }

    // Use this for initialization
    void Start () {
        //Shoot(new Vector3(0, 200, 2000));
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
