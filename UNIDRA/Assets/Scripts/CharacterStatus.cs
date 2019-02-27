using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterStatus : MonoBehaviour {

    // 공격 장에서 사용한다
    
    // 체력
    public int HP = 100;
    public int MaxHP = 100;

    // 공격
    public int Power = 10;
    
    // 마지막 공격 대상
    public GameObject lastAttackTarge = null;

    // gui 및 네트워크 장에서 사용한다

    // 플레이어 이름
    public string characterName = "Player";

    // animation 장에서 사용한다
    // 상태
    public bool attacking = false;
    public bool died = false;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
