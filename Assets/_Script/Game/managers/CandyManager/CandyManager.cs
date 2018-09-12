using System.Collections;
using System.Collections.Generic;
using Pathfinding;
using UnityEngine;

public class CandyManager : MonoBehaviour
{
    // 糖块的路径
    string CandyPath = "prefab/npc/04"; //以后用表格替换硬写得路径资源
    
    //刷出时间
    public float CandyTime;
    //糖果层
    void Awake()
    {
        //找到所有的candy
        GameObject[] candys = GameObject.FindGameObjectsWithTag("candy_layer");
        //找到一个空的对象candy_box方便场景管理
        GameObject candy_box = GameObject.Find("candy_box");

        for (int i = 0; i < candys.Length; i++)
        {
            GameObject candy = candys[i];
            //实例化游戏物体
            GameObject _candy = (GameObject)Instantiate(Resources.Load(CandyPath), candy.transform.position, Quaternion.identity);

            _candy.transform.parent = candy_box.transform;

            BoxCollider2D boxCollider2D = _candy.AddComponent<BoxCollider2D>();

            boxCollider2D.offset = new Vector2(0f, 0f);

            boxCollider2D.size = new Vector2(0.95f, 0.65f);

            boxCollider2D.isTrigger = true;

            _candy.gameObject.transform.position = new Vector3(_candy.gameObject.transform.position.x, _candy.gameObject.transform.position.y, _candy.gameObject.transform.position.y * 0.1F);

            SpriteRenderer spriteRenderer = _candy.GetComponent<SpriteRenderer>();
            //设定统一的sortingOrder
            spriteRenderer.sortingLayerName = "Character";

            spriteRenderer.sortingOrder = 0;
            //挂上Candy_box 脚本
            _candy.AddComponent<Candy_box>();

            _candy.name = "candy" + i;

            _candy.layer = 15;//层

            _candy.tag = "candy_layer";

            candy.SetActive(false);


        }
        //在糖果盒子实例化后刷新地图格子
        Axing_refrsh_map();
    }
    void Axing_refrsh_map()
    {
        GameObject Axing = GameObject.FindGameObjectWithTag("Axing");

        Axing.GetComponent<AstarPath>().Scan();

    }

}

