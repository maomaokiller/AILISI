using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game_manager : MonoBehaviour {

    GameObject GameRoot_manager;

    public bool IsStop = false;

    void Awake()
    {
        GameRoot_manager = new GameObject("GameRoot_manager");

        Application.targetFrameRate = 30;

        //MgrIniter.Init();

    }
    void Update()
    {
        UpdateTick();
    }
    void FixedUpdate()
    {
        if (IsStop)
        {
            GameStop();
        }
    }

    void OnGUI()
    {
        DrawFps();
    }

    private void DrawFps()
    {
        if (mLastFps > 25)
        {
            GUI.color = new Color(0, 1, 0);
        }
        else if (mLastFps > 20)
        {
            GUI.color = new Color(1, 1, 0);
        }
        else
        {
            GUI.color = new Color(1.0f, 0, 0);
        }

        GUI.Label(new Rect(50, 32, 64, 24), "fps: " + mLastFps);

    }

    private long mFrameCount = 0;
    private long mLastFrameTime = 0;
    static long mLastFps = 0;
    private void UpdateTick()
    {
        if (true)
        {
            mFrameCount++;
            long nCurTime = TickToMilliSec(System.DateTime.Now.Ticks);
            if (mLastFrameTime == 0)
            {
                mLastFrameTime = TickToMilliSec(System.DateTime.Now.Ticks);
            }

            if ((nCurTime - mLastFrameTime) >= 1000)
            {
                long fps = (long)(mFrameCount * 1.0f / ((nCurTime - mLastFrameTime) / 1000.0f));

                mLastFps = fps;

                mFrameCount = 0;

                mLastFrameTime = nCurTime;
            }
        }
    }
    public static long TickToMilliSec(long tick)
    {
        return tick / (10 * 1000);
    }
    /// <summary>
    /// 游戏暂停
    /// </summary>
    void GameStop()
    {
        Time.timeScale = 0;
    }
    /// <summary>
    /// 游戏退出
    /// </summary>
    void GameQuit()
    {
        Application.Quit();
    }
}  
	
	

