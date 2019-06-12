using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyGeneratorCtrl : MonoBehaviour
{
    public GameObject enemyPrefab;
    GameObject[] existEnemys;
    public int maxEnemy = 2;

    // Start is called before the first frame update
    void Start()
    {
        existEnemys = new GameObject[maxEnemy];
        StartCoroutine(Exec());
    }

    IEnumerator Exec()
    {
        while(true)
        {
            Generate();
            yield return new WaitForSeconds(3.0f);
        }
    }

    void Generate()
    {
        for (int enemyCount = 0; enemyCount < existEnemys.Length; ++enemyCount)
        {
            if (existEnemys[enemyCount] == null)
            {
                //적 생성
                existEnemys[enemyCount] = Instantiate(enemyPrefab, transform.position, transform.rotation) as GameObject;
                return;
            }

        }
    }
}
