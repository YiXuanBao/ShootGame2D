using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//爆炸特效类
public class Explosion : MonoBehaviour
{
    private Animator animator;
    private AnimatorStateInfo info;

    void Awake()
    {
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        info = animator.GetCurrentAnimatorStateInfo(0);
        //爆炸动画播放完毕后，对象池回收此特效
        if (info.normalizedTime >= 1)
        {
            // Destroy(gameObject);
            ObjectPool.Instance.PushObject(gameObject);
        }
    }
}
