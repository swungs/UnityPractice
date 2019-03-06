using UnityEngine;
using System.Collections;

public class AttackColliderControl : MonoBehaviour {

	public PlayerControl	player = null;

	// 공격 판정 시작
	private bool		is_powered = false;

	// -------------------------------------------------------------------------------- //

	void Start ()
	{
		this.SetPowered(false);
	}
	
	// Update is called once per frame
	void Update ()
	{
	}

	// OnTriggerEnter 이벤트는 collision일때에만 호출되는데,
	// 공격 판정의 원이 발생했을 때, 도깨비가 원 안에 있다면 
	// 공격 판정이 어렵다.
	// void OnTriggerEnter(Collider other)
	void OnTriggerStay(Collider other) 
	{
		do {

			if(!this.is_powered) {

				break;
			}

			if(other.tag != "OniGroup") {
	
				break;
			}

			OniGroupControl	oni = other.GetComponent<OniGroupControl>();

			if(oni == null) {

				break;
			}

			//

			oni.OnAttackedFromPlayer();

			// 공격할 수 없는 동안 타이머를 리셋한다.(바로 공격할 수 있도록)
			this.player.resetAttackDisableTimer();

			// 공격 효과를 재생한다.
			this.player.playHitEffect(oni.transform.position);

			// 공격 음악을 재생한다.
			this.player.playHitSound();

		} while(false);
	}

	public void	SetPowered(bool sw)
	{
		this.is_powered = sw;

		if(SceneControl.IS_DRAW_PLAYER_ATTACK_COLLISION) {

			this.GetComponent<Renderer>().enabled = sw;
		}
	}
}
