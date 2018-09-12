using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Manager : MonoBehaviour {

    // player的路径
    string PlayerPath = "prefab/Player/Player";
    //play的出生点
    public Transform PlayerPoint;
    //刷出时间
    public float PlayerTime;


    void Awake() {
        
        //找到游戏中的Start点，并把它的位置付给PlayerPoint
        GameObject StartObject = GameObject.FindWithTag("Start");

        PlayerPoint = StartObject.GetComponent<Transform>();
       
        //实例化游戏物体
        GameObject player = (GameObject)Instantiate(Resources.Load(PlayerPath), PlayerPoint.position, Quaternion.identity);

        player.AddComponent<PlayerMove>();

        player.name = "Player";

        StartObject.SetActive(false);


    }
}



