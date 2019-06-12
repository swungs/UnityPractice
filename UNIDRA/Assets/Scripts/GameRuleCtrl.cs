using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class GameRuleCtrl : MonoBehaviour
{

    //남은 시간
    public float timeRemaining = 5.0f * 60.0f;

    //게임 오버 플래그
    public bool gameOver = false;

    //게임 클리어 플래그
    public bool gameClear = false;

    //씬 이행 시간
    public float sceneChangeTime = 3.0f;

    public AudioClip clearSeClip;
    AudioSource clearSeAudio;


    // Start is called before the first frame update
    void Start()
    {
        //오디오 초기화
        clearSeAudio = gameObject.AddComponent<AudioSource>();
        clearSeAudio.clip = clearSeClip;
        clearSeAudio.loop = false;

    }

    // Update is called once per frame
    void Update()
    {
        if(gameOver || gameClear)
        {
            sceneChangeTime -= Time.deltaTime;
            if(sceneChangeTime <=0.0f)
            {
                //타이틀씬으로 전환
                SceneManager.LoadScene("TitleScene");
            }
            return;
        }
        timeRemaining -= Time.deltaTime;
        
        if(timeRemaining <= 0.0f)
        {
            GameOver();
        }
    }

    public void GameOver()
    {
        gameOver = true;
        Debug.Log("GameOver");
    }

    public void GameClear()
    {
        gameClear = true;
        clearSeAudio.Play();
        Debug.Log("GameClear");
    }
}
