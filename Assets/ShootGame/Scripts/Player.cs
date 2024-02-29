using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//玩家类
public class Player : MonoBehaviour
{
    public int maxHp = 3;
    public float speed = 5f;
    public float jumpForce = 5f;
    public float jumpMultipler = 1.5f;
    public float fallMultipler = 2.5f;
    public float boxHeight = 0.5f;
    public float knockbackOnHit = 0.5f;
    public LayerMask groundMask;
    public Gun[] guns;
    public Transform cameraFollowTarget;

    SpriteRenderer spriteRenderer;
    Animator anim;
    Rigidbody2D rb2D;
    //Cinemachine.CinemachineCollisionImpulseSource impulse;

    Vector2 playerSize;
    Vector2 boxSize;

    [SerializeField]
    bool isLive = false;
    [SerializeField]
    bool isJump = false;
    [SerializeField]
    bool isGround = false;
    [SerializeField]
    bool jumpPress = false;
    [SerializeField]
    int hp;
    bool isAlive = true;
    int curGunIndex;

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        rb2D = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        playerSize = spriteRenderer.bounds.size;
        boxSize = new Vector2(playerSize.x * 0.4f, boxHeight);
        var cameraFollow = Camera.main.GetComponent<CameraFollow>();
        if (cameraFollow == null)
            cameraFollow = Camera.main.gameObject.AddComponent<CameraFollow>();
        //设置相机跟随位置
        cameraFollow.SetTarget(cameraFollowTarget);
        hp = maxHp;
        curGunIndex = 0;
        EventCenter.Broadcast<GameObject>(MyEventType.PlayerAlive, gameObject);
    }

    private void OnEnable()
    {
        //监听射击事件和敌人攻击事件
        EventCenter.AddListener<Vector2, GunInfo>(MyEventType.OnGunShoot, OnShoot);
        EventCenter.AddListener(MyEventType.EnemyAttack, OnEnemyAttack);
    }

    private void OnDisable()
    {
        EventCenter.RemoveListener<Vector2, GunInfo>(MyEventType.OnGunShoot, OnShoot);
        EventCenter.RemoveListener(MyEventType.EnemyAttack, OnEnemyAttack);
    }

    // Update is called once per frame
    void Update()
    {
        if (!isAlive) return;
        if (isGround && Input.GetButtonDown("Jump"))
        {
            jumpPress = true;
        }
    }

    private void FixedUpdate()
    {
        if (!isAlive) return;
        //检测是否在地面
        Vector2 boxCenter = (Vector2)transform.position + Vector2.down * playerSize.y * 0.5f;
        isGround = Physics2D.OverlapBox(boxCenter, boxSize, 0, groundMask);

        GroundMove();
        Jump();
        SwitchAnim();
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = isGround ? Color.red : Color.green;
        Vector2 boxCenter = (Vector2)transform.position + Vector2.down * playerSize.y * 0.5f;
        Gizmos.DrawWireCube(boxCenter, boxSize);
    }

    //移动
    void GroundMove()
    {
        float h = Input.GetAxisRaw("Horizontal");
        //if (!Physics2D.OverlapCircle(frontCheck.position, 0.1f, groundMask))
        //{
        rb2D.velocity = new Vector2(h * speed, rb2D.velocity.y);
        //}

        if (h < 0)
        {
            var angle = transform.localEulerAngles;
            angle.y = 180;
            transform.localEulerAngles = angle;
        }
        else if (h > 0)
        {
            var angle = transform.localEulerAngles;
            angle.y = 0;
            transform.localEulerAngles = angle;
        }
    }

    void Jump()
    {
        //在地面时，跳跃状态为false
        if (isGround)
        {
            isJump = false;
        }

        //按下跳跃且在地面时，跳跃
        if (jumpPress && isGround)
        {
            isJump = true;
            rb2D.velocity = new Vector2(rb2D.velocity.x, jumpForce);
            jumpPress = false;
        }

        // 往上跳时，松开空格，重力加倍，达到长按空格跳的更高的效果
        if (rb2D.velocity.y > 0 && !Input.GetButton("Jump"))
        {
            rb2D.gravityScale = jumpMultipler;
        }
        // 下落时，重力加倍，达到快速下落效果
        else if (rb2D.velocity.y < 0)
        {
            rb2D.gravityScale = fallMultipler;
        }
        else
        {
            //落地时，重力正常
            rb2D.gravityScale = 1f;
        }
    }

    void SwitchAnim()
    {
        //为动画设置参数，动画控制器会根据参数切换动画
        anim.SetFloat("run", Mathf.Abs(rb2D.velocity.x));
        //落地情况
        if (isGround)
        {
            anim.SetBool("fall", false);
            anim.SetBool("jump", false);
        }
        //往上跳的情况
        else if (!isGround && rb2D.velocity.y > 0)
        {
            anim.SetBool("jump", true);
            anim.SetBool("fall", false);
        }
        //空中下落情况
        else if (!isGround && rb2D.velocity.y < 0)
        {
            anim.SetBool("jump", false);
            anim.SetBool("fall", true);
        }
    }

    void OnShoot(Vector2 dir, GunInfo gunInfo)
    {
        // 射击后坐力体现
        rb2D.position += (-dir * gunInfo.knockback * Time.deltaTime);
    }

    void OnEnemyAttack()
    {
        //被敌人攻击
        if (hp <= 0) return;
        hp--;
        if (hp <= 0)
        {
            Death();
        }
        else
        {
            anim.SetTrigger("hit");
            rb2D.position += -(Vector2)transform.right * knockbackOnHit;
        }
    }

    void Death()
    {
        //死亡
        isAlive = false;
        anim.SetTrigger("death");
        EventCenter.Broadcast<GameObject>(MyEventType.PlayerDeath, gameObject);
    }

    //死亡动画中调用此方法
    public void DestroySelf()
    {
        Destroy(gameObject);
    }

    private void OnDestroy()
    {
        foreach (var gun in guns)
        {
            if (gun.gameObject != null)
                Destroy(gun.gameObject);
        }
    }
}
