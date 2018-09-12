using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;
using Pathfinding.RVO;
using Pathfinding.Examples;

public class Monster_Manager : MonoBehaviour {
    //怪物获取路径
    string MonsterPath1 = "prefab/Monster/01Bron/01Bron";
    string MonsterPath2 = "prefab/Monster/03lollipop/03lollipop";
    //找到一个空的对象Monster_box方便场景管理
    
    void Awake()
    {
        //找到地图上所有的Monster点
        GameObject[] Monsters = GameObject.FindGameObjectsWithTag("monster_layer");
        

        for (int i = 0; i < Monsters.Length; i++)
        {
            GameObject monster = Monsters[i];
            //实例化游戏物体（这里要处理怪物被箱子或者人压着的情况）
            GameObject _monster = (GameObject)Instantiate(Resources.Load(MonsterPath1), monster.transform.position, Quaternion.identity);

            GameObject Monster_box = GameObject.Find("Monster_box");
            //将怪物放置到一个统一的父物体下
            _monster.transform.parent = Monster_box.transform;
            //处理怪物的排序问题
            _monster.gameObject.transform.position = new Vector3(_monster.gameObject.transform.position.x, _monster.gameObject.transform.position.y, _monster.gameObject.transform.position.y * 0.1F);

            _monster.GetComponent<SpriteRenderer>().sortingLayerName = "Character";
            //设定统一的sortingOrder
            _monster.GetComponent<SpriteRenderer>().sortingOrder = -1;
            //挂上Monster1 脚本
            _monster.AddComponent<Monster1>();

            _monster.name = i + "Bron";

            monster.SetActive(false);

        }
    }
    /// <summary>
    /// 刷怪基础方法，需要传入一个pos和怪物prefab
    /// </summary>p
    /// <param name="pos"></param>
    public void CreateMonster( Vector2 pos )
    {
        
        GameObject monster_new = (GameObject)Instantiate(Resources.Load(MonsterPath2), pos, Quaternion.identity);

        monster_new.tag = "monster_layer";

        GameObject Monster_box = GameObject.Find("Monster_box");
        //将怪物放置到一个统一的父物体下
        monster_new.transform.parent = Monster_box.transform;
        //处理怪物的排序问题
        monster_new.gameObject.transform.position = new Vector3(monster_new.gameObject.transform.position.x, monster_new.gameObject.transform.position.y, monster_new.gameObject.transform.position.y * 0.1F);

        monster_new.GetComponent<SpriteRenderer>().sortingLayerName = "Character";
        //设定统一的sortingOrder
        monster_new.GetComponent<SpriteRenderer>().sortingOrder = 0;
        //游戏逻辑AI
        monster_new.AddComponent<AI>();

    }
}
