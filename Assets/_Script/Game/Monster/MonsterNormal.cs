using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterNormal : MonoBehaviour {


    //巡逻模式的制作
    //方向的常量,上1，下2，左3，右4
    int[] dirs = new int[] { 1,2,3,4 };
    public int dir =2;
    Vector2 Monster_dir; 
    //移动的距离格数
    int[] GridCounts = new int[] {12,3,4,5,6,7,8,9,10 };
    int GridCount = 5;
    //怪物移动速度
    public float Monster_Speed = 0.5f;
    //角色的目标点
    private Vector2 TargetPos;
    //指定动画控制器
    private Animator animator;
    //设置碰撞层
    LayerMask mask = 1 << 15;
    //移动格子计数器
    public int num = 0;
    //怪物是否巡逻
    public bool IsPatrol = true;
    //动作触发机制需要制作，不然角色会回到默认状态再跳转到角色播放动画


    void Awake ()
    {
        //先随机一个方向和移动格子数

        RandomDirAndGrid();

        TargetPos = (Vector2)transform.position;

        animator = gameObject.GetComponent<Animator>();
    }
	
	
	void Update ()
    {
        Vector2 p = Vector2.MoveTowards( gameObject.transform.position, TargetPos, Monster_Speed/10);

        if (IsPatrol)
        {
            GetComponent<Rigidbody2D>().MovePosition(p);

            Displacement();
        }
        

        

    }
    /// <summary>
    /// 判断是否碰到阻挡
    /// </summary>
    /// <param name="dir"></param>
    /// <returns></returns>
    bool Valid_Monster(Vector2 dir)
    {
        //向角色前方发射一条射线dir/2的长度
        Vector2 pos = transform.position;

        RaycastHit2D hit = Physics2D.Raycast(pos, dir, 0.7f, mask);

        //判断是否投射到collider上
        if (hit)
        {
            Debug.Log("箱子发射光线碰撞" + hit.collider.gameObject.name);
        }

        return (hit);
    }
    /// <summary>
    /// 移动规则
    /// </summary>
    void Displacement()
    {
        if ( num > GridCount )
        {
            RandomDirAndGrid();
        }
        if ( Monster_dir == Vector2.up && TargetPos == (Vector2)transform.position )
        {
            if (Valid_Monster(Vector2.up) == false && num <= GridCount)//前方没碰撞就
            {
                animator.Play("run_up");

                TargetPos = (Vector2)transform.position + Monster_dir * 0.7f;

                num += 1;

            }
            else if (Valid_Monster(Vector2.up) && num <= GridCount)//有碰撞
            {
                RandomDirAndGrid();
                
            }
           
        }
        else if ( Monster_dir == -Vector2.up && TargetPos == (Vector2)transform.position )
        {
            if (Valid_Monster(-Vector2.up) == false && num <= GridCount)//前方没碰撞就
            {
                animator.Play("run_down");

                TargetPos = (Vector2)transform.position + Monster_dir * 0.7f;

                num += 1;

            }
            else if (Valid_Monster(-Vector2.up) && num <= GridCount)
            {
                RandomDirAndGrid();
            }
          
        }
        else if ( Monster_dir == Vector2.right && TargetPos == (Vector2)transform.position )
        {
            if (Valid_Monster(Vector2.right) == false && num <= GridCount )//前方没碰撞就
            {
                animator.Play("run_right");

                TargetPos = (Vector2)transform.position + Monster_dir;

               

                num += 1;

            }
            else if (Valid_Monster(Vector2.right) && num <= GridCount )//有碰撞
            {
                RandomDirAndGrid();  
            }
           
        }
        else if ( Monster_dir == -Vector2.right && TargetPos == (Vector2)transform.position )
        {
            if (Valid_Monster(-Vector2.right) == false && num <= GridCount)//前方没碰撞就
            {
                animator.Play("run_left");

                TargetPos = (Vector2)transform.position + Monster_dir;
                
                num += 1;

            }
            else if (Valid_Monster(-Vector2.right) && num <= GridCount)//有碰撞
            {
                RandomDirAndGrid(); 
            }
           
        }
        //给怪物刷新z轴位置
        gameObject.transform.position = new Vector3(gameObject.transform.position.x, gameObject.transform.position.y, gameObject.transform.position.y * 0.1F);
        //给方向右做一个镜像处理
        if ( dir == 4 )
        {
            transform.localScale = new Vector3(-1, 1, 1);
        }
        else
        {
            transform.localScale = new Vector3(1, 1, 1);
        }


    }
    void RandomDirAndGrid()
    {
        //随机给怪物一个方向
        dir = dirs[Random.Range(0, dirs.Length)];
        //随机给移动格子数
        GridCount = GridCounts[Random.Range(0, GridCounts.Length)];
        //计数归零
        num = 0;

        if (dir == 1 && Valid_Monster(Vector2.up) == false)
        {
            Monster_dir = Vector2.up;
        }
        if (dir == 2 && Valid_Monster(-Vector2.up) == false)
        {
            Monster_dir = -Vector2.up;
        }
        if (dir == 3 && Valid_Monster(-Vector2.right) == false)
        {
            Monster_dir = -Vector2.right;
        }
        if (dir == 4 && Valid_Monster(Vector2.right) == false)
        {
            Monster_dir = Vector2.right;
        }
        if ( Valid_Monster(Vector2.up) &&  Valid_Monster(-Vector2.up) && Valid_Monster(Vector2.right) && Valid_Monster(-Vector2.right) )
        {
            Stand_tra();
        }
    }
    void Stand_tra()
    {
        if (Monster_dir == Vector2.up)
        {
            animator.Play("stand_up");
        }
        else if (Monster_dir == -Vector2.up)
        {
            animator.Play("stand_down");
        }
        else if (Monster_dir == Vector2.right)
        {
            animator.Play("stand_right");
        }
        else if (Monster_dir == -Vector2.right)
        {
            animator.Play("stand_left");
        }
    }
 
}
