using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Threading;


public class ChangeSceneManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ChangeGameScene()
    {     
        Time.timeScale = 1.0f;
        DontDestroyOnLoad(GameObject.Find("Sounds"));
        GameObject.Find("ButtonSound").GetComponent<AudioSource>().Play();

        SceneManager.LoadScene("MainGame");
    }


}
