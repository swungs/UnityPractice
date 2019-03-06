using UnityEngine;
using System.Collections;

public class OniEmitterControl : MonoBehaviour {

	public GameObject[]	oni_prefab;

	// SE.
	public AudioClip	EmitSound = null;		// 멀리서 날아오는 소리(휘웅~)
	public AudioClip	HitSound = null;		// 도께비가 도깨비 무리에 부딪히는 소리

	// 마지막에 생성된 도깨비.
	private GameObject	last_created_oni = null;

	private const float	collision_radius = 0.25f;

	// 생성할 도깨비수(나머지).
	// 실제 값은 결과에 따라 달라진다.
	public int		oni_num = 2;

	public bool		is_enable_hit_sound = true;

	// -------------------------------------------------------------------------------- //

	void Start()
	{
		this.GetComponent<AudioSource>().PlayOneShot(this.EmitSound);
	}

	void 	Update()
	{

		do {

			if(this.oni_num <= 0) {

				break;
			}

			// 마지막에 생성된 도깨비가 충분히 흩어질 때가지 기다린다.
			// (같은 위치에 겹치게 생성되면 콜리전에 의해 튕겨버리게 된다.)
			if(this.last_created_oni != null) {

				if(Vector3.Distance(this.transform.position, last_created_oni.transform.position) <= OniEmitterControl.collision_radius*2.0f) {

					break;
				}
			}

			Vector3	initial_position = this.transform.position;

			initial_position.y += Random.Range(-0.5f, 0.5f);
			initial_position.z += Random.Range(-0.5f, 0.5f);

			// 회전(랜점하게 보이도록 한다）.
			Quaternion	initial_rotation;

			initial_rotation = Quaternion.identity;
			initial_rotation *= Quaternion.AngleAxis(this.oni_num*50.0f, Vector3.forward);
			initial_rotation *= Quaternion.AngleAxis(this.oni_num*30.0f, Vector3.right);

			GameObject oni = Instantiate(this.oni_prefab[this.oni_num%2], initial_position, initial_rotation) as GameObject;	

			//

			oni.GetComponent<Rigidbody>().velocity        = Vector3.down*1.0f;
			oni.GetComponent<Rigidbody>().angularVelocity = initial_rotation*Vector3.forward*5.0f*(this.oni_num%3);

			oni.GetComponent<OniStillBodyControl>().emitter_control = this;

			//

			this.last_created_oni = oni;

			this.oni_num--;

		} while(false);

	}

	// 도깨비가 도깨비 무리에 부딪히는 소리를 재생한다.
	//
	// 짧은 간격으로 울리면 잘 들리지 않을 수 있으므로 일정 간격으로 
	// 울릴 수 있도록 조정한다.
	//
	public void	PlayHitSound()
	{
		if(this.is_enable_hit_sound) {

			bool	to_play = true;
	
			if(this.GetComponent<AudioSource>().isPlaying) {
	
				if(this.GetComponent<AudioSource>().time < this.GetComponent<AudioSource>().clip.length*0.75f) {
	
					to_play = false;
				}
			}
	
			if(to_play) {
	
				this.GetComponent<AudioSource>().clip = this.HitSound;
				this.GetComponent<AudioSource>().Play();
			}
		}
	}

}
