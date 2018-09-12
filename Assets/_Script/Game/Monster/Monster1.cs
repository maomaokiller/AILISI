using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Monster1 : MonoBehaviour {

    //时间间隔1
    public float TimeIn;
    //时间间隔2
    public float TimeOut;
    //时间消耗1
    public float Timeing1;
    //时间消耗2
    public float Timeing2;
    //指定动画控制器
    private Animator animator;
    //主角到当前物体的距离
    float distance;
    //刷怪计数
    int monster_number = 0;

    //设置碰撞层
    LayerMask mask = 1 << 14 | 1 << 15;


    void Start ()
    {
        //给时间间隔赋随机值
        TimeIn = Random.RandomRange(5f , 10f);

        TimeOut = Random.RandomRange( 3f, 5f );

        animator = gameObject.GetComponent<Animator>();


    }
	
	void FixedUpdate ()
    {
        GameObject player = GameObject.Find("Player");

        float distance = Vector2.Distance ( player.transform.position , gameObject.transform.position);
 

        if ( Valid_Monster( ) )//有物体压着
        {
            animator.Play("standN");
            Timeing1 = 0f;
            Timeing2 = 0f;
           
        }
        if (distance <= 0.7f)
        {
            animator.Play("standN");
            Timeing1 = 0f;
            Timeing2 = 0f;
            if (TimeOut >= 1.5f)
            {
                TimeOut -= Time.deltaTime;

            }
            if (TimeIn >= 1.5f)
            {
                TimeIn -= Time.deltaTime;
            }

        }

        if ( Valid_Monster() == false || distance > 0.7f )
        {

            Timeing1 += Time.deltaTime;

            //Debug.Log("计时为" + Timeing1);
            if (Timeing1 >= TimeIn)
            {
                animator.Play("stand");

                Timeing2 += Time.deltaTime;

            }
            if (Timeing2 >= TimeOut)
            {
                gameObject.GetComponent<SpriteRenderer>().sortingOrder = 0;
                animator.Play("attack");
                monster_number ++;
                if ( monster_number == 1 )
                {
                    GameObject.Find("monster_Manager").GetComponent<Monster_Manager>().CreateMonster(gameObject.transform.position);
                    //处理一下刷出问题和坑的排序问题
                    gameObject.transform.position = new Vector3(gameObject.transform.position.x, gameObject.transform.position.y, (gameObject.transform.position.y * 0.1F + 0.03f));
                    //这里可以添加特效播放
                }
                Timeing1 = 0;
                Destroy( gameObject, 0.8f );
            }
        }
       
    }
    bool Valid_Monster( )
    {
        //向上发射一条射线
        Vector2 pos = transform.position;

        RaycastHit2D hit = Physics2D.Raycast( pos, Vector2.up , 0.35f, mask);

        //判断是否投射到collider上
        //if (hit)
        //{
        //    Debug.Log("箱子发射光线碰撞" + hit.collider.gameObject.name);
        //}

        return (hit);
    }
}
