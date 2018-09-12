using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterTiSheng : MonoBehaviour {

    GameObject player;
	void Awake () {
        player = GameObject.FindGameObjectWithTag("Player");

        Rigidbody2D rigidbody2D = gameObject.AddComponent<Rigidbody2D>();
        rigidbody2D.bodyType = RigidbodyType2D.Kinematic;
        rigidbody2D.constraints = RigidbodyConstraints2D.FreezeRotation;

        gameObject.layer = 12;

        CircleCollider2D collider2D = gameObject.AddComponent<CircleCollider2D>();
        collider2D.radius = 0.2f; 
        collider2D.offset = new Vector2(0f, 0f);
        collider2D.isTrigger = true;

       

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            Debug.Log("主角死亡");

            player.GetComponent<PlayerMove>().Dead = true;
            //后面写死亡相关的内容触发
        }
        if ( collision.tag == "candy_layer" )
        {
            Debug.Log("撞击怪物的箱子是" + collision.transform.gameObject.name);

            var onai = gameObject.transform.parent.GetComponent<AI>();

            onai.Die_candy_dir = collision.transform.parent.gameObject.GetComponent<Candy_box>().candy_box_dir_move.normalized;

            onai.Die_candy = collision.transform.gameObject;
        }

    }

}
