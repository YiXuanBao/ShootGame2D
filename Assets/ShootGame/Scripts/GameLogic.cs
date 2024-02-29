using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//游戏整体逻辑出路类
public class GameLogic : MonoBehaviour
{
    public GameObject playerPrefab; //玩家预制体
    public Transform playerSpawnPos; //玩家生成位置

    GameObject player;
    Coroutine sleepCor;

    private void Update()
    {
        //按K开始游戏
        if (Input.GetKeyDown(KeyCode.K))
        {
            StartGame();
        }
    }

    private void OnEnable()
    {
        //监听游戏暂停事件
        EventCenter.AddListener<int>(MyEventType.GameSleep, OnGameSleep);
    }

    private void OnDisable()
    {
        EventCenter.RemoveListener<int>(MyEventType.GameSleep, OnGameSleep);
    }

    void OnGameSleep(int frameCount)
    {
        if (sleepCor != null)
            StopCoroutine(sleepCor);
        sleepCor = StartCoroutine(GameSleep(frameCount));
    }

    //游戏暂停指定帧数，达到顿帧效果
    IEnumerator GameSleep(int frameCount)
    {
        Time.timeScale = 0;
        for (int i = 0; i < frameCount; i++)
        {
            yield return null;
        }
        Time.timeScale = 1;
    }

    //开始游戏
    void StartGame()
    {
        if (player != null)
            Destroy(player);
        player = Instantiate(playerPrefab);
        player.transform.position = playerSpawnPos.position;
        //广播游戏开始事件
        EventCenter.Broadcast(MyEventType.GameStart);
    }
}
