using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterTiAI : MonoBehaviour {
    //主角
    private GameObject Player;
    //怪物警戒范围
    public const int AI_Distance_In = 2;
    public const int AI_Distance_out = 4;
    // Use this for initialization
    GameObject FuObject;
    void Awake () {
        Player = GameObject.FindGameObjectWithTag("Player");

        FuObject = transform.parent.gameObject;

        Rigidbody2D rigidbody2D = gameObject.AddComponent<Rigidbody2D>();

        rigidbody2D.bodyType = RigidbodyType2D.Kinematic;
        rigidbody2D.constraints = RigidbodyConstraints2D.FreezeRotation;

        CircleCollider2D circleCollider2D = gameObject.AddComponent<CircleCollider2D>();
        circleCollider2D.offset = new Vector2(0f, 0f);
        circleCollider2D.isTrigger = true;
        circleCollider2D.radius =AI_Distance_In;
        gameObject.layer = 16;
    }

    // Update is called once per frame
    void OnTriggerEnter2D(Collider2D c)
    {
        if (c.tag == "Player")
        {
            Debug.Log("我发现你了！别想跑");
            //转入追逐模式

            gameObject.GetComponent<AILerp>().target = Player.transform;

            GetComponent<CircleCollider2D>().radius = AI_Distance_out;
        }

    }
    /// <summary>
    /// 检测玩家是否跑出追击范围
    /// </summary>
    /// <param name="c"></param>
    void OnTriggerExit2D(Collider2D c)
    {
        if (c.tag == "Player")
        {
            Debug.Log("你跑的太快了 我追不上了！");

            gameObject.GetComponent<AILerp>().target = null;

            GetComponent<CircleCollider2D>().radius = AI_Distance_In;

            FuObject.GetComponent<AI>().Act = 1;

        }

    }
}
