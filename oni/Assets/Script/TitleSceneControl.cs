using UnityEngine;
using System.Collections;

public class TitleSceneControl : MonoBehaviour {

	// 진행 상태.
	public enum STEP {

		NONE = -1,

		TITLE = 0,				// 타이틀 표시（버튼 대기）.
		WAIT_SE_END,			// 시작 SE 종료를 기다린다.
		FADE_WAIT,				// 페이드 종료 대기.

		NUM,
	};

	private STEP	step = STEP.NONE;
	private STEP	next_step = STEP.NONE;
	private float	step_timer = 0.0f;

	private FadeControl	fader = null;					// 페이드 제어.   
	
	public UnityEngine.UI.Image		uiImageStart;		// 『시작하기！』의 UI.Image.	
	
	// 시작 버튼을 누를 때 애니메이션 처리되는 시간.
	private const float	TITLE_ANIME_TIME = 0.1f;
	private const float	FADE_TIME = 1.0f;
	
	// -------------------------------------------------------------------------------- //

	void 	Start()
	{
		// 플레이어를 조작할 수 없게 처리한다.
		PlayerControl	player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerControl>();
		player.UnPlayable();
		
		// 페이드 제어.
		this.fader = FadeControl.get();
		this.fader.fade( 1.0f, new Color( 0.0f, 0.0f, 0.0f, 1.0f ), new Color( 0.0f, 0.0f, 0.0f, 0.0f) );
		
		this.next_step = STEP.TITLE;
	}

	void 	Update()
	{
		this.step_timer += Time.deltaTime;

		// 다음 상태로 진행할지 점검한다.
		switch(this.step) {

			case STEP.TITLE:
			{
				// 마우스를 클릭했다.  
				//
				if(Input.GetMouseButtonDown(0)) {

					this.next_step = STEP.WAIT_SE_END;
				}
			}
			break;

			case STEP.WAIT_SE_END:
			{
				// SE 재생이 종료되면 페이드아웃. 
			
				bool	to_finish = true;

				do {

					if(!this.GetComponent<AudioSource>().isPlaying) {

						break;
					}

					if(this.GetComponent<AudioSource>().time >= this.GetComponent<AudioSource>().clip.length) {

						break;
					}

					to_finish = false;

				} while(false);

				if(to_finish) {

					this.fader.fade( FADE_TIME, new Color( 0.0f, 0.0f, 0.0f, 0.0f ), new Color( 0.0f, 0.0f, 0.0f, 1.0f) );
				
					this.next_step = STEP.FADE_WAIT;
				}
			}
			break;
			
			case STEP.FADE_WAIT:
			{
				// 페이드가 종료되면 게임씬을 로드하여 종료.
				if(!this.fader.isActive()) 
				{
					UnityEngine.SceneManagement.SceneManager.LoadScene("GameScene");
				}
			}
			
			break;
		}

		// 상태가 바뀔 때의 초기화 처리. 

		if(this.next_step != STEP.NONE) {

			switch(this.next_step) {

				case STEP.WAIT_SE_END:
				{
					// 시작 SE를 재생한다.
					this.GetComponent<AudioSource>().Play();
				}
				break;
			}

			this.step = this.next_step;
			this.next_step = STEP.NONE;

			this.step_timer = 0.0f;
		}

		// 각 상태에서의 실행 처리.

		switch(this.step) {

			case STEP.WAIT_SE_END:
			{
				float	scale	= 1.0f;
				
				float	rate = this.step_timer/TITLE_ANIME_TIME;
					
				scale = Mathf.Lerp(2.0f, 1.0f, rate);

				this.uiImageStart.GetComponent<RectTransform>().localScale = Vector3.one*scale;
			}
			break;
		}

	}
}
