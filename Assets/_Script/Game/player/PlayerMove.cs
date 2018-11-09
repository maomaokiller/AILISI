using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlayerMove : MonoBehaviour
{
    //角色移动速度
    public float PlayerSpeed = 1f;
    //方向X，动作为Y 
    int a ;
    int b ;
    //移动方向为1上2下3左4右  (a*10 + b)
    int DirID; 

    //角色目标点（这里要取地图离角色最近的节点做处理）

    private Vector2 TargetPos = Vector2.zero;

    //指定动画控制器
    private Animator animator;
    //被推的箱子
    public GameObject _Candy_box;

    public bool Dead;


    void Awake()
    {
        Dead = false;
        Collider2D collider = gameObject.AddComponent<CircleCollider2D>();
        collider.isTrigger = true;
        GetComponent<CircleCollider2D>().radius = 0.3f;
        gameObject.layer = 16;
        //找到游戏主摄像机
        //GameObject Camera = GameObject.Find("Main Camera");       
        TargetPos = transform.position;
        //初始状态方向a：1-上  2-下  3-左  4-右
        //初始状态动作b：1-站  2-跑  3-推  4-死 5笑
        a = 2;
        b = 1;


        
    }
    void Start()
    {
        //找到第一个子物体
        var child = transform.GetChild(0);
        //初始化动画组件
        animator = child.gameObject.GetComponent<Animator>();

        
    }

    void Update()
    {

        Vector2 p = Vector2.MoveTowards(transform.position, TargetPos ,PlayerSpeed/10);


        GetComponent<Rigidbody2D>().MovePosition(p);

        if (Dead)
        {
            b = 4;
            if (a == 1)
            {
                animator.Play("die_up");
            }
            else if (a == 2)
            {
                animator.Play("die_down");
            }
            else if (a == 3)
            {
                animator.Play("die_left");
            }
            else if ( a == 4 )
            {
                animator.Play("die_right");
            }
           

            DirID = b * 10 + a;

            animator.SetInteger("DirID", DirID );

            Invoke("DeadManager",1.5f);

        }
        else
        {
            if ((Vector2)transform.position == TargetPos)
            {

                AnimatorStateInfo info = animator.GetCurrentAnimatorStateInfo(0);


                if (Input.GetKeyDown(KeyCode.K) && !info.IsTag("attack_all"))
                {
                    //Debug.Log(" 我要攻击了 ");

                    //动作为3
                    b = 3;
                    //这里有点坑如果不用这个写法调取不到层
                    int layerMask = 1 << 15 | 1 << 14;

                    if (a == 1)
                    {
                        animator.Play("attack_up");

                        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.up, 1f, layerMask);

                        if (hit.collider != null && hit.collider.gameObject.tag == "candy_layer")
                        {
                            //Debug.Log("向上推箱子");
                            hit.collider.gameObject.GetComponent<Candy_box>().CandyMove = true;
                            hit.collider.gameObject.GetComponent<Candy_box>().Ispush = true;
                            hit.collider.gameObject.GetComponent<Candy_box>().candy_box_dir = Vector2.up;
                            //Debug.Log("方向指向上方");
                            hit.collider.gameObject.GetComponent<Candy_box>().candy_old_pos = hit.collider.transform.position;
                            //Debug.Log(hit.collider.gameObject.name + "初始位置为" + hit.collider.gameObject.GetComponent<Candy_box>().candy_old_pos);

                        }

                    }
                    if (a == 2)
                    {
                        animator.Play("attack_down");

                        RaycastHit2D hit = Physics2D.Raycast(transform.position, -Vector2.up, 1f, layerMask);

                        if (hit.collider != null && hit.collider.gameObject.tag == "candy_layer")
                        {
                            //Debug.Log("向下推箱子");
                            hit.collider.gameObject.GetComponent<Candy_box>().CandyMove = true;
                            hit.collider.gameObject.GetComponent<Candy_box>().Ispush = true;
                            hit.collider.gameObject.GetComponent<Candy_box>().candy_box_dir = -Vector2.up;
                            hit.collider.gameObject.GetComponent<Candy_box>().candy_old_pos = hit.collider.transform.position;
                            //Debug.Log(hit.collider.gameObject.name+"被推的box是初始位置" + hit.collider.transform.position);

                        }
                    }
                    if (a == 3)
                    {
                        animator.Play("attack_left");

                        RaycastHit2D hit = Physics2D.Raycast(transform.position, -Vector2.right, 1f, layerMask);


                        if (hit.collider != null && hit.collider.gameObject.tag == "candy_layer")
                        {
                            //Debug.Log("向左推箱子");
                            hit.collider.gameObject.GetComponent<Candy_box>().CandyMove = true;
                            hit.collider.gameObject.GetComponent<Candy_box>().Ispush = true;
                            hit.collider.gameObject.GetComponent<Candy_box>().candy_box_dir = -Vector2.right;
                            hit.collider.gameObject.GetComponent<Candy_box>().candy_old_pos = hit.collider.transform.position;
                            //Debug.Log( hit.collider.gameObject.name + "初始位置为" + hit.collider.gameObject.GetComponent<Candy_box>().candy_old_pos);

                        }
                    }
                    if (a == 4)
                    {
                        animator.Play("attack_right");

                        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.right, 1f, layerMask);

                        if (hit.collider != null && hit.collider.gameObject.tag == "candy_layer")
                        {
                            //Debug.Log("向右推箱子");

                            hit.collider.gameObject.GetComponent<Candy_box>().CandyMove = true;
                            hit.collider.gameObject.GetComponent<Candy_box>().Ispush = true;
                            hit.collider.gameObject.GetComponent<Candy_box>().candy_box_dir = Vector2.right;
                            hit.collider.gameObject.GetComponent<Candy_box>().candy_old_pos = hit.collider.transform.position;
                            //Debug.Log(hit.collider.gameObject.name + "初始位置为" + hit.collider.gameObject.GetComponent<Candy_box>().candy_old_pos);

                        }
                    }



                    //info.normalizedTime >= 0.6f
                }
                else if (Input.GetKey(KeyCode.W))
                {
                    //Debug.Log(" 我要上 ");
                    //动作为2
                    a = 1;
                    b = 2;
                    animator.Play("run_up");


                    if (!Valid(Vector2.up))
                    {
                        TargetPos = (Vector2)transform.position + Vector2.up * 0.7f;
                    }

                }
                else if (Input.GetKey(KeyCode.D))
                {
                    //Debug.Log(" 我要右 ");
                    //动作为2
                    a = 4;
                    b = 2;
                    animator.Play("run_right");


                    if (!Valid(Vector2.right))
                    {
                        TargetPos = (Vector2)transform.position + Vector2.right * 1f;
                    }

                }
                else if (Input.GetKey(KeyCode.S))
                {
                    //Debug.Log(" 我要下 ");
                    //动作为2
                    a = 2;
                    b = 2;
                    animator.Play("run_down");


                    if (!Valid(-Vector2.up))
                    {
                        TargetPos = (Vector2)transform.position - Vector2.up * 0.7f;

                    }
                }
                else if (Input.GetKey(KeyCode.A))
                {
                    //Debug.Log(" 我要左 ");
                    //动作为2
                    a = 3;
                    b = 2;

                    animator.Play("run_left");

                    DirID = a * 10 + b;

                    if (!Valid(-Vector2.right))
                    {
                        TargetPos = (Vector2)transform.position - Vector2.right * 1f;
                    }
                }
                else if (!info.IsTag("attack_all"))
                {

                    //Debug.Log(" 我要播放站立了 ");
                    b = 1;
                    if (a == 1)
                    {
                        animator.Play("stand_up");
                    }
                    if (a == 2)
                    {
                        animator.Play("stand_down");
                    }
                    if (a == 3)
                    {
                        animator.Play("stand_left");
                    }
                    if (a == 4)
                    {
                        animator.Play("stand_right");
                    }

                }
                Zdingwei();
                DirID = a * 10 + b;
            }
        }
 
        //给方向右做一个镜像处理
        if ( a == 4 )
        {
            transform.localScale = new Vector3(-1, 1, 1);
        }
        else
        {
            transform.localScale = new Vector3(1, 1, 1);
        }

    }
    void DeadManager()
    {
        GameObject game = GameObject.Find("Game_Manager");

        game.GetComponent<Game_manager>().IsStop = true;

    }
    /// <summary>
    /// 射线检测前方有没有碰撞体
    /// </summary>
    /// <param name="dir"></param>
    /// <returns></returns>
    bool Valid (Vector2 dir)
    {
        //设定碰撞层
        LayerMask mask = 1 << 14 | 1 << 15;

        Vector2 pos = transform.position;

        RaycastHit2D hit = Physics2D.Raycast(pos, dir, 0.7f, mask);

        //判断是否投射到collider上
        //if (hit)
        //{
        //    Debug.Log("箱子发射光线碰撞" + hit.collider.gameObject.name);
        //}

        return (hit);
    }
    /// <summary>
    /// 处理物体的前后排序问题
    /// </summary>
    void Zdingwei()
    {
        gameObject.transform.position = new Vector3(gameObject.transform.position.x, gameObject.transform.position.y, gameObject.transform.position.y * 0.1f);
    }

}


