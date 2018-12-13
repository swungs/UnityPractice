using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameDirector : MonoBehaviour {

    GameObject hpGauge;

	// Use this for initialization
	void Start () {

        this.hpGauge = GameObject.Find("hpGauge");
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void DecreaseHP()
    {
        this.hpGauge.GetComponent<Image>().fillAmount -= 0.1f;
    }
}
