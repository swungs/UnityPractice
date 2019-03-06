using UnityEngine;
using System.Collections;

public class ScoreControl : MonoBehaviour {

	public	bool		drawZero;

	public	AudioClip	CountUpSound = null;			// CountUp
	public	AudioClip[]	CountUpSounds = null;			// CountUp
	public	AudioSource	count_up_audio;					// CountUp 소리 재생.
	private	int			CountLevel;

	private	int			targetNum;
	private int			currentNum;
	private float		timer;

	public GameObject				uiScore;				// 전체 점수의 GameObject.
	public UnityEngine.UI.Image		uiImageDigit0;
	public UnityEngine.UI.Image[]	uiImageScoreDigits;
	public UnityEngine.Sprite[]		numSprites;
	
	// Use this for initialization
	void Start ()
	{
		this.count_up_audio = this.gameObject.AddComponent<AudioSource>();
	
		this.timer	= 0.0f;
	}

	public void	Update()
	{
		if( this.targetNum > this.currentNum )
		{
			this.timer += Time.deltaTime;
			
			if( timer > 0.1f )
			{
				// 무작위로 SE를 재생한다..
				int	idx = Random.Range(0, this.CountUpSounds.Length);
		
				this.count_up_audio.PlayOneShot( this.CountUpSounds[idx] );

				this.timer	= 0.0f;
				
				// 너무 차이가 있을 때에는 5씩 카운트업한다.
				if( this.targetNum - this.currentNum > 10 )
				{
					this.currentNum += 5;
				}
				else
				{
					this.currentNum++;
				}
				CountLevel++;
			}
		}
		else
		{
			CountLevel	= 0;
		}

		// 각 자릿수의 Image 에 숫자 텍스처를 설정한다.

		float	scale = 1.0f;

		if(this.targetNum != this.currentNum) {

			scale = 2.5f - 1.5f*(this.timer*10.0f);
		}

		int		disp_number = Mathf.Max(0, this.currentNum);

		for(int i = 0;i < this.uiImageScoreDigits.Length;i++) {

			int		number_at_digit = disp_number%10;

			if(number_at_digit == 0) {

				if(!this.drawZero) {
					continue;
				}
			}

			this.uiImageScoreDigits[i].sprite = this.numSprites[number_at_digit];
			this.uiImageScoreDigits[i].GetComponent<RectTransform>().localScale = Vector3.one*scale;

			disp_number /= 10;
		}

		// 표시/미표시.

		if(SceneControl.get().IsDrawScore()) {

			this.uiScore.SetActive(true);

		} else {

			this.uiScore.SetActive(false);
		}
	}

	//표시할 숫자 설정.
	public void setNum( int num )
	{
		if( this.targetNum == this.currentNum )
		{
			this.timer	= 0.0f;
		}
		this.targetNum	= num;
	}
	
	//표시할 숫자를 바로 설정.
	public void setNumForce( int num )
	{
		this.targetNum		= num;
		this.currentNum		= num;
	}
	
	public bool isActive()
	{
		return ( this.targetNum != this.currentNum ) ? true : false;
	}
}
