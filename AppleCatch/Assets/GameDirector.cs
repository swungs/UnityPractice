using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameDirector : MonoBehaviour {

    GameObject timerText;
    GameObject pointText;
    float time = 60.0f;
    int point = 0;

    GameObject generator;

    public void GetApple()
    {
        this.point += 100;
    }
    public void GetBomb()
    {
        this.point /= 2;
    }

    // Use this for initialization
    void Start () {
        this.timerText = GameObject.Find("Time");
        this.pointText = GameObject.Find("Point");
        this.generator = GameObject.Find("ItemGenerator");

	}
	
	// Update is called once per frame
	void Update () {

        this.time -= Time.deltaTime;
        if(this.time <0)
        {
            this.time = 0;
            this.generator.GetComponent<ItemGenerator>().SetParameter(10000.0f, 0f, 0);
        }
        else if(0 <= this.time && this.time <5)
        {
            this.generator.GetComponent<ItemGenerator>().SetParameter(10000.0f, 0f, 0);
        }
        else if (5 <= this.time && this.time < 10)
        {
            this.generator.GetComponent<ItemGenerator>().SetParameter(10000.0f, 0f, 0);
        }
        else if (10 <= this.time && this.time < 20)
        {
            this.generator.GetComponent<ItemGenerator>().SetParameter(1.0f, -0.06f, 5);
        }
        else if (20 <= this.time && this.time < 30)
        {
            this.generator.GetComponent<ItemGenerator>().SetParameter(5.0f, -0.03f, 2);
        }
        this.timerText.GetComponent<Text>().text = this.time.ToString("F1");
        this.pointText.GetComponent<Text>().text = this.point.ToString() + " Point";

    }

}
