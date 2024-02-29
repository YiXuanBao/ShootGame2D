using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class GunInfo
{
    [Header("开枪间隔")]
    public float interval;
    [Header("后坐力")]
    public float knockback;
    [Header("持续射击多久晃动,0表示不晃动")]
    [Range(0, 2)]
    public float shakeTime;
    [Header("子弹偏移最大角度")]
    public float maxOffetAngle;
    [Header("子弹随机最小角度")]
    public float minOffetAngle;
    [Header("持续射击时屏幕单次晃动时间")]
    public float screenShakeTime = 0.1f;
    [Header("持续射击时屏幕晃动强度")]
    public float screenShakeIntensity = 1f;
    [Header("GunDelay跟随速度")]
    public float gunDelaySmooth = 10f;
    public GameObject bulletPrefab;
    public GameObject shellPrefab;
}

//枪
public class Gun : MonoBehaviour
{
    [SerializeField]
    protected GunInfo info;
    protected Transform muzzlePos; // 枪口位置
    protected Transform shellPos; // 弹壳位置
    protected Vector2 direction;
    protected float timer;
    protected Animator animator;
    protected float fireTime; // 持续射击时间
    [SerializeField]
    protected Transform followTarget;
    protected Vector3 targetPosition;

    // 射击方向，枪的信息
    public UnityAction<Vector2, GunInfo> OnShoot { get; set; }

    protected virtual void Start()
    {
        animator = GetComponent<Animator>();
        muzzlePos = transform.Find("Muzzle");
        shellPos = transform.Find("BulletShell");
        transform.parent = null;
    }

    protected virtual void Update()
    {
        GunDelay();
        Shoot();
    }

    //人物下落时，枪在y轴延迟跟随目标
    void GunDelay()
    {
        if (followTarget != null)
        {
            targetPosition.x = followTarget.position.x;
            if (transform.position.y > followTarget.position.y)
                targetPosition.y = transform.position.y + (followTarget.position.y - transform.position.y) * info.gunDelaySmooth * Time.deltaTime;
            else
                targetPosition.y = followTarget.position.y;
            targetPosition.z = followTarget.position.z;
            transform.position = targetPosition;
            transform.rotation = followTarget.rotation;
        }
    }

    //射击
    protected virtual void Shoot()
    {
        direction = transform.right;

        //射击间隔
        if (timer != 0)
        {
            timer -= Time.deltaTime;
            if (timer <= 0)
                timer = 0;
        }

        if (Input.GetButton("Fire1"))
        {
            //记录持续射击时间
            fireTime += Time.deltaTime;
            if (timer == 0)
            {
                timer = info.interval;
                Fire();
            }
        }
        else
        {
            fireTime = 0;
        }

        //持续射击时间达到info.shakeTime后，广播抖屏事件
        if (info.shakeTime > 0 && fireTime >= info.shakeTime)
        {
            EventCenter.Broadcast<float, float>(MyEventType.ScreenShake, info.screenShakeTime, info.screenShakeIntensity);
        }
    }

    //射击
    protected virtual void Fire()
    {
        //从对象池生成子弹
        GameObject bullet = ObjectPool.Instance.GetObject(info.bulletPrefab);
        bullet.transform.position = muzzlePos.position;
        
        //设置随机偏移角度
        float angle = Random.Range(info.minOffetAngle, info.maxOffetAngle);
        bullet.GetComponent<Bullet>().SetDirection(Quaternion.AngleAxis(angle, Vector3.forward) * direction);

        //从对象池生成弹壳
        GameObject shell = ObjectPool.Instance.GetObject(info.shellPrefab);
        shell.transform.position = shellPos.position;
        shell.transform.rotation = shellPos.rotation;

        //广播射击事件
        EventCenter.Broadcast<Vector2, GunInfo>(MyEventType.OnGunShoot, direction, info);
    }
}
