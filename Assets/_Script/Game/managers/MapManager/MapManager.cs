using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapManager : MonoBehaviour {

    private static bool flag = false;   //播放音乐状态  
    private GameObject backMusic;
    public GameObject prefabBackMusic;
   
    void Awake ()
    {
        WuJian_PaiXu();

        Music();

        Monsters_PaiXu();


    }
	void FixedUpdate () {
        
       
    }
    /// <summary>
    ///对所有物件做了深度排序，由于这里调用了装饰做父物体，在游戏创建时一定要有把所有物件放在名称为“ZhuangShi”物体下
    ///物件的Tag要为“map_wujian”
    /// </summary>
    void WuJian_PaiXu()
    {
        GameObject[] All_wujian = GameObject.FindGameObjectsWithTag("map_wujian");

        GameObject WuJian = GameObject.Find("ZhuangShi");

        for (int i = 0; i < All_wujian.Length; i++)
        {
            GameObject _wujian = All_wujian[i];

            _wujian.transform.parent = WuJian.transform;
            //用Y轴定义Z轴
            _wujian.gameObject.transform.position = new Vector3(_wujian.gameObject.transform.position.x, _wujian.gameObject.transform.position.y, _wujian.gameObject.transform.position.y * 0.1F);
            //设定统一的sortingOrder
            _wujian.GetComponent<SpriteRenderer>().sortingOrder = 0;
        }
       
    }

    /// <summary>
    /// 活物的排序处理方法由于要每帧调用希望做到最简化，也只对运动了的物体做处理不然会消耗很大
    /// 活物的Tag要为“Player”
    /// </summary>
    void Monsters_PaiXu()
    {
       
        GameObject[] All_Player = GameObject.FindGameObjectsWithTag("Player");

        for (int i = 0; i < All_Player.Length; i++)
        {
            GameObject _monster = All_Player[i];

            _monster.AddComponent<SpriteRenderer>();
            //设定统一的sortingOrder
            _monster.GetComponent<SpriteRenderer>().sortingOrder = 0;
            //用Y轴定义Z轴
            _monster.gameObject.transform.position = new Vector3(_monster.gameObject.transform.position.x, _monster.gameObject.transform.position.y, _monster.gameObject.transform.position.y * 0.1F);
        }

    }
    void Music()
    {
        backMusic = GameObject.FindGameObjectWithTag("sound");

        if (backMusic == null)
        {
            backMusic = (GameObject)Instantiate(prefabBackMusic);
            backMusic.tag = "sound";
        }
    }

}


