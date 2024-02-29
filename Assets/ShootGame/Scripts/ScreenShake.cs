using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//抖屏功能实现
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
            //渲染前根据抖屏强度，相机产生偏移
            lastOffset = Random.insideUnitCircle * intensity;
            transform.localPosition = transform.localPosition + lastOffset;
        }
    }

    private void OnPostRender()
    {
        if (stopTimer > 0.0f)
        {
            //渲染完成后，相机回到原来位置
            transform.localPosition = transform.localPosition - lastOffset;
            stopTimer -= Time.deltaTime;
        }
    }

    //抖屏，time 持续时间， intensity 强度
    void Shake(float time, float intensity)
    {
        stopTimer = time;
        this.intensity = intensity;
    }
}
