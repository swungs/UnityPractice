using UnityEngine;
using System.Collections;

public class GUIControl : MonoBehaviour {

	public SceneControl		scene_control = null;
	public ScoreControl		score_control = null;
	
	public GameObject	uiImageStart;		// 『시작하기！』
	public GameObject	uiImageReturn;		// 『되돌아가기！』

	public RankDisp		rankSmallDefeat;				// 『공격한 도깨비』평가.
	public RankDisp		rankSmallEval;					// 『근접 거리 공격』평가.
	public RankDisp		rankTotal;						// 총평가.

	public UnityEngine.Sprite[]	uiSprite_GradeSmall;	// 『공격한 도깨비』『근접 거리 공격』평가 표시(훌륭함/좋음/보통/나쁨）
	public UnityEngine.Sprite[]	uiSprite_Grade;			// 총평가 표시(훌륭함/좋음/보통/나쁨）

	// ================================================================ //

	void	Awake()
	{
		this.scene_control = SceneControl.get();
		this.score_control = GetComponent<ScoreControl>();
		
		this.score_control.setNumForce( this.scene_control.result.oni_defeat_num );

		this.rankSmallDefeat.uiSpriteRank = this.uiSprite_GradeSmall;
		this.rankSmallEval.uiSpriteRank   = this.uiSprite_GradeSmall;
		this.rankTotal.uiSpriteRank       = this.uiSprite_Grade;
	}

	void	Start()
	{
	}
	
	void	Update()
	{
		// "공격한 도깨비 수"를 점수 기록에 표시.
		this.score_control.setNum(this.scene_control.result.oni_defeat_num);

		// ---------------------------------------------------------------- //
		// 디버그용
	#if false
		SceneControl	scene = this.scene_control;

		dbPrint.setLocate(10, 5);
		dbPrint.print(scene.attack_time);
		dbPrint.print(scene.evaluation);
		if(this.scene_control.level_control.is_random) {

			dbPrint.print("RANDOM(" + scene.level_control.group_type_next + ")");

		} else {

			dbPrint.print(scene.level_control.group_type_next);
		}

		dbPrint.print(scene.result.oni_defeat_num);

		// 공격 평가(근접 공격) 합계
		for(int i = 0;i < (int)SceneControl.EVALUATION.NUM;i++) {

			dbPrint.print(((SceneControl.EVALUATION)i).ToString() + " " + scene.result.eval_count[i].ToString());
		}
	#endif
	}

	// 『시작하기！』문구를 표시 또는 표시하지 않는다.
	public void	setVisibleStart(bool is_visible)
	{
		this.uiImageStart.SetActive(is_visible);
	}

	// 『되돌아가기！』 문구를 표시 또는 표시하지 않는다.
	public void	setVisibleReturn(bool is_visible)
	{
		this.uiImageReturn.SetActive(is_visible);
	}

	// 『공격한 도깨비』평가 표시를 시작한다.
	public void	startDispDefeatRank()
	{
		int		rank  = this.scene_control.result_control.getDefeatRank();

		this.rankSmallDefeat.startDisp(rank);
	}

	// 『공격한 도깨비』평가 표시를 삭제한다.
	public void	hideDefeatRank()
	{
		this.rankSmallDefeat.hide();
	}

	// 『공격한 도깨비』평가 표시를 시작한다.
	public void	startDispEvaluationRank()
	{
		int		rank  = this.scene_control.result_control.getEvaluationRank();

		this.rankSmallEval.startDisp(rank);
	}

	// 『공격한 도깨비』평가 표시를 삭제한다.
	public void	hideEvaluationRank()
	{
		this.rankSmallEval.hide();
	}

	// 총평가 표시를 시작한다.
	public void	startDispTotalRank()
	{
		int		rank  = this.scene_control.result_control.getTotalRank();

		this.rankTotal.startDisp(rank);
	}

	void	OnGUI()
	{			
	}

	// ================================================================ //
	// 인스턴스.

	protected	static GUIControl	instance = null;

	public static GUIControl	get()
	{
		if(GUIControl.instance == null) {

			GameObject		go = GameObject.Find("GameCanvas");

			if(go != null) {

				GUIControl.instance = go.GetComponent<GUIControl>();

			} else {

				Debug.LogError("Can't find game object \"GUIControl\".");
			}
		}

		return(GUIControl.instance);
	}
}
