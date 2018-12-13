using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasketController : MonoBehaviour {

    public AudioClip appleSE;
    public AudioClip bombSE;
    AudioSource aud;

    GameObject director;

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("catch!");
        if(other.gameObject.tag =="Apple")
        {
            Debug.Log("catch apple");
            this.aud.PlayOneShot(this.appleSE);
            this.director.GetComponent<GameDirector>().GetApple();

        }
        else
        {
            Debug.Log("catch bomb");
            this.aud.PlayOneShot(this.bombSE);
            this.director.GetComponent<GameDirector>().GetBomb();


        }
        Destroy(other.gameObject);
    }

    // Use this for initialization
    void Start () {
        this.aud = GetComponent<AudioSource>();
        this.director = GameObject.Find("GameDirector");
	}
	
	// Update is called once per frame
	void Update () {
		if(Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if(Physics.Raycast(ray, out hit, Mathf.Infinity))
            {
                float x = Mathf.RoundToInt(hit.point.x);
                float z = Mathf.RoundToInt(hit.point.z);
                transform.position = new Vector3(x, 0, z);

            }
        }
	}
}
