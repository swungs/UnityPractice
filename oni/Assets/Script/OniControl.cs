using UnityEngine;
using System.Collections;

public class OniControl : MonoBehaviour {

	// 플레이어
	public PlayerControl		player = null;

	// 카메라
	public GameObject	main_camera = null;

	// 콜리전 박스의 크기(1변의 길이）.
	public const float collision_size = 0.5f;

	// 아직 살아있는가?
	private bool	is_alive = true;

	// 생성 위치.
	private Vector3	initial_position;

	// 이동시 좌우로 움직이는 주기.
	public float	wave_angle_offset = 0.0f;

	// 이동시 좌우로 움직이는 폭.
	public float	wave_amplitude = 0.0f;

	// 도깨비 상태.
	enum STEP {

		NONE = -1,

		RUN = 0,			// 달리며 도망친다.
		DEFEATED,			// 공격받아 흩어진다.

		NUM,
	};

	// 현재 상태.
	private	STEP		step      = STEP.NONE;

	// 다음으로 진행되는 상태.
	private	STEP		next_step = STEP.NONE;

	// [sec]상태가 진행되는 시간.
	private float		step_time = 0.0f;

	// DEFEATED, FLY_TO_STACK 시작 시의 속도벡터.
	private Vector3		blowout_vector = Vector3.zero;
	private Vector3		blowout_angular_velocity = Vector3.zero;

	// -------------------------------------------------------------------------------- //

	void 	Start()
	{
		// 생성 위치.
		this.initial_position = this.transform.position;

		this.transform.rotation = Quaternion.AngleAxis(180.0f, Vector3.up);

		this.GetComponent<Collider>().enabled = false;

		// 회전속도 제어에 제한을 두지 않는다.
		this.GetComponent<Rigidbody>().maxAngularVelocity = float.PositiveInfinity;

		// 모델의 중심이 조금 아래쪽에 있으므로, 무게 중심을 약간 아래쪽에 두도록 한다.
		//this.GetComponent<Rigidbody>().centerOfMass = new Vector3(0.0f, 0.5f, 0.0f);
	}

	void	Update()
	{
		this.step_time += Time.deltaTime;

		// 상태 변화 체크.
		// （외부의 요청 외에는 변화가 없다.）

		switch(this.step) {

			case STEP.NONE:
			{
				this.next_step = STEP.RUN;
			}
			break;
		}

		// 초기화.
		// 상태가 변했을 때의 초기화 처리.

		if(this.next_step != STEP.NONE) {

			switch(this.next_step) {

				case STEP.DEFEATED:
				{
					this.GetComponent<Rigidbody>().velocity = this.blowout_vector;

					// 회전 각의 속도.
					this.GetComponent<Rigidbody>().angularVelocity = this.blowout_angular_velocity;

					// 부모 자식 관계를 따르지 않는다.
					// 부모（OniGroup）가 삭제되면 함께 삭제되기 때문에.
					this.transform.parent = null;
			
					// 카메라 좌표계 내에서 움직이도록 한다. 
					// （카메라 움직임과 연동하도록 하기 위해）
					if(SceneControl.IS_ONI_BLOWOUT_CAMERA_LOCAL) {
			
						this.transform.parent = this.main_camera.transform;
					}

					// 공격받는 움직임을 재생한다.
					this.transform.GetChild(0).GetComponent<Animation>().Play("oni_yarare");

					this.is_alive = false;

					// 그림자를 표시하지 않는다.
					foreach(var renderer in this.GetComponentsInChildren<SkinnedMeshRenderer>()) {
					
						renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
					}
				}
				break;
			}

			this.step = this.next_step;

			this.next_step = STEP.NONE;

			this.step_time = 0.0f;
		}

		// 각 상태에서 실행 처리.

		Vector3	new_position = this.transform.position;

		float low_limit = this.initial_position.y;

		switch(this.step) {

			case STEP.RUN:
			{
				// 도깨비가 살아있는 동안에는 지면 아래로 떨어지지 않도록 주의한다.

				if(new_position.y < low_limit) {
		
					new_position.y = low_limit;
				}
	
				// 좌우로 움직인다.
	
				float	wave_angle = 2.0f*Mathf.PI*Mathf.Repeat(this.step_time, 1.0f) + this.wave_angle_offset;
	
				float	wave_offset = this.wave_amplitude*Mathf.Sin(wave_angle);
	
				new_position.z = this.initial_position.z + wave_offset;
	
				// 방향（Y축 회전）.
				if(this.wave_amplitude > 0.0f) {
	
					this.transform.rotation = Quaternion.AngleAxis(180.0f - 30.0f*Mathf.Sin(wave_angle + 90.0f), Vector3.up);
				}

			}
			break;

			case STEP.DEFEATED:
			{
				// 도깨비가 죽은 후 지면이 가리게 되는 경우가 있으므로 속도를 상향시킨다.
				// （＝죽은 후）지면 아래로 떨어지지 않도록 주의한다.
				if(new_position.y < low_limit) {
	
					if(this.GetComponent<Rigidbody>().velocity.y > 0.0f) {
	
						new_position.y = low_limit;
					}
				}
	
				// 조금 뒤쪽으로 쓰러지듯이 표현하고 싶다.
				if(this.transform.parent != null) {
	
					this.GetComponent<Rigidbody>().velocity += -3.0f*Vector3.right*Time.deltaTime;
				}
			}
			break;

		}

		this.transform.position = new_position;

		// 불필요한 경우 삭제한다.
		//
		// 화면 밖으로 나온 경우
		//-공격 받은 후
		//-SE재생이 멈추었다.
		//
		// OnBecameInvisible() 은 화면 밖으로 나온 순간에만 불러올수 있으므로
		// 『화면 밖에서 잠시 소리를 울린 후』사용할 수 있다.
		//

		do {

			// 화면 밖에서 도깨비(도깨비 그룹)을 발생시키기 때문에
			// 도깨비 생존 상태에서도 계속 도깨비를 발생시키게 된다. 
			// 따라서 this.is_alive 를 점검하여 도깨비의 사망상태를 확인해 화면에 출현했을 때에는 삭제하도록 한다.
			if(this.GetComponent<Renderer>().isVisible) {

				break;
			}

			if(this.is_alive) {

				break;
			}

			// SE 를 재생하는 동안에는 삭제하지 않는다.
			if(this.GetComponent<AudioSource>().isPlaying) {

				if(this.GetComponent<AudioSource>().time < this.GetComponent<AudioSource>().clip.length) {

					break;
				}
			}

			//

			Destroy(this.gameObject);

		} while(false);
	}

	// 동작 재생 속도를 설정한다.
	public void setMotionSpeed(float speed)
	{
		this.transform.GetChild(0).GetComponent<Animation>()["oni_run1"].speed = speed;
		this.transform.GetChild(0).GetComponent<Animation>()["oni_run2"].speed = speed;
	}

	// 공격을 받았을 때의 처리를 시작한다.
	public void AttackedFromPlayer(Vector3 blowout, Vector3	angular_velocity)
	{
		this.blowout_vector           = blowout;
		this.blowout_angular_velocity = angular_velocity;

		// 부모 자식 구조를 따르지 않는다.
		// 부모（OniGroup）가 삭제되면 함께 삭제되기 때문에.
		this.transform.parent = null;

		this.next_step = STEP.DEFEATED;
	}

}
