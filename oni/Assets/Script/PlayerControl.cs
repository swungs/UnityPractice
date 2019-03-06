using UnityEngine;
using System.Collections;

// 메모
//
// 회전하지 않으려면
// rigidbody -> constraint -> freeze rotation
// 을 점검한다.
//
// 프리팹 복사
// × Ctrl-C Ctrl-V
// ○ Ctrl-D
//
// 적의 콜리전을 종합한다.
//
// 지속적으로 반복되는 배경 처리 방법
//
// GameObject에서 스크립트 변수나 메서드를 사용할 때에는
// GetComponent<클래스명>()을 사용한다.
//
// 불필요한 인스턴스가 제대로 삭제되었는지
// 게임을 일시정지해 Hierarchy 뷰를 보면 확인하기 쉽다.
//
// 생성된 인스턴스를 GameObject 형태로 받을 때에는
// as GameObject 로 처리한다.
//
// 인스턴스를 삭제할 때는 Destory(this) 가 아닌、
// Destory(this.gameObject)로 한다.
//
// OnBecameVisible/Invisible() 을 불러올 수 없다.
// MeshRender 가 무효 상태（Inspector 에서 체크박스가 표시되어 있지 않다.）
// 라면 불러올 수 없다.
//
// On*() 을 불러올 수 없다.
// 메서드 이름이 일치하더라도, 인수 형태가 다르면 불러올 수 없다.
// × void OnCollisionEnter(Collider other)
// ○ void OnCollisionEnter(Collision other)
//.

public class PlayerControl : MonoBehaviour {

	public Animator		animator;

	// -------------------------------------------------------------------------------- //

	// 사운드
	public AudioClip[]	AttackSound;				// 공격할 때의 소리
	public AudioClip	SwordSound;					// 칼을 휘두르는 소리
	public AudioClip	SwordHitSound;				// 충돌 효과음(검이 도깨비에 닿았을 때의 소리)
	public AudioClip	MissSound;					// 실패 시의 소리.
	public AudioClip	runSound;
	
	public AudioSource	attack_voice_audio;			// 공격음.
	public AudioSource	sword_audio;				// 칼 소리(휘두르는 소리, 충돌음)
	public AudioSource	miss_audio;					// 실패 시의 소리.
	public AudioSource	run_audio;
	
	public int			attack_sound_index = 0;		// 다음에 울리는 AttakSound.

	// -------------------------------------------------------------------------------- //

	// 이동 속도.
	public	float	run_speed = 5.0f;

	// 이동 속도의 최대값 [m/sec].
	public const float	RUN_SPEED_MAX = 20.0f;

	// 이동 속도의 가속치 [m/sec^2].
	protected const float	run_speed_add = 5.0f;

	// 이동 속도의 감속치 [m/sec^2].
	protected const float	run_speed_sub = 5.0f*4.0f;

	// 실패 연출시 중력의 크기 [m/sec^2].
	protected const float	MISS_GRAVITY = 9.8f*2.0f;

	// 공격 판정용 collider 
	protected	AttackColliderControl	attack_collider = null;

	public SceneControl				scene_control = null;

	// 공격 판정 실행중 타이머.
	// attack_timer > 0.0f 이라면 공격 중.
	protected float	attack_timer = 0.0f;

	// 헛스윙 후 공격할 수 없는 동안의 타이머.
	// attack_disable_timer > 0.0f 이라면 공격할 수 없다.
	protected float	attack_disable_timer = 0.0f;

	// 공격 판정이 지속되는 시간 [sec].
	protected const float	ATTACK_TIME = 0.3f;

	// 공격 판정이 지속되는 시간 [sec].
	protected const float	ATTACK_DISABLE_TIME = 1.0f;

	protected bool	is_running       = true;
	protected bool	is_contact_floor = false;
	protected bool	is_playable		 = true;
	
	// 정지 목표 위치.
	// （SceneControl.cs 가 결정하는, 정지하고자 하는 위치）.
	public float	stop_position = -1.0f;

	// 공격 모션의 종류.
	public enum ATTACK_MOTION {

		NONE = -1,

		RIGHT = 0,
		LEFT,

		NUM,
	};

	public ATTACK_MOTION	attack_motion = ATTACK_MOTION.LEFT;

	// 검을 휘두르는 효과
	public AnimatedTextureExtendedUV	kiseki_left = null;
	public AnimatedTextureExtendedUV	kiseki_right = null;

	// 충돌 효과.
	public ParticleSystem				fx_hit = null;
	
	// 달릴 때의 효과
	public ParticleSystem				fx_run = null;

	// 
	public	float	min_rate = 0.0f;
	public	float	max_rate = 3.0f;
	
	// -------------------------------------------------------------------------------- //

	public enum STEP {

		NONE = -1,

		RUN = 0,		// 달린다    게임 중
		STOP,			// 정지한다  목표점 연출 시
		MISS,			// 실패      도깨비와 부딪혔을 때
		NUM,
	};

	public STEP		step			= STEP.NONE;
	public STEP		next_step    	= STEP.NONE;

	// ================================================================ //

	void	Start()
	{
		this.animator = this.GetComponentInChildren<Animator>();

		// 공격 판정용 콜라이더를 찾아둔다.
		this.attack_collider = GameObject.FindGameObjectWithTag("AttackCollider").GetComponent<AttackColliderControl>();

		// 공격 판정용 콜라이더에 플레이어의 인스턴스를 설정해둔다.
		this.attack_collider.player = this;

		// 검을 휘두르는 효과

		this.kiseki_left = GameObject.FindGameObjectWithTag("FX_Kiseki_L").GetComponent<AnimatedTextureExtendedUV>();
		this.kiseki_left.stopPlay();

		this.kiseki_right = GameObject.FindGameObjectWithTag("FX_Kiseki_R").GetComponent<AnimatedTextureExtendedUV>();
		this.kiseki_right.stopPlay();

		// 충돌 효과

		this.fx_hit = GameObject.FindGameObjectWithTag("FX_Hit").GetComponent<ParticleSystem>();
		
		this.fx_run = GameObject.FindGameObjectWithTag("FX_Run").GetComponent<ParticleSystem>();
		//

		this.run_speed = 0.0f;

		this.next_step = STEP.RUN;

		this.attack_voice_audio = this.gameObject.AddComponent<AudioSource>();
		this.sword_audio        = this.gameObject.AddComponent<AudioSource>();
		this.miss_audio         = this.gameObject.AddComponent<AudioSource>();
		
		this.run_audio         	= this.gameObject.AddComponent<AudioSource>();
		this.run_audio.clip		= this.runSound;
		this.run_audio.loop		= true;
		this.run_audio.Play();
	}

	void	Update()
	{
#if false
		if(Input.GetKey(KeyCode.Keypad1)) {
			min_rate -= 0.1f;
		}
		if(Input.GetKey(KeyCode.Keypad2)) {
			min_rate += 0.1f;
		}
		if(Input.GetKey(KeyCode.Keypad4)) {
			max_rate -= 0.1f;
		}
		if(Input.GetKey(KeyCode.Keypad5)) {
			max_rate += 0.1f;
		}
#endif
		min_rate = Mathf.Clamp( min_rate, 0.0f, max_rate );
		max_rate = Mathf.Clamp( max_rate, min_rate, 5.0f );
		
		// 다음 상태로 진행할지 점검한다.
		if(this.next_step == STEP.NONE) {

			switch(this.step) {
	
				case STEP.RUN:
				{
					if(!this.is_running) {
	
						if(this.run_speed <= 0.0f) {
						
							// 달릴 때의 소리와 효과를 정지한다.
							this.fx_run.Stop();
						
							this.next_step = STEP.STOP;
						}
					}
				}
				break;

				case STEP.MISS:
				{
					if(this.is_contact_floor) {
					
						// 달릴 때 효과를 재시작한다.
						this.fx_run.Play();
					
						this.GetComponent<Rigidbody>().useGravity = true;
						this.next_step = STEP.RUN;
					}
				}
				break;
			}
		}
			
		// 상태 변화 후 초기화.
		if(this.next_step != STEP.NONE) {

			switch(this.next_step) {

				case STEP.STOP:
				{
					this.animator.SetTrigger("stop");
				}
				break;

				case STEP.MISS:
				{
					// 대각선으로 되돌아온다.

					Vector3	velocity = this.GetComponent<Rigidbody>().velocity;

					float	jump_height = 1.0f;

					velocity.x = -2.5f;
					velocity.y = Mathf.Sqrt(MISS_GRAVITY*jump_height);
					velocity.z = 0.0f;

					this.GetComponent<Rigidbody>().velocity = velocity;
					this.GetComponent<Rigidbody>().useGravity = false;

					this.run_speed = 0.0f;

					this.animator.SetTrigger("yarare");

					//

					this.miss_audio.PlayOneShot(this.MissSound);
				
					// 달릴 때의 효과를 정지한다.
					this.fx_run.Stop();
				}
				break;
			}

			this.step = this.next_step;

			this.next_step = STEP.NONE;
		}
		
		// 달릴 때의 소리음을 제어한다.
		if(this.is_running){

			this.run_audio.volume = 1.0f;

		} else {

			this.run_audio.volume = Mathf.Max(0.0f, this.run_audio.volume - 0.05f );
		}
		
		// 각 상태 실행.

		// ---------------------------------------------------- //
		// 포지션.

		switch(this.step) {

			case STEP.RUN:
			{
				// ---------------------------------------------------- //
				// 속도.
		
				if(this.is_running) {
		
					this.run_speed += PlayerControl.run_speed_add*Time.deltaTime;
		
				} else {
		
					this.run_speed -= PlayerControl.run_speed_sub*Time.deltaTime;
				}
		
				this.run_speed = Mathf.Clamp(this.run_speed, 0.0f, PlayerControl.RUN_SPEED_MAX);
		
				Vector3	new_velocity = this.GetComponent<Rigidbody>().velocity;
		
				new_velocity.x = run_speed;
		
				if(new_velocity.y > 0.0f) {
		
					new_velocity.y = 0.0f;
				}
		
				this.GetComponent<Rigidbody>().velocity = new_velocity;
		
				float	rate;
			
				rate	= this.run_speed/PlayerControl.RUN_SPEED_MAX;
				this.run_audio.pitch	= Mathf.Lerp( min_rate, max_rate, rate);

				// ---------------------------------------------------- //
				// 공격.
		
				this.attack_control();

				this.sword_fx_control();

				// ---------------------------------------------------- //
				// 공격 가능 여부를 색깔로 알 수 있다(디버그용).
		
				if(this.attack_disable_timer > 0.0f) {
		
					this.GetComponent<Renderer>().material.color = Color.gray;
		
				} else {
		
					this.GetComponent<Renderer>().material.color = Color.Lerp(Color.white, Color.blue, 0.5f);
				}
		
				// ---------------------------------------------------- //
				// "W" 키로 앞쪽으로 멀리 이동(디버그용)
#if UNITY_EDITOR
				if(Input.GetKeyDown(KeyCode.W)) {
		
					Vector3		position = this.transform.position;
		
					position.x += 100.0f*FloorControl.WIDTH*FloorControl.MODEL_NUM;
	
					this.transform.position = position;
				}
#endif
			}
			break;

			case STEP.MISS:
			{
				this.GetComponent<Rigidbody>().velocity += Vector3.down*MISS_GRAVITY*Time.deltaTime;
			}
			break;

		}

		//

		this.is_contact_floor = false;
	}


	void OnCollisionStay(Collision other)
	{
		// 도깨비와 충돌하면 감속한다.
		//

		if(other.gameObject.tag == "OniGroup") {

			do {

				// 공격 판정 실행 중에 실패 처리를 하지 않는다.
				if(this.attack_timer > 0.0f) {

					break;
				}

				if(this.step == STEP.MISS) {

					break;
				}

				//

				this.next_step = STEP.MISS;

				// 플레이어가 도깨비와 부딪혔을 때의 처리.

				this.scene_control.OnPlayerMissed();

				// 플레이어와 부딪힌 것을 도깨비 그룹에게 기억시켜둔다.

				OniGroupControl	oni_group = other.gameObject.GetComponent<OniGroupControl>();
				
				oni_group.onPlayerHitted();

			} while(false);

		}

		// 착지했는가?
		if(other.gameObject.tag == "Floor") {

			if(other.relativeVelocity.y >= Physics.gravity.y*Time.deltaTime) {

				this.is_contact_floor = true;
			}
		}
	}

	// CollisionStay 를 불러올 수 없는 경우를 대비해 아래와 같이 설정한다.
	void OnCollisionEnter(Collision other)
	{
		this.OnCollisionStay(other);
	}

	// -------------------------------------------------------------------------------- //

	// 공격 충돌 효과를 재생한다.
	public void		playHitEffect(Vector3 position)
	{
		this.fx_hit.transform.position = position;
		this.fx_hit.Play();
	}

	// 공격 충돌음을 재생한다.
	public void		playHitSound()
	{
		this.sword_audio.PlayOneShot(this.SwordHitSound);
	}

	// 『공격할 수 없는 동안』타이머를 재설정한다.(바로 공격할 수 있도록)
	public void 	resetAttackDisableTimer()
	{
		this.attack_disable_timer = 0.0f;
	}

	// 공격을 시작한 후의 (마우스 버튼을 클릭한 후)경과 시간을 구한다.
	public float	GetAttackTimer()
	{
		return(PlayerControl.ATTACK_TIME - this.attack_timer);
	}

	// 플레이어의 속도율（0.0f ～ 1.0f）을 구한다.
	public float	GetSpeedRate()
	{
		float	player_speed_rate = Mathf.InverseLerp(0.0f, PlayerControl.RUN_SPEED_MAX, this.GetComponent<Rigidbody>().velocity.magnitude);

		return(player_speed_rate);
	}

	// 정지 요청
	public void 	StopRequest()
	{
		this.is_running = false;
	}
	
	// 플레이어 조작 가능
	public void		Playable()
	{
		this.is_playable = true;
	}
	
	// 플레이어 조작 금지
	public void		UnPlayable()
	{
		this.is_playable = false;
	}
	
	// 플레이어가 정지했는가?
	public bool 	IsStopped()
	{
		bool	is_stopped = false;

		do {

			if(this.is_running) {

				break;
			}
			if(this.run_speed > 0.0f) {

				break;
			}

			//

			is_stopped = true;

		} while(false);

		return(is_stopped);
	}

	// 감속한 경우의 예상 정지 위치를 구한다.
	public float CalcDistanceToStop()
	{
		float distance = this.GetComponent<Rigidbody>().velocity.sqrMagnitude/(2.0f*PlayerControl.run_speed_sub);

		return(distance);
	}

	// -------------------------------------------------------------------------------- //

	// 공격 입력이 있었나?
	private bool	is_attack_input()
	{
		bool	is_attacking = false;

		// 마우스 왼쪽 버튼을 클릭하면 공격.
		//
		// OnMouseDown() 의 경우 자체를 클릭해야만 불러올 수 있다.
		// 이번에는 화면 어디를 클릭하더라도 반응할 수 있도록
		// Input.GetMouseButtonDown() 을 사용한다.
		//
		if(Input.GetMouseButtonDown(0)) {

			is_attacking = true;
		}

		// 디버그용 자동 공격.
		if(SceneControl.IS_AUTO_ATTACK) {

			GameObject[] oni_groups = GameObject.FindGameObjectsWithTag("OniGroup");

			foreach(GameObject oni_group in oni_groups) {

				float	distance = oni_group.transform.position.x - this.transform.position.x;
				
				distance -= 1.0f/2.0f;
				distance -= OniGroupControl.collision_size/2.0f;

				// 뒤쪽에 있는 것은 무시한다.
				// （실제로 게임에서는 있을 수 없지만, 가정하에）.
				//
				if(distance < 0.0f) {

					continue;
				}

				// 충돌까지 예상 시간.

				float	time_left = distance/(this.GetComponent<Rigidbody>().velocity.x - oni_group.GetComponent<OniGroupControl>().run_speed);

				// 멀리 있는 것은 무시한다.
				//
				if(time_left < 0.0f) {

					continue;
				}

				if(time_left < 0.1f) {

					is_attacking = true;
				}
			}
		}

		return(is_attacking);
	}

	// 공격 제어.
	private void	attack_control()
	{
		if(!this.is_playable) {
			return;	
		}
		
		if(this.attack_timer > 0.0f) {

			// 공격 판정 실행 중.

			this.attack_timer -= Time.deltaTime;

			// 공격 판정 종료 점검.
			if(this.attack_timer <= 0.0f) {

				// 콜라이더（공격의 타격 판정）의 타격 판정을 무효로 한다.
				//
				attack_collider.SetPowered(false);
			}

		} else {

			this.attack_disable_timer -= Time.deltaTime;

			if(this.attack_disable_timer > 0.0f) {

				// 아직 공격할 수 없다.

			} else {

				this.attack_disable_timer = 0.0f;

				if(this.is_attack_input()) {

					// 콜라이더（공격의 타격 판정）의 타격 판정을 유효로 한다.
					//
					attack_collider.SetPowered(true);
		
					this.attack_timer         = PlayerControl.ATTACK_TIME;
					this.attack_disable_timer = PlayerControl.ATTACK_DISABLE_TIME;

					// 공격 모션을 재생한다.

					// 다음에 재생할 모션을 선택한다.
					//
					// 『도깨비』가 날아가는 방향을 정할 때『이전의 공격 모션』을 알아야 하기 때문에
					//  재생 후가 아닌 재생 전에 모션을 선택한다.
					//
					switch(this.attack_motion) {

						default:
						case ATTACK_MOTION.RIGHT:	this.attack_motion = ATTACK_MOTION.LEFT;	break;
						case ATTACK_MOTION.LEFT:	this.attack_motion = ATTACK_MOTION.RIGHT;	break;
					}


					switch(this.attack_motion) {

						default:
						case ATTACK_MOTION.RIGHT:	this.animator.SetTrigger("attack_r");	break;
						case ATTACK_MOTION.LEFT:	this.animator.SetTrigger("attack_l");	break;
					}

					this.attack_voice_audio.PlayOneShot(this.AttackSound[this.attack_sound_index]);

					this.attack_sound_index = (this.attack_sound_index + 1)%this.AttackSound.Length;

					this.sword_audio.PlayOneShot(this.SwordSound);

				}
			}
		}
	}

	// 검을 휘두르는 효과.
	private	void	sword_fx_control()
	{

		do {
		
			if(this.attack_timer <= 0.0f) {
				break;
			}
	
			Animator	animator = this.GetComponentInChildren<Animator>();

			AnimatorStateInfo	state_info = animator.GetCurrentAnimatorStateInfo(0);
			AnimatorClipInfo	clip_info  = animator.GetCurrentAnimatorClipInfo(0)[0];
			AnimationClip		clip       = clip_info.clip;

			AnimatedTextureExtendedUV	anim_player;
		
			switch(this.attack_motion) {
		
				default:
				case ATTACK_MOTION.RIGHT:
				{
					anim_player = this.kiseki_right;
				}
				break;
		
				case ATTACK_MOTION.LEFT:
				{
					anim_player = this.kiseki_left;
				}
				break;
			}
		
			float	start_frame   = 2.5f;
			float	start_time    = start_frame/clip.frameRate;
			float	current_time  = state_info.normalizedTime*state_info.length;
			
			if(current_time < start_time) {
				break;
			}
		
			anim_player.startPlay(current_time - start_time);
		
		} while(false);
	}
}
