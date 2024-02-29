using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleEnemySpawner : MonoBehaviour
{
    public GameObject enemyPrefab;
    public Transform[] patrolPoints;
    public Transform[] chasePoints;
    public Transform spawnPos;
    public float spawnInterval = 1f;
    public int maxEnemyCount = 5;
    List<GameObject> enemyList = new List<GameObject>();

    private void OnEnable()
    {
        //监听游戏开始和结束事件
        EventCenter.AddListener(MyEventType.GameStart, OnGameStart);
        EventCenter.AddListener(MyEventType.GameEnd, OnGameEnd);
    }

    private void OnDisable()
    {
        EventCenter.RemoveListener(MyEventType.GameStart, OnGameStart);
        EventCenter.RemoveListener(MyEventType.GameEnd, OnGameEnd);
    }

    void OnGameStart()
    {
        //游戏开始，清除场上已有敌人
        //重新生成敌人
        ClearEnemy();
        StartCoroutine(SpawnEnemy());
    }

    void OnGameEnd()
    {
        //停止生成敌人
        StopAllCoroutines();
    }

    void ClearEnemy()
    {
        foreach (var go in enemyList)
        {
            Destroy(go);
        }
        enemyList.Clear();
    }

    IEnumerator SpawnEnemy()
    {
        for (int i = 0; i < maxEnemyCount; i++)
        {
            var go = Instantiate(enemyPrefab, transform);
            go.transform.position = spawnPos.position;
            enemyList.Add(go);
            var fsm = go.GetComponent<EnemyFSM>();
            //初始化敌人巡逻范围和追击范围
            fsm.parameter.chasePoints = this.chasePoints;
            fsm.parameter.patrolPoints = this.patrolPoints;
            yield return new WaitForSeconds(spawnInterval);
        }
    }
}
