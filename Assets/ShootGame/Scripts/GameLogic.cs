using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//��Ϸ�����߼���·��
public class GameLogic : MonoBehaviour
{
    public GameObject playerPrefab; //���Ԥ����
    public Transform playerSpawnPos; //�������λ��

    GameObject player;
    Coroutine sleepCor;

    private void Update()
    {
        //��K��ʼ��Ϸ
        if (Input.GetKeyDown(KeyCode.K))
        {
            StartGame();
        }
    }

    private void OnEnable()
    {
        //������Ϸ��ͣ�¼�
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

    //��Ϸ��ָͣ��֡�����ﵽ��֡Ч��
    IEnumerator GameSleep(int frameCount)
    {
        Time.timeScale = 0;
        for (int i = 0; i < frameCount; i++)
        {
            yield return null;
        }
        Time.timeScale = 1;
    }

    //��ʼ��Ϸ
    void StartGame()
    {
        if (player != null)
            Destroy(player);
        player = Instantiate(playerPrefab);
        player.transform.position = playerSpawnPos.position;
        //�㲥��Ϸ��ʼ�¼�
        EventCenter.Broadcast(MyEventType.GameStart);
    }
}
