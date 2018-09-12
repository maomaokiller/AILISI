using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Zhuangshi_Manager : MonoBehaviour { 

    void Awake () {
        //首先找到所有装饰层的树给树添加一个2D碰撞盒子
        GameObject[] ZhuangShis = GameObject.FindGameObjectsWithTag("map_wujian");
        
        for (int i = 0; i < ZhuangShis.Length; i++)
        {
            GameObject zhuangshi = ZhuangShis[i];

            zhuangshi.AddComponent<BoxCollider2D>();
            zhuangshi.GetComponent<BoxCollider2D>().offset = new Vector2(0f, 0f);
            zhuangshi.GetComponent<BoxCollider2D>().size = new Vector2(1f,0.7f);
            zhuangshi.layer = 15;//设置他们的层为Collier层

        } 

		
	}
	
	
	void Update () {
		
	}
}
