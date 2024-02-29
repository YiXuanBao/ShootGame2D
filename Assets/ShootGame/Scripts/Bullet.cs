using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//子弹类
public class Bullet : MonoBehaviour
{
    [Header("打中敌人时暂停多少帧")]
    public int hitSleepFrame = 5;
    public int damage = 1; //子弹伤害
    public float speed; //速度
    public GameObject explosionPrefab;// 子弹爆炸特效
    private Rigidbody2D rb2D;

    void Awake()
    {
        rb2D = GetComponent<Rigidbody2D>();
    }

    public void SetDirection(Vector2 direction)
    {
        rb2D.velocity = direction * speed;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        //碰到物体时，从对象池生成一个爆炸特效
        GameObject exp = ObjectPool.Instance.GetObject(explosionPrefab);
        exp.transform.position = transform.position;
        ObjectPool.Instance.PushObject(gameObject);
    }
}
