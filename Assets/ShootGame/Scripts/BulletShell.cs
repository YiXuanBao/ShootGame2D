using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//弹壳
public class BulletShell : MonoBehaviour
{
    public float speed;
    public float minAngle = -30f;
    public float maxAngle = 30f;
    private Rigidbody2D rb2D;

    void Awake()
    {
        rb2D = GetComponent<Rigidbody2D>();
    }

    private void OnEnable()
    {
        //随机向上抛落角度
        float angel = Random.Range(minAngle, maxAngle);
        rb2D.velocity = Quaternion.AngleAxis(angel, Vector3.forward) * Vector3.up * speed;
        //3倍重力，加速下落
        rb2D.gravityScale = 3;
    }
}
