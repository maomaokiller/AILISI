using System.Collections;
using System.Collections.Generic;
using Engine;
using UnityEngine;


namespace Game
{
    public class MgrIniter
    {
        private static bool _inited;
        public static void Init()
        {
            if (_inited)
            {
                return;
            }

            GameObject mgrRoot = new GameObject("MgrsObj");
            Object.DontDestroyOnLoad(mgrRoot);

            //CoroutineMgr.me.Init(mgrRoot);


            //if (GuideManager.Open)
            //{
            //    GuideManager.me.Init(mgrRoot);
            //}
            //if (GameDefine.ShowGM)
            //{
            //    GMPanel.showGMPanel();
            //}
            _inited = true;
        }
    }
}