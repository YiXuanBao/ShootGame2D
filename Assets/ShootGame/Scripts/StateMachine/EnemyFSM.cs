using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//枚举敌人状态类型 
public enum StateType
{
    Idle, Patrol, Chase, React, Attack, Hit, Death
}

//敌人参数类
[Serializable]
public class Parameter
{
    public int health;
    public float moveSpeed;
    public float chaseSpeed;
    public float idleTime;
    public Transform[] patrolPoints;//巡逻点
    public Transform[] chasePoints; //追逐范围点
    public Transform target; //目标
    public LayerMask targetLayer; //目标层级
    public Transform attackPoint;
    public float attackArea; //攻击范围
    public Animator animator; //敌人动画控制器
    public bool getHit; //是否受击
}

//敌人FSM状态机
public class EnemyFSM : MonoBehaviour
{
    private IState currentState;
    private Dictionary<StateType, IState> states = new Dictionary<StateType, IState>();

    public Parameter parameter;

    void Start()
    {
        states.Add(StateType.Idle, new IdleState(this));
        states.Add(StateType.Patrol, new PatrolState(this));
        states.Add(StateType.Chase, new ChaseState(this));
        states.Add(StateType.React, new ReactState(this));
        states.Add(StateType.Attack, new AttackState(this));
        states.Add(StateType.Hit, new HitState(this));
        states.Add(StateType.Death, new DeathState(this));

        //默认进入闲置状态
        TransitionState(StateType.Idle);

        parameter.animator = transform.GetComponent<Animator>();
    }

    void Update()
    {
        //更新当前状态事件
        currentState.OnUpdate();
    }

    //状态切换
    public void TransitionState(StateType type)
    {
        //当前状态执行OnExit方法
        if (currentState != null)
            currentState.OnExit();
        //设置当前状态
        currentState = states[type];
        //新状态执行OnEnter方法
        currentState.OnEnter();
    }

    //如果有目标，则朝向目标
    public void FlipTo(Transform target)
    {
        if (target != null)
        {
            // 目标在左边
            if (transform.position.x > target.position.x)
            {
                transform.localScale = new Vector3(-1, 1, 1);
            }
            //目标在右边
            else if (transform.position.x < target.position.x)
            {
                transform.localScale = new Vector3(1, 1, 1);
            }
        }
    }

    //死亡处理
    public void Death()
    {
        //禁用碰撞相关组件
        GetComponent<Collider2D>().enabled = false;
        GetComponent<Rigidbody2D>().gravityScale = 0f;
    }

    //触发器进入
    private void OnTriggerEnter2D(Collider2D other)
    {
        //碰到了玩家
        if (other.CompareTag("Player"))
        {
            //把玩家当做目标
            parameter.target = other.transform;
        }
        //碰到了子弹
        else if (other.CompareTag("Bullet"))
        {
            var bullet = other.GetComponent<Bullet>();
            parameter.health -= bullet.damage;
            parameter.getHit = true;
            EventCenter.Broadcast<int>(MyEventType.GameSleep, bullet.hitSleepFrame);
        }
    }

    //触发器退出，结束触发
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            //目标设置为空
            parameter.target = null;
        }
    }

    //画出攻击区域
    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(parameter.attackPoint.position, parameter.attackArea);
    }
}