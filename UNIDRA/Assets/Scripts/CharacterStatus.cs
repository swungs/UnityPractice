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
    public GameObject lastAttackTarget = null;

    // gui 및 네트워크 장에서 사용한다

    // 플레이어 이름
    public string characterName = "Player";

    // animation 장에서 사용한다
    // 상태
    public bool attacking = false;
    public bool died = false;

    // 공격력 강화.
    public bool powerBoost = false;
    // 공격력 강화 시간.
    float powerBoostTime = 0.0f;

    // 아이템 획득.
    public void GetItem(DropItem.ItemKind itemKind)
    {
        switch (itemKind)
        {
            case DropItem.ItemKind.Attack:
                powerBoostTime = 5.0f;
                break;
            case DropItem.ItemKind.Heal:
                // MaxHP의 절반 회복.
                HP = Mathf.Min(HP + MaxHP / 2, MaxHP);
                break;
        }
    }

	// Update is called once per frame
	void Update () {

        powerBoost = false;
        if(powerBoostTime > 0.0f)
        {
            powerBoost = true;
            powerBoostTime = Mathf.Max(powerBoostTime - Time.deltaTime, 0.0f);
        }
		
	}
}
