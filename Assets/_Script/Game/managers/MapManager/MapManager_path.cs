using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapManager_path : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
    /// <summary>
    /// 通过坐标获取格子ID
    /// </summary>
    /// <param name="pos"></param>
    /// <returns></returns>
    public int GetGridByPos(Vector2 pos)
    {
        int x = (int)((pos.x + (50.0f / 2.0f)) / 50.0f);
        int y = (int)((pos.y + (50.0f / 2.0f)) / 50.0f);
        int gridID = x * (int)10000 + y;
        return gridID;
    }
    /// <summary>
    /// 通过格子ID获得坐标
    /// </summary>
    /// <param name="Grid"></param>
    /// <returns></returns>
    public Vector2 GetPosByGrid(int Grid)
    {
        int X = (Grid / (int)10000);
        int Y = (Grid / (int)10000);
        Vector2 pos = new Vector2(X * 50.0f, Y * 50.0f);
        return pos;
    }
}
