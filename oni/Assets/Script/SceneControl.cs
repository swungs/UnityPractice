using UnityEngine;
using System.Collections;

public class SceneControl : MonoBehaviour {

	// -------------------------------------------------------------------------------- //
	// 프리팹.

	public GameObject		OniGroupPrefab = null;
	public GameObject		OniPrefab = null;
	public GameObject		OniEmitterPrefab = null;
	public GameObject[]		OniYamaPrefab;

	// SE.
	public AudioClip	GameStart = null;
	public AudioClip	EvalSound = null;			// 평가.
	public AudioClip	ReturnSound = null;			// 되돌아가기.

	// -------------------------------------------------------------------------------- //

	// 플레이어.
	public PlayerControl	player = null;

	// 점수. 
	public ScoreControl		score_control = null;
	
	// 카메라. 
	public GameObject	main_camera = null;

	// 도깨비 출현을 제어한다.
	public LevelControl	level_control = null;
	
	// 득점 계산을 제어한다.
	public ResultControl result_control = null;

	// 목표지점의 위에서 떨어지는 도깨비를 위한 오브젝트.
	public OniEmitterControl	oni_emitter = null;

	// GUI（２D 표시）제어.  
	private GUIControl	gui_control = null;
	
	// 페이드 제어.
	private FadeControl	fader = null;
	
	// -------------------------------------------------------------------------------- //

	// 게임 진행 상태.
	public enum STEP {

		NONE = -1,

		START,					// 『시작하기！』문자 표시.
		GAME,					// 게임 중.
		ONI_VANISH_WAIT,		// 시간 종료 후, 화면의 도깨비가 사라지는 것을 기다린다.
		LAST_RUN,				// 도깨비가 출현하지 않은 상태에서 진행.
		PLAYER_STOP_WAIT,		// 플레이어가 정지하는 것을 기다린다.

		GOAL,					// 목표지점 연출.
		ONI_FALL_WAIT,			// 『위에서 도깨비가 떨어진다.』 연출이 종료되는 것을 기다린다.
		RESULT_DEFEAT,			// 쓰러뜨린 도깨비 수의 평가 표시.
		RESULT_EVALUATION,		// 쓰러뜨린 시간상의 평가 표시.
		RESULT_TOTAL,			// 종합평가.

		GAME_OVER,				// 게임 종료.  
		GOTO_TITLE,				// 타이틀로 전환.

		NUM,
	};

	public STEP	step      = STEP.NONE;			// 현재 게임 진행 상태.
	public STEP	next_step = STEP.NONE;			// 변화하는 상태.
	public float	step_timer      = 0.0f;		// 상태가 변화하는 시간.
	public float	step_timer_prev = 0.0f;

	// -------------------------------------------------------------------------------- //

	// 마우스 버튼을 클릭하여 공격이 닿을 때까지의 시간(평가에 사용한다.)
	public float		attack_time = 0.0f;


	// 평가.
	// 도깨비에 근접하여 공격할수록 고득점.
	public enum EVALUATION {

		NONE = -1,

		OKAY = 0,		// 보통. 
		GOOD,			// 잘했음. 
		GREAT,			// 훌륭함.

		MISS,			// 실패(도깨비와 부딪혔다.)

		NUM,
	};
	public static string[] evaluation_str = {

		"okay",
		"good",
		"great",
		"miss",
	};
	
	public EVALUATION	evaluation = EVALUATION.NONE;

	// -------------------------------------------------------------------------------- //

	// 게임 전체 결과.
	public struct Result {

		public int		oni_defeat_num;			// 공격한 도깨비수(총합계).
		public int[]	eval_count;				// 각 평가 횟수.

		public int		rank;					// 게임 전체 결과.
		
		public float	score;					// 현재 점수. 
		public float	score_max;				// 게임에서 취득할 수 있는 최대 점수.
		
	};

	public Result	result;

	// -------------------------------------------------------------------------------- //

	// 한 번에 출현할 수 있는 도깨비의 최대수.
	// 실패하지 않으면 도깨비의 수가 점점 늘지만, 최대수 이상 증가하지 않는다.
	public const int	ONI_APPEAR_NUM_MAX = 10;

	// 게임이 종료되는 도깨비 그룹의 수
	public int				oni_group_appear_max = 50;
	
	// 실패시 감소하는 출현수.
	public const int		oni_group_penalty = 1;
	
	// 점수를 가리는 출현수.
	public const float		SCORE_HIDE_NUM = 40;
	
	// 그룹의 출현수.
	public int				oni_group_num = 0;

	// 공격한 or 부딪힌 도깨비 그룹 수.
	public int				oni_group_complite = 0;
	
	// 공격한 도깨비 그룹 수.
	public int				oni_group_defeat_num = 0;
	
	// 부딪힌 도깨비 그룹 수.
	public int				oni_group_miss_num = 0;
	
	// 시작 연출（『시작하기！』문자가 표시되고 있는）시간.
	private const float	START_TIME = 2.0f;

	// 목표지점 연출 시『도깨비가 축적된 곳』에서『플레이어가 정지한 위치』까지의 거리.
	private const float	GOAL_STOP_DISTANCE = 8.0f;

	// 평가 결정 시, 마우스 버튼을 클릭하여 공격이 닿을 때까지의 시간.
	public const float	ATTACK_TIME_GREAT = 0.05f;
	public const float	ATTACK_TIME_GOOD  = 0.10f;

	// -------------------------------------------------------------------------------- //
	// 디버그용 플래그.
	// 적절히 변경하며 게임이 어떻게 바뀌는지 시도해보자!

	// true 일 경우 쓰러뜨린 도깨비가 카메라의 로컬 좌표계로 이동하게 된다.
	// 카메라의 움직임과 연동되기 때문에, 카메라가 갑자기 정지한 경우에도 움직임이 부자연스럽게 
	// 변하지 않는다.
	//
	public const bool	IS_ONI_BLOWOUT_CAMERA_LOCAL = true;

	// 도깨비 그룹의 콜리전을 표시한다.（디버그용）.
	// 여러 마리의 도깨비가 출현하지만, 그룹내에서 공통의 콜리전을 사용한다.
	//
	// 이것은
	//
	// ・플레이어가 도깨비와 부딪혔을 때의 처리를 쉽게 조정할 수 있기 때문
	// ・공격당한 도깨비가 한꺼번에 날아가는 장면이 멋지기 때문
	//
	// 이다.
	//
	public const bool	IS_DRAW_ONI_GROUP_COLLISION = false;

	// 플레이어의 공격 시, 공격 판정을 표시한다.
	public const bool	IS_DRAW_PLAYER_ATTACK_COLLISION = false;

	// 디버그용 전자동 기능.
	// true 로 설정하면 공격판정이 표시된 상태가 지속되기 때문이다.
	//
	public const bool	IS_AUTO_ATTACK = false;

	// AUTO_ATTACK 일 때의 평가.
	public EVALUATION	evaluation_auto_attack = EVALUATION.GOOD;
	
	// 공격한 도깨비의 수가 삭제된 순간의 도깨비 수.
	private int         backup_oni_defeat_num = -1;
	
	// 디버그용 배경 모델을 표시한다.（빨강, 파랑, 초록 색깔을 표시한다）.
	public const bool	IS_DRAW_DEBUG_FLOOR_MODEL = false;

	public	float		eval_rate_okay  = 1.0f;
	public	float		eval_rate_good  = 2.0f;
	public	float		eval_rate_great = 4.0f;
	public	int			eval_rate		= 1;
	
	// -------------------------------------------------------------------------------- //
	
	void	Start()
	{
		// 플레이어 인스턴스를 찾아둔다.  
		this.player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerControl>();

		this.player.scene_control = this;

		// 카메라 인스턴스를 찾아둔다.  
		this.main_camera = GameObject.FindGameObjectWithTag("MainCamera");

		this.level_control = new LevelControl();
		this.level_control.scene_control = this;
		this.level_control.player = this.player;
		this.level_control.OniGroupPrefab = this.OniGroupPrefab;
		this.level_control.create();
		
		this.result_control = new ResultControl();

		// GUI 제어 스크립트（컴포넌트）.
		this.gui_control = GUIControl.get();
		
		// 점수 인스턴스를 찾아둔다.   
		this.score_control = this.gui_control.score_control;
		
		// 게임 결과를 삭제한다.    
		this.result.oni_defeat_num = 0;
		this.result.eval_count = new int[(int)EVALUATION.NUM];
		this.result.rank = 0;
		this.result.score = 0;
		this.result.score_max = 0;
		
		for(int i = 0;i < this.result.eval_count.Length;i++) {

			this.result.eval_count[i] = 0;
		}
		
		// 페이드인 시작.  
		this.fader = FadeControl.get();
		this.fader.fade( 3.0f, new Color( 0.0f, 0.0f, 0.0f, 1.0f ), new Color( 0.0f, 0.0f, 0.0f, 0.0f ) );
		
		this.next_step = STEP.START;
	}

	void	Update()
	{
		if(Input.GetKeyDown(KeyCode.P)) {

			Debug.Break();
		}

		// 게임 현재 상태를 관리한다.
		this.step_timer_prev = this.step_timer;
		this.step_timer += Time.deltaTime;

		// 다음 상태로 진행할지를 점검한다.
		switch(this.step) {
		
			case STEP.START:
			{
				if(this.step_timer > SceneControl.START_TIME) {

					GUIControl.get().setVisibleStart(false);
					this.next_step = STEP.GAME;
				}
			}
			break;

			case STEP.GAME:
			{
				// 도깨비의 출현 최대수를 초과하면 도깨비 발생을 중지한다.
				if(this.oni_group_complite >= this.oni_group_appear_max) {

					next_step = STEP.ONI_VANISH_WAIT;
				}
			
				if(this.oni_group_complite >= SCORE_HIDE_NUM && this.backup_oni_defeat_num == -1) {

					this.backup_oni_defeat_num = this.result.oni_defeat_num;
				}
			}
			break;

			case STEP.ONI_VANISH_WAIT:
			{
				do {

					// 도깨비（공격받기 전）가 모두 쓰러질 때까지 기다린다.
					if(GameObject.FindGameObjectsWithTag("OniGroup").Length > 0) {

						break;
					}

					// 플레이어가 가속할 때까지 기다린다.
					// 도깨비 무리를 화면 밖으로 출현시키기 위해 마지막 도깨비를 쓰러뜨리고 일정 거리를
					// 달리도록 설정한다.
					if(this.player.GetSpeedRate() < 0.5f) {

						break;
					}

					//

					next_step = STEP.LAST_RUN;

				} while(false);
			}
			break;

			case STEP.LAST_RUN:
			{
				if(this.step_timer > 2.0f) {

					// 플레이어를 정지시킨다.
					next_step = STEP.PLAYER_STOP_WAIT;
				}
			}
			break;

			case STEP.PLAYER_STOP_WAIT:
			{
				// 플레이어가 정지하면 목표 성공 연출 시작.
				if(this.player.IsStopped()) {
			
					this.gui_control.score_control.setNumForce(this.backup_oni_defeat_num);
					this.gui_control.score_control.setNum(this.result.oni_defeat_num);
					next_step = STEP.GOAL;
				}
			}
			break;

			case STEP.GOAL:
			{
				// 도깨비가 전부 화면에 나올 때까지 기다린다.
				if(this.oni_emitter.oni_num == 0) {

					this.next_step = STEP.ONI_FALL_WAIT;
				}
			}
			break;

			case STEP.ONI_FALL_WAIT:
			{
				if(!this.score_control.isActive() && this.step_timer > 1.5f) {
					this.next_step = STEP.RESULT_DEFEAT;
				}
			}
			break;

			case STEP.RESULT_DEFEAT:
			{
				if(this.step_timer >= 0.4f && this.step_timer_prev < 0.4f )
				{
					// SE（『둥둥~』）.
					this.GetComponent<AudioSource>().PlayOneShot(this.EvalSound);
				}
				// 평가 표시가 종료될 때까지 기다린다.
				//
				if(this.step_timer > 0.5f) {

					this.next_step = STEP.RESULT_EVALUATION;
				}
			}
			break;
			
			case STEP.RESULT_EVALUATION:
			{
				if(this.step_timer >= 0.4f && this.step_timer_prev < 0.4f )
				{
					// SE（『둥둥~』）.
					this.GetComponent<AudioSource>().PlayOneShot(this.EvalSound);
				}
				// 평가 표시가 종료될 때까지 기다린다.
				//
				if(this.step_timer > 2.0f) {

					this.next_step = STEP.RESULT_TOTAL;
				}
			}
			break;
			
			case STEP.RESULT_TOTAL:
			{
				if(this.step_timer >= 0.4f && this.step_timer_prev < 0.4f )
				{
					// SE（『둥둥~』）.
					this.GetComponent<AudioSource>().PlayOneShot(this.EvalSound);
				}
				// 평가 표시가 종료될 때까지 기다린다.
				//
				if(this.step_timer > 2.0f) {

					this.next_step = STEP.GAME_OVER;
				}
			}
			break;

			case STEP.GAME_OVER:
			{
				// 마우스를 클릭하면 페이드 아웃하며 타이틀 화면으로 돌아간다.
				//
				if(Input.GetMouseButtonDown(0)) {
				
					// 페이드 아웃시킨다.
					this.fader.fade( 1.0f, new Color( 0.0f, 0.0f, 0.0f, 0.0f ), new Color( 0.0f, 0.0f, 0.0f, 1.0f ) );
					this.GetComponent<AudioSource>().PlayOneShot(this.ReturnSound);
					
					this.next_step = STEP.GOTO_TITLE;
				}
			}
			break;
			
			case STEP.GOTO_TITLE:
			{
				// 페이드가 종료되면 타이틀 화면으로 돌아간다.
				//
				if(!this.fader.isActive()) { 
					UnityEngine.SceneManagement.SceneManager.LoadScene("TitleScene");
				}
			}
			break;
		}

		// 상태가 바뀌었을 때 초기화 처리.

		if(this.next_step != STEP.NONE) {

			switch(this.next_step) {
			
				case STEP.START:
				{
					// 『시작하기！』문자를 표시한다.
					GUIControl.get().setVisibleStart(true);
				}
				break;

				case STEP.PLAYER_STOP_WAIT:
				{
					// 플레이어를 정지시킨다.
					this.player.StopRequest();

					// -------------------------------------------------------- //
					// 『도깨비가 축적된 무리』를 생성한다.
					
					if( this.result_control.getTotalRank() > 0 ) {
						GameObject	oni_yama = Instantiate(this.OniYamaPrefab[this.result_control.getTotalRank() - 1]) as GameObject;
				
						Vector3		oni_yama_position = this.player.transform.position;
				
						oni_yama_position.x += this.player.CalcDistanceToStop();
						oni_yama_position.x += SceneControl.GOAL_STOP_DISTANCE;
	
						oni_yama_position.y = 0.0f;
				
						oni_yama.transform.position = oni_yama_position;
					}
					else{
						
					}

					// -------------------------------------------------------- //
				}
				break;

				case STEP.GOAL:
				{
					// 목표 지점 연출 시작.

					// 『도깨비가 화면 위에서 날아온다』용 에미터를 생성한다.

					GameObject	go = Instantiate(this.OniEmitterPrefab) as GameObject;
	
					this.oni_emitter = go.GetComponent<OniEmitterControl>();

					Vector3		emitter_position = oni_emitter.transform.position;

					// 도깨비 무리 위에 놓는다.

					emitter_position.x  = this.player.transform.position.x;
					emitter_position.x += this.player.CalcDistanceToStop();
					emitter_position.x += SceneControl.GOAL_STOP_DISTANCE;
	
					this.oni_emitter.transform.position = emitter_position;

					// 최종 평가에서 몰려오는 도깨비의 수를 변경한다.

					int		oni_num = 0;

					switch(this.result_control.getTotalRank()) {
						case 0:		oni_num = Mathf.Min( this.result.oni_defeat_num, 10 );	break;
						case 1:		oni_num = 6;	break;
						case 2:		oni_num = 10;	break;
						case 3:		oni_num = 20;	break;
					}
				
					this.oni_emitter.oni_num = oni_num;
					if( oni_num == 0 )
					{
						this.oni_emitter.is_enable_hit_sound = false;
					}
				}
				break;

				case STEP.RESULT_DEFEAT:
				{
					// 평가 표시 후 도깨비 낙하음을 정지한다.
					this.oni_emitter.is_enable_hit_sound = false;
					// 『공격한 도깨비』 평가 표시를 시작한다.
					this.gui_control.startDispDefeatRank();
				}
				break;

				case STEP.RESULT_EVALUATION:
				{
					// 『근접 거리 공격』평가 표시를 시작한다.
					this.gui_control.startDispEvaluationRank();
				}
				break;

				case STEP.RESULT_TOTAL:
				{
					// 『공격한 도깨비』『근접 거리 공격』평가 표시를 삭제한다.
					this.gui_control.hideDefeatRank();
					this.gui_control.hideEvaluationRank();

					// 종합 평가 표시를 시작한다.
					this.gui_control.startDispTotalRank();
				}
				break;

				case STEP.GAME_OVER:
				{
					// 『되돌아가기！』문자를 표시한다.
					this.gui_control.setVisibleReturn(true);
				}
				break;
			}

			this.step = this.next_step;
			this.next_step = STEP.NONE;

			this.step_timer = 0.0f;
			this.step_timer_prev = -1.0f;
		}

		// 각 상태에서 실행 처리.

		switch(this.step) {

			case STEP.GAME:
			{
				// 도깨비 출현 제어.
				this.level_control.oniAppearControl();
			}
			break;
		}

	}

	// 플레이어가 실패했을 때의 처리.   
	public void	OnPlayerMissed()
	{
		this.oni_group_miss_num++;
		this.oni_group_complite++;
		this.oni_group_appear_max -= oni_group_penalty;
		
		this.level_control.OnPlayerMissed();

		this.evaluation = EVALUATION.MISS;

		this.result.eval_count[(int)this.evaluation]++;

		// 화면 내의 그룹을 모두 퇴장시킨다.

		GameObject[] oni_groups = GameObject.FindGameObjectsWithTag("OniGroup");

		foreach(var oni_group in oni_groups) {
			this.oni_group_num--;
			oni_group.GetComponent<OniGroupControl>().beginLeave();
		}
	}

	// 쓰러진 도깨비 수를 추가.
	public void	AddDefeatNum(int num)
	{
		this.oni_group_defeat_num++;
		this.oni_group_complite++;
		this.result.oni_defeat_num += num;
		
		// 마우스 버튼을 클릭한 시점을 평가한다.
		// （마우스를 클릭 후 공격이 닿을 때까지의 소요시간이 짧다=근접 거리에서 공격했다）.

		this.attack_time = this.player.GetComponent<PlayerControl>().GetAttackTimer();

		if(this.evaluation == EVALUATION.MISS) {

			// 실패 후（느린 동작）는 OKAY 표시만 된다.
			this.evaluation = EVALUATION.OKAY;

		} else {

			if(this.attack_time < ATTACK_TIME_GREAT) {
	
				this.evaluation = EVALUATION.GREAT;
	
			} else if(this.attack_time < ATTACK_TIME_GOOD) {
	
				this.evaluation = EVALUATION.GOOD;
	
			} else {
	
				this.evaluation = EVALUATION.OKAY;
			}
		}

		if(SceneControl.IS_AUTO_ATTACK) {

			this.evaluation = this.evaluation_auto_attack;
		}

		this.result.eval_count[(int)this.evaluation] += num;
		
		// 득점 계산.
		float[] score_list = { this.eval_rate_okay, this.eval_rate_good, this.eval_rate_great, 0 };
		this.result.score_max += num * this.eval_rate_great;
		this.result.score += num * score_list[(int)this.evaluation];
		
		this.result_control.addOniDefeatScore(num);
		this.result_control.addEvaluationScore((int)this.evaluation);
	}
	
	//점수 표시를 해도 되는가.
	public bool IsDrawScore()
	{
		if( this.step >= STEP.GOAL )
		{
			return true;
		}
		
		if(this.oni_group_complite >= SCORE_HIDE_NUM )
		{
			return false;
		}
		
		return true;
	}

	// ================================================================ //
	// 인스턴스.  

	protected	static SceneControl	instance = null;

	public static SceneControl	get()
	{
		if(SceneControl.instance == null) {

			GameObject		go = GameObject.Find("SceneControl");

			if(go != null) {

				SceneControl.instance = go.GetComponent<SceneControl>();

			} else {

				Debug.LogError("Can't find game object \"SceneControl\".");
			}
		}

		return(SceneControl.instance);
	}

}
