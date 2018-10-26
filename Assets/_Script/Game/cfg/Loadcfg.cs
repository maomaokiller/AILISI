using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Loadcfg : MonoBehaviour
{
	IEnumerator Start ()
    {
        NpcCfgMgr.LoadConfig();
        while (!NpcCfgMgr.IsDone)
            yield return null;
        print( NpcCfgMgr.GetcfgName(2));
        print(NpcCfgMgr.GetcfgIntroduce(2));

        RushCfgMgr.LoadRushConfig();
        while (!RushCfgMgr.IsDone)
            yield return null;
        print(RushCfgMgr.GetMapCfg(1));
        print(RushCfgMgr.GetMapPosCfg(002, 002));
        print(RushCfgMgr.GetMapPosCfgValue(002, 002, "Count"));
        print(RushCfgMgr.GetMapPosCfgValue(002, 002, "Scale"));
        //这里刷怪参数名称如下和对应意义
        //_Name = Count/数量 NpcID/npcID FirstTime/刷怪初始时间  Intervals/刷怪间隔时间 Scale/缩放比（10为一倍大小）








    }
}
