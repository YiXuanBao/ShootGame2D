using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//��������ʵ��
[RequireComponent(typeof(Camera))]
public class ScreenShake : MonoBehaviour
{
    float stopTimer;
    float intensity;
    Vector3 lastOffset;

    private void OnEnable()
    {
        EventCenter.AddListener<float,float>(MyEventType.ScreenShake, Shake);
    }

    private void OnDisable()
    {
        EventCenter.RemoveListener<float, float>(MyEventType.ScreenShake, Shake);
    }

    private void OnPreRender()
    {
        if (stopTimer > 0.0f)
        {
            //��Ⱦǰ���ݶ���ǿ�ȣ��������ƫ��
            lastOffset = Random.insideUnitCircle * intensity;
            transform.localPosition = transform.localPosition + lastOffset;
        }
    }

    private void OnPostRender()
    {
        if (stopTimer > 0.0f)
        {
            //��Ⱦ��ɺ�����ص�ԭ��λ��
            transform.localPosition = transform.localPosition - lastOffset;
            stopTimer -= Time.deltaTime;
        }
    }

    //������time ����ʱ�䣬 intensity ǿ��
    void Shake(float time, float intensity)
    {
        stopTimer = time;
        this.intensity = intensity;
    }
}
