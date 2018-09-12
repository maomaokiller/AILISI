
using UnityEngine;
namespace Engine
{
    /// <summary>
    /// unity的layer定义
    /// </summary>
    public class LayerDefine
    {
        public const int MapSurface = 8;
        public const int MapSurfaceMask = 1 << MapSurface;


        public const int Sky = 9;
        public const int StaticItem = 10;
        public const int DynamicItem = 11;
        public const int Creature = 12;
        public const int UpCreatureEffect = 13;
        public const int Collier_box = 14;
        public const int Collier_background = 15;
        public const int Collier_npc = 16;

        public const int MapAlphaBlockItem = 17;
        public const int MapAlphaBlockItemMask = 1 << MapAlphaBlockItem;

        public const int GuideLayer = 30;
        public const int GuideLayerMask = 1 << GuideLayer;

        public const int RenderTexure = 18;

        public const int Zhezhao = 20;
        public const int UIScene = 21;
        public const int UISceneMask = 1 << UIScene;
        public const int FogOfWarMask = 22;
        public static void SetLayer(GameObject obj, int layerType)
        {
            obj.layer = layerType;
            int childCount = obj.transform.childCount;
            for (int index = 0; index < childCount; ++index)
                SetLayer(obj.transform.GetChild(index).gameObject, layerType);
        }

        public static void SetLayer(Component obj, int layerType)
        {
            obj.gameObject.layer = layerType;
            int childCount = obj.transform.childCount;
            for (int index = 0; index < childCount; ++index)
                SetLayer(obj.transform.GetChild(index).gameObject, layerType);
        }
    }
}
