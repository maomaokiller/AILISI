using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class Candy_box : MonoBehaviour
{

    //candy_box移动速度
    public float Candy_box_Speed = 3.5f;
    //candy_box移动方向
    public Vector2 candy_box_dir;
    public Vector2 candy_box_dir_move;
    //箱子被推得判断
    public bool Ispush;
    //目标点
    private Vector2 TargetPos;
    //判定candy是否移动
    public bool CandyMove;
    //candy原始位置(推箱子那一刻被记录初始值——PlayerMove里定义)
    public Vector2 candy_old_pos;
    //candy当前位置
    public Vector2 candy_now_pos;
    //设置碰撞层
    LayerMask mask = 1 << 15;
    //是否找到怪物
    bool FindMonster =false;
    //__________________________________________
    GameObject otherBody;
    void Awake()
    {
        otherBody = new GameObject
        {
            layer = 12,
            tag = "candy_layer"
        };

        otherBody.transform.position = transform.position;

        otherBody.transform.parent = transform;

        otherBody.AddComponent<FindMonster>();

        Ispush = false;

        CandyMove = false;

        TargetPos = (Vector2)transform.position;

        candy_box_dir = Vector2.zero;

    }
    private void Start()
    {
        otherBody.name = gameObject.name + "100" ;
    }

    // Update is called once per frame
    void Update()
    {
       
        Vector2 p = Vector2.MoveTowards(transform.position, TargetPos, Candy_box_Speed / 10);

        if (Ispush)
        {
            if (CandyMove)
            {
                ToMove(gameObject);

                GetComponent<Rigidbody2D>().MovePosition(p);

                Displacement();

                //取得当前位置
                candy_now_pos = (Vector2)transform.position;

                candy_box_dir_move = candy_box_dir;
            }
            if (CandyMove == false)
            {
                ToStand(gameObject);



                //判断挤爆的情况
                if (candy_old_pos - candy_now_pos == Vector2.zero)
                {
                    //Debug.Log(gameObject.name + "当前位置为" + candy_now_pos);

                    //刷新一次A星地图格子
                    Collider_disappear();

                    Door_Open();
                    Destroy(gameObject, 0.3f);
                }
            }

        }
    }
    private void OnTriggerStay(Collider other)
    {
        if (other.tag == "monster_layer")
        {
            Debug.Log("敌人在我范围内" + other.name);
        }
        
    }
    /// <summary>
    /// 判断是否碰到物体
    /// </summary>
    /// <param name="dir"></param>
    /// <returns></returns>
    bool Valid_candy(Vector2 dir)
    {
        //向角色前方发射一条射线dir/2的长度
        Vector2 pos = transform.position;

        RaycastHit2D hit = Physics2D.Raycast( pos , dir , 0.7f , mask );

        return ( hit );
    }
    /// <summary>
    /// 在箱子变为移动状态做的一系列处理
    /// </summary>
    void ToMove(GameObject box)
    {
        if (box.GetComponent<Rigidbody2D>() == null)
        {
            
            box.GetComponent<BoxCollider2D>().offset = new Vector2(0.45f, 0.3f);
            box.GetComponent<BoxCollider2D>().size = new Vector2(0.1f, 0.1f);
            box.AddComponent<Rigidbody2D>();
            box.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Kinematic;
            box.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeRotation;
        }

    }
    /// <summary>
    /// 箱子变为站立状态做的一系列处理
    /// </summary>
    /// <param name="box"></param>
    void ToStand (GameObject box)
    {
        Destroy(box.GetComponent<Rigidbody2D>());
        box.GetComponent<BoxCollider2D>().offset = new Vector2(0, 0);
        box.GetComponent<BoxCollider2D>().size = new Vector2(0.95f, 0.65f);
        box.gameObject.transform.position = new Vector3(box.gameObject.transform.position.x, box.gameObject.transform.position.y, box.gameObject.transform.position.y * 0.1F);
        candy_box_dir = Vector2.zero;
        Ispush = false;
        //刷新一次A星地图格子
        Door_Open();
        if (otherBody.transform.childCount>=1)
        {
            Destroy();
        }

    }
     void Destroy()
     {
        Debug.Log(transform.name + "删除子物体");
        for (int i = 0; i < otherBody.transform.childCount; i++)
        {
            Destroy(otherBody.transform.GetChild(i).gameObject, 0.2f);
            //此处播放怪物消失特效和音效
        }
     }
    /// <summary>
    /// 地图中箱子的移动处理方案
    /// 
    /// </summary>
    void Displacement()
    {
        if (candy_box_dir == Vector2.up && TargetPos == (Vector2)transform.position)
        {
            Door_Open();

            if (Valid_candy(Vector2.up) == false)//前方没碰撞就
            {
                TargetPos = (Vector2)transform.position + candy_box_dir * 0.7f;

            }
            else if (Valid_candy(Vector2.up))//有碰撞
            {
                CandyMove = false;

            }
        }
        else if (candy_box_dir == -Vector2.up && TargetPos == (Vector2)transform.position)
        {
            Door_Open();
            if (Valid_candy(-Vector2.up) == false)//前方没碰撞就
            {
                TargetPos = (Vector2)transform.position + candy_box_dir * 0.7f;

            }
            else if (Valid_candy(-Vector2.up))
            {
                CandyMove = false;

                //Debug.Log( gameObject.name + "当前位置为" + candy_now_pos);
            }
        }
        else if (candy_box_dir == Vector2.right && TargetPos == (Vector2)transform.position)
        {
            Door_Open();
            if (Valid_candy(Vector2.right) == false)//前方没碰撞就
            {
                TargetPos = (Vector2)transform.position + candy_box_dir;

            }
            else if (Valid_candy(Vector2.right))
            {
                CandyMove = false;
               
                //Debug.Log(gameObject.name+"当前位置为" + candy_now_pos);
            }
        }
        else if (candy_box_dir == -Vector2.right && TargetPos == (Vector2)transform.position)
        {
            Door_Open();
            if (Valid_candy(-Vector2.right) == false)//前方没碰撞就
            {
                TargetPos = (Vector2)transform.position + candy_box_dir;

            }
            else if (Valid_candy(-Vector2.right))
            {
                CandyMove = false;
                
                //Debug.Log(gameObject.name+"当前位置为" + candy_now_pos);
            }
        }
        
        

    }
    /// <summary>
    /// 
    /// 箱子移动后A*算法的刷新
    /// </summary>
    void Door_Open()
    { 
            Bounds b = gameObject.GetComponent<BoxCollider2D>().bounds;
            GraphUpdateObject guo = new GraphUpdateObject(b);
            AstarPath.active.UpdateGraphs(guo);
            AstarPath.active.FlushWorkItems();
    }
    /// <summary>
    /// 箱子需要被销前做的处理，为解决A*插件算法上的bug做的变相处理
    /// </summary>
    private void Collider_disappear()
    {
        SpriteRenderer spriteRenderer = gameObject.GetComponent<SpriteRenderer>();

        spriteRenderer.color = new Color(1, 1, 1, 0);

        BoxCollider2D boxCollider2D = gameObject.GetComponent<BoxCollider2D>();
        boxCollider2D.offset = new Vector2(0.45f, 0.3f);
        boxCollider2D.size = new Vector2(0.1f, 0.1f);

    }

}

