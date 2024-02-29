using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//敌人闲置状态
public class IdleState : IState
{
    private EnemyFSM manager;
    private Parameter parameter;

    private float timer;
    public IdleState(EnemyFSM manager)
    {
        this.manager = manager;
        this.parameter = manager.parameter;
    }
    public void OnEnter()
    {
        parameter.animator.Play("Idle");
    }

    public void OnUpdate()
    {
        timer += Time.deltaTime;

        if (parameter.getHit)
        {
            manager.TransitionState(StateType.Hit);
            return;
        }
        //追击范围内发现敌人，进入警戒状态
        if (parameter.target != null &&
            parameter.target.position.x >= parameter.chasePoints[0].position.x &&
            parameter.target.position.x <= parameter.chasePoints[1].position.x)
        {
            manager.TransitionState(StateType.React);
            return;
        }
        //超过闲置时间，进入巡逻状态
        if (timer >= parameter.idleTime)
        {
            manager.TransitionState(StateType.Patrol);
        }
    }

    public void OnExit()
    {
        timer = 0;
    }
}

//巡逻状态
public class PatrolState : IState
{
    private EnemyFSM manager;
    private Parameter parameter;

    private int patrolPosition;
    public PatrolState(EnemyFSM manager)
    {
        this.manager = manager;
        this.parameter = manager.parameter;
    }
    public void OnEnter()
    {
        parameter.animator.Play("Walk");
    }

    public void OnUpdate()
    {
        //朝向下一个巡逻点移动
        manager.FlipTo(parameter.patrolPoints[patrolPosition]);

        manager.transform.position = Vector2.MoveTowards(manager.transform.position,
            parameter.patrolPoints[patrolPosition].position, parameter.moveSpeed * Time.deltaTime);

        if (parameter.getHit)
        {
            manager.TransitionState(StateType.Hit);
            return;
        }
        //追击范围内发现目标，进入警戒状态
        if (parameter.target != null &&
            parameter.target.position.x >= parameter.chasePoints[0].position.x &&
            parameter.target.position.x <= parameter.chasePoints[1].position.x)
        {
            manager.TransitionState(StateType.React);
            return;
        }
        //走到了巡逻点，进入Idle状态
        if (Vector2.Distance(manager.transform.position, parameter.patrolPoints[patrolPosition].position) < .1f)
        {
            manager.TransitionState(StateType.Idle);
        }
    }

    //退出巡逻状态时，更新下一个巡逻点
    public void OnExit()
    {
        patrolPosition++;

        if (patrolPosition >= parameter.patrolPoints.Length)
        {
            patrolPosition = 0;
        }
    }
}

//追击状态
public class ChaseState : IState
{
    private EnemyFSM manager;
    private Parameter parameter;

    public ChaseState(EnemyFSM manager)
    {
        this.manager = manager;
        this.parameter = manager.parameter;
    }
    public void OnEnter()
    {
        parameter.animator.Play("Walk");
    }

    public void OnUpdate()
    {
        //朝向目标
        manager.FlipTo(parameter.target);
        //向目标移动
        if (parameter.target)
            manager.transform.position = Vector2.MoveTowards(manager.transform.position,
            parameter.target.position, parameter.chaseSpeed * Time.deltaTime);

        if (parameter.getHit)
        {
            manager.TransitionState(StateType.Hit);
            return;
        }
        //追击目标脱离范围则切换到闲置状态
        if (parameter.target == null ||
            manager.transform.position.x < parameter.chasePoints[0].position.x ||
            manager.transform.position.x > parameter.chasePoints[1].position.x)
        {
            manager.TransitionState(StateType.Idle);
        }
        //目标处于攻击范围，进入攻击状态
        if (Physics2D.OverlapCircle(parameter.attackPoint.position, parameter.attackArea, parameter.targetLayer))
        {
            manager.TransitionState(StateType.Attack);
        }
    }

    public void OnExit()
    {

    }
}

//发现玩家后的警戒状态
public class ReactState : IState
{
    private EnemyFSM manager;
    private Parameter parameter;

    private AnimatorStateInfo info;
    public ReactState(EnemyFSM manager)
    {
        this.manager = manager;
        this.parameter = manager.parameter;
    }
    public void OnEnter()
    {
        parameter.animator.Play("React");
    }

    public void OnUpdate()
    {
        info = parameter.animator.GetCurrentAnimatorStateInfo(0);

        if (parameter.getHit)
        {
            manager.TransitionState(StateType.Hit);
        }
        // 警戒动画播放完后进入追击状态
        if (info.normalizedTime >= .95f)
        {
            manager.TransitionState(StateType.Chase);
        }
    }

    public void OnExit()
    {

    }
}

//攻击状态
public class AttackState : IState
{
    private EnemyFSM manager;
    private Parameter parameter;

    private AnimatorStateInfo info;
    List<GameObject> attackPlayer = new List<GameObject>();

    public AttackState(EnemyFSM manager)
    {
        this.manager = manager;
        this.parameter = manager.parameter;
    }
    public void OnEnter()
    {
        parameter.animator.Play("Attack");
    }

    public void OnUpdate()
    {
        info = parameter.animator.GetCurrentAnimatorStateInfo(0);
        //受到攻击，切换到受击状态
        if (parameter.getHit)
        {
            manager.TransitionState(StateType.Hit);
        }
        //动画播放完毕后，切换到追击状态
        if (info.normalizedTime >= .95f)
        {
            manager.TransitionState(StateType.Chase);
        }
        //动画在指定时间内（挥动斧子的时候），判断玩家是否在攻击范围内，是的话，广播敌人攻击事件
        if (info.normalizedTime >= .1 && info.normalizedTime <= .7)
        {
            var target = Physics2D.OverlapCircle(parameter.attackPoint.position, parameter.attackArea, parameter.targetLayer);
            if (target)
            {
                //判断是否重复攻击，一个玩家只受到一次攻击
                if (!attackPlayer.Contains(target.gameObject))
                {
                    attackPlayer.Add(target.gameObject);
                    EventCenter.Broadcast(MyEventType.EnemyAttack);
                }
            }
        }
    }

    public void OnExit()
    {
        attackPlayer.Clear();
    }
}

//受击状态
public class HitState : IState
{
    private EnemyFSM manager;
    private Parameter parameter;

    private AnimatorStateInfo info;
    public HitState(EnemyFSM manager)
    {
        this.manager = manager;
        this.parameter = manager.parameter;
    }
    public void OnEnter()
    {
        //播放受击动画
        parameter.animator.Play("Hit");
        parameter.getHit = false;
    }

    public void OnUpdate()
    {
        info = parameter.animator.GetCurrentAnimatorStateInfo(0);

        //判断死亡
        if (parameter.health <= 0)
        {
            manager.TransitionState(StateType.Death);
        }
        //未死亡，受击动画播放结束，进入到Chase状态
        if (info.normalizedTime >= .95f)
        {
            parameter.target = GameObject.FindWithTag("Player").transform;
            manager.TransitionState(StateType.Chase);
        }
    }

    public void OnExit()
    {

    }
}

//敌人死亡状态
public class DeathState : IState
{
    private EnemyFSM manager;
    private Parameter parameter;

    public DeathState(EnemyFSM manager)
    {
        this.manager = manager;
        this.parameter = manager.parameter;
    }
    public void OnEnter()
    {
        parameter.animator.Play("Dead");
        manager.Death();
    }

    public void OnUpdate()
    {

    }

    public void OnExit()
    {

    }
}