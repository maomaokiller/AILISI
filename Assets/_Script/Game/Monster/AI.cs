using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;
using Pathfinding.Examples;

public class AI : MonoBehaviour
{
    //下一帧的位置
    public Vector2 nexPos;
   
    //怪物移动速度
    public float Monster_speed = 2.5f;

    // 怪物的当前位置
    public Vector2 curPos;

    //方向为上1，下2，左3， 右4，

    public int dir = 2;
    //动作为1站，2跑，3攻，4死
    public int Act = 1;

    //动画播放驱动为ActDir =Act*10 +dir;死亡时加上ActDir =Act*100+dir*10+1or2（x=1 y=2）
    public int ActDir;

    
    //用于AI碰撞
    GameObject TishengA;
    //用于死亡检测
    GameObject TishengB;


    public Animator animator;
    //死亡方向判断
    int dieDir;
    //撞击怪物的箱子
    public GameObject Die_candy;
    //撞击箱子的移动方向
    public Vector2 Die_candy_dir;

    //死亡计数

    int isA = 0;
    
    

    void Awake()
    {
        

        TishengA = new GameObject();

        TishengB = new GameObject();
        //把替身放置到怪物位置
        TishengA.transform.position = transform.position;
        TishengB.transform.position = transform.position;
        TishengA.name = "A";
        TishengB.name = "B";
        TishengA.transform.parent = transform;
        TishengB.transform.parent = transform;
        //Tisheng.transform.parent = transform;
        //为替身添加寻路部件
        AILerp ailerp = TishengA.AddComponent<AILerp>();
        ailerp.speed = Monster_speed;
        ailerp.enableRotation = true;
        ailerp.rotationIn2D = true;
        ailerp.rotationSpeed = 3;
        //为替身添加碰撞脚本
        TishengA.AddComponent<MonsterTiAI>();
        TishengB.AddComponent<MonsterTiSheng>();

        animator = gameObject.GetComponent<Animator>();
        

    }
    void Start()
    {
        
        Act = 1;

        dir = 2;
    }
    
  

    void Update()
    {
        //让怪物使用替身A的移动
        if (TishengA != null)
        {
            transform.position = new Vector3(TishengA.transform.position.x, TishengA.transform.position.y, TishengA.transform.position.y * 0.1f);
        }

        if (Die_candy != null )
        {
            isA++;
            if (isA==1)
            {
                Debug.Log("我被击败了！");
                //TishengA.GetComponentInChildren<AILerp>().canMove = false;
                Destroy(TishengA);
                Destroy(TishengB);
                transform.parent = Die_candy.transform;
                IsDie();
            }
            
        }
        else if ( Die_candy == null)
        {
            AnimationPlayer();
        }

        curPos = gameObject.transform.position;
       
        //Invoke("Zdingwei",1.5f);
    }
   
    /// <summary>
    /// 怪物活着的情况下的动画播放机制
    /// </summary>
    void AnimationPlayer()
    {
        Animator animator = gameObject.GetComponent<Animator>();

        Vector2 Pos = TishengA.GetComponent<AILerp>().DirPos.normalized;
        
        if (nexPos == curPos)
        {
            Act = 1;

            ActDir = Act * 10 + dir;

            animator.SetInteger("ActDir", ActDir);

        }
        else
        {
            if (Pos == Vector2.up)
            {
                Act = 2;

                dir = 1;

                ActDir = Act * 10 + dir;

                animator.SetInteger("ActDir", ActDir);
            }
            if (Pos == -Vector2.up)
            {
                Act = 2;

                dir = 2;

                ActDir = Act * 10 + dir;

                animator.SetInteger("ActDir", ActDir);

            }
            if (Pos == Vector2.right)
            {
                Act = 2;

                dir = 4;

                ActDir = Act * 10 + dir;

                animator.SetInteger("ActDir", ActDir);

            }
            if (Pos == -Vector2.right)
            {
                Act = 2;

                dir = 3;

                ActDir = Act * 10 + dir;

                animator.SetInteger("ActDir", ActDir);

            }
            if (dir == 4)
            {
                transform.localScale = new Vector3(-1, 1, 1);
            }
            else
            {
                transform.localScale = new Vector3(1, 1, 1);
            }
        }

        //Debug.Log("怪物动画播放速度" + animator.speed);
    }
    /// <summary>
    /// 处理角色前后遮挡关系
    /// 这里距离有问题，这个时间不可能移动这么远所以要处理
    /// </summary>
    void Zdingwei()
    {
        gameObject.transform.position = new Vector3(gameObject.transform.position.x, gameObject.transform.position.y, gameObject.transform.position.y * 0.1F );
    }
    /// <summary>
    /// 死亡时需要播放的动作
    /// </summary>
    void IsDie( )
    {
        if (Die_candy_dir == Vector2.up || Die_candy_dir == Vector2.down)
        {
            dieDir = 2;
        }
        else if (Die_candy_dir == Vector2.right || Die_candy_dir == Vector2.left)
        {
            dieDir = 1;
        }

        Act = 4;

        ActDir = Act * 100 + dir * 10 + dieDir;

        animator.SetInteger("ActDir", ActDir);

    }
}

