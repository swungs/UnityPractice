using UnityEngine;
using System.Collections;

// 도깨비 출현 제어.
public class LevelControl {

	// -------------------------------------------------------------------------------- //
	// 프리팹.

	public GameObject	OniGroupPrefab = null;

	// -------------------------------------------------------------------------------- //

	public SceneControl		scene_control = null;
	public PlayerControl	player = null;

	// 도깨비 발생 위치
	// 플레이어의 X좌표가 기준선을 넘으면 플레이어의 앞쪽에 
	// 도깨비가 출현한다.
	private float		oni_generate_line;

	// 플레이어의 appear_margin 앞쪽 위치에 도깨비가 출현한다.
	private float		appear_margin = 15.0f;

	// １그룹 도깨비의 수（＝한 번에 등장하는 도깨비 수）.
	private int			oni_appear_num = 1;

	// 연속 성공 횟수.
	private int			no_miss_count = 0;

	// 도깨비 타입.
	public enum GROUP_TYPE {

		NONE = -1,

		SLOW = 0,			// 느리다.
		DECELERATE,			// 이동 중 감속.
		PASSING,			// 두 개의 그룹으로 추적.
		RAPID,				// 짧은 간격으로.

		NORMAL,				// 보통.

		NUM,
	};

	public GROUP_TYPE		group_type      = GROUP_TYPE.NORMAL;
	public GROUP_TYPE		group_type_next = GROUP_TYPE.NORMAL;

	private	bool	can_dispatch = false;

	// 난수 변수 (보통 게임)
	public	bool	is_random = true;

	// 다음 도깨비 그룹의 출현 위치（일반적으로 플레이어 위치에서 오프셋）.
	private float			next_line = 50.0f;

	// 다음 도깨비 그룹의 속도(보통 게임).
	private	float			next_speed = OniGroupControl.SPEED_MIN*5.0f;

	// 나머지 도깨비의 출현 횟수.
	private int				normal_count = 5;

	// 나머지 이벤트의 실행 횟수.
	private int				event_count = 1;

	// 실행 중인 이벤트.
	private GROUP_TYPE		event_type = GROUP_TYPE.NONE;
	
	// -------------------------------------------------------------------------------- //

	public const float	INTERVAL_MIN = 20.0f;			// 도깨비가 출현하는 간격의 최소값.
	public const float	INTERVAL_MAX = 50.0f;			// 도깨비가 출현하는 간격의 최대값.

	// -------------------------------------------------------------------------------- //

	public void	create()
	{
		// 게임이 시작되면 도깨비가 출현할 수 있도록
		// 플레이어의 뒷쪽에 발생위치를 초기화해둔다.
		this.oni_generate_line = this.player.transform.position.x - 1.0f;

	}

	public void OnPlayerMissed()
	{
		// 한 번에 출현할 도깨비 수를 재설정한다.
		this.oni_appear_num = 1;

		this.no_miss_count = 0;
	}

	public void	oniAppearControl()
	{
	#if false
		for(int i = 0;i < 4;i++) {

			if(Input.GetKeyDown((KeyCode)(KeyCode.Alpha1 + i))) {

				this.group_type_next = (GROUP_TYPE)i;

				this.is_random = false;
			}
		}
		if(Input.GetKeyDown(KeyCode.Alpha0)) {

			this.is_random = !this.is_random;
		}
	#endif

		// 플레이어가 일정거리를 이동할 때마다 도깨비 그룹을 출현시킨다.

		if(this.can_dispatch) {

			// 다음 도깨비 그룹을 발생할 준비를 한다.

		} else {

			// 다음 도깨비 그룹을 발생할 준비가 되지 않았다.

			if(this.is_exclusive_group()) {

				// 특별 패턴의 경우, 화면에서 도깨비가 없어지는 것을 기다려야 한다.
			

				if(GameObject.FindGameObjectsWithTag("OniGroup").Length == 0) {

					this.can_dispatch = true;
				}

			} else {

				// 도깨비를 발생할 준비가 되면, 
				this.can_dispatch = true;
			}

			if(this.can_dispatch) {

				// 도깨비를 발생할 준비가 되면, 플레이어의 현재 위치에서 도깨비의 출현 위치를 계산한다.

				if(this.group_type_next == GROUP_TYPE.NORMAL) {

					this.oni_generate_line = this.player.transform.position.x + this.next_line;

				} else if(this.group_type_next == GROUP_TYPE.SLOW) {

					this.oni_generate_line = this.player.transform.position.x + 50.0f;

				} else {

					this.oni_generate_line = this.player.transform.position.x + 10.0f;
				}
			}
		}

		// 플레이어가 일정거리를 이동하면 다음 그룹의 도깨비를 출현시킨다.

		do {

			if(this.scene_control.oni_group_num >= this.scene_control.oni_group_appear_max) {

				break;
			}
			if(!this.can_dispatch) {

				break;
			}
			if(this.player.transform.position.x <= this.oni_generate_line) {

				break;
			}

			//

			this.group_type = this.group_type_next;

			switch(this.group_type) {
	
				case GROUP_TYPE.SLOW:
				{
					this.dispatch_slow();
				}
				break;
	
				case GROUP_TYPE.DECELERATE:
				{
					this.dispatch_decelerate();
				}
				break;

				case GROUP_TYPE.PASSING:
				{
					this.dispatch_passing();
				}
				break;

				case GROUP_TYPE.RAPID:
				{
					this.dispatch_rapid();
				}
				break;

				case GROUP_TYPE.NORMAL:
				{
					this.dispatch_normal(this.next_speed);
				}
				break;
			}
	
			// 다음에 등장할 그룹의 도깨비 수를 갱신해둔다.
			// （점점 증가시킨다.)
			this.oni_appear_num++;
			this.oni_appear_num = Mathf.Min(this.oni_appear_num, SceneControl.ONI_APPEAR_NUM_MAX);

			this.can_dispatch = false;

			this.no_miss_count++;

			this.scene_control.oni_group_num++;
			
			if(this.is_random) {

				// 다음에 출현할 도깨비 그룹을 선택한다.
				this.select_next_group_type();
			}

		} while(false);
	}

	// 화면에 한 번만 등장시킬 도깨비 그룹?
	public bool	is_exclusive_group()
	{
		bool	ret;

		do {

			ret = true;

			if(this.group_type == GROUP_TYPE.PASSING || this.group_type_next == GROUP_TYPE.PASSING) {

				break;
			}
			if(this.group_type == GROUP_TYPE.DECELERATE || this.group_type_next == GROUP_TYPE.DECELERATE) {

				break;
			}
			if(this.group_type == GROUP_TYPE.SLOW || this.group_type_next == GROUP_TYPE.SLOW) {

				break;
			}

			ret = false;

		} while(false);

		return(ret);
	}

	public void select_next_group_type()
	{

		// 보통 패턴과 특별 패턴 전환 체크.

		if(this.event_type != GROUP_TYPE.NONE) {

			this.event_count--;

			if(this.event_count <= 0) {

				this.event_type = GROUP_TYPE.NONE;

				this.normal_count = Random.Range(3, 7);
			}

		} else {

			this.normal_count--;

			if(this.normal_count <= 0) {

				// 이벤트를 발생시킨다.

				this.event_type = (GROUP_TYPE)Random.Range(0, 4);

				switch(this.event_type) {

					default:
					case GROUP_TYPE.DECELERATE:
					case GROUP_TYPE.PASSING:
					case GROUP_TYPE.SLOW:
					{
						this.event_count = 1;
					}
					break;

					case GROUP_TYPE.RAPID:
					{
						this.event_count = Random.Range(2, 4);
					}
					break;
				}
			}
		}

		// 보통, 특별 패턴 도깨비 그룹을 발생시킨다.

		if(this.event_type == GROUP_TYPE.NONE) {

			// 	보통 패턴의 도깨비 그룹.

			float		rate;
	
			rate = (float)this.no_miss_count/10.0f;
	
			rate = Mathf.Clamp01(rate);
	
			this.next_speed = Mathf.Lerp(OniGroupControl.SPEED_MAX, OniGroupControl.SPEED_MIN, rate);	

			this.next_line = Mathf.Lerp(LevelControl.INTERVAL_MAX, LevelControl.INTERVAL_MIN, rate);

			this.group_type_next = GROUP_TYPE.NORMAL;

		} else {

			// 특별 패턴의 도깨비 그룹.

			this.group_type_next = this.event_type;
		}

	}

	// 보통 패턴.
	public void dispatch_normal(float speed)
	{
		Vector3	appear_position = this.player.transform.position;

		// 플레이어의 앞쪽으로, 화면 테두리 부근에 도깨비를 출현시킨다.
		appear_position.x += appear_margin;
		
		this.create_oni_group(appear_position, speed, OniGroupControl.TYPE.NORMAL);
	}

	// 느린 패턴.
	public void dispatch_slow()
	{
		Vector3	appear_position = this.player.transform.position;

		// 플레이어의 앞쪽으로, 화면 테두리 부근에 도깨비를 출현시킨다.
		appear_position.x += appear_margin;
		
		float		rate;

		rate = (float)this.no_miss_count/10.0f;

		rate = Mathf.Clamp01(rate);

		this.create_oni_group(appear_position, OniGroupControl.SPEED_MIN*5.0f, OniGroupControl.TYPE.NORMAL);
	}

	// 짧은 패턴.
	public void dispatch_rapid()
	{
		Vector3	appear_position = this.player.transform.position;

		// 플레이어의 앞쪽으로, 화면 테두리 부근에 도깨비를 출현시킨다.
		appear_position.x += appear_margin;
		
		//this.create_oni_group(appear_position, OniGroupControl.SPEED_MIN, OniGroupControl.TYPE.NORMAL);
		this.create_oni_group(appear_position, this.next_speed, OniGroupControl.TYPE.NORMAL);
	}

	// 서서히 감속하는 패턴.
	public void dispatch_decelerate()
	{
		Vector3	appear_position = this.player.transform.position;

		// 플레이어의 앞쪽으로, 화면 테두리 부근에 도깨비를 출현시킨다.
		appear_position.x += appear_margin;
		
		this.create_oni_group(appear_position, 9.0f, OniGroupControl.TYPE.DECELERATE);
	}

	// 이동 중 도깨비들의 추격이 발생하는 패턴.
	public void dispatch_passing()
	{
		float	speed_low  = 2.0f;
		float	speed_rate = 2.0f;
		float	speed_high = (speed_low - this.player.GetComponent<Rigidbody>().velocity.x)/speed_rate + this.player.GetComponent<Rigidbody>().velocity.x;

		// 속도가 빠른 도깨비가 속도가 느린 도깨비를 추월하는 위치（0.0 플레이어의 위치 ～ 1.0 화면 오른쪽）.
		float	passing_point = 0.5f;

		Vector3	appear_position = this.player.transform.position;

		// 두 그룹의 도깨비가 이동 중에 교차할 수 있도록 발생위치를 조정한다.

		appear_position.x = this.player.transform.position.x + appear_margin;
		
		this.create_oni_group(appear_position, speed_high, OniGroupControl.TYPE.NORMAL);

		appear_position.x = this.player.transform.position.x + appear_margin*Mathf.Lerp(speed_rate, 1.0f, passing_point);
		
		this.create_oni_group(appear_position, speed_low, OniGroupControl.TYPE.NORMAL);
	}

	// -------------------------------------------------------------------------------- //

	// 도깨비 그룹을 발생시킨다.
	private void create_oni_group(Vector3 appear_position, float speed, OniGroupControl.TYPE type)
	{
		// -------------------------------------------------------- //
		// 그룹 전체의 콜리전(충돌 판정)을 생성한다.	

		Vector3	position = appear_position;

		// OniGroupPrefab 의 인스턴스를 생성한다.
		// "as GameObject" 를 문장끝에 추가하면 생성되는 오브젝트는
		// GameObject 오브젝트가 된다.
		//
		GameObject 	go = GameObject.Instantiate(this.OniGroupPrefab) as GameObject;

		OniGroupControl new_group = go.GetComponent<OniGroupControl>();

		// 지면에 닿는 높이.
		position.y = OniGroupControl.collision_size/2.0f;

		position.z = 0.0f;

		new_group.transform.position = position;

		new_group.scene_control  = this.scene_control;
		new_group.main_camera    = this.scene_control.main_camera;
		new_group.player         = this.player;
		new_group.run_speed      = speed;
		new_group.type           = type;

		// -------------------------------------------------------- //
		// 그룹에 속하는 도깨비 집단을 생성한다.

		Vector3	base_position = position;

		int		oni_num = this.oni_appear_num;

		// 콜리전박스 왼쪽에 보낸다.
		base_position.x -= (OniGroupControl.collision_size/2.0f - OniControl.collision_size/2.0f);

		// 지면에 닿는 높이.
		base_position.y = OniControl.collision_size/2.0f;

		// 도깨비를 발생시킨다.
		new_group.CreateOnis(oni_num, base_position);

	}
}
