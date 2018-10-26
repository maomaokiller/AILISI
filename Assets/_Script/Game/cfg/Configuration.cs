using System.Collections;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public static class Configuration
{
    //申明字典以及结构
    private static  Dictionary<string, Dictionary<string, string>> dicConfig = new Dictionary<string, Dictionary<string, string>>();
    //声明一个www
    private static WWW www;
    //加载配置信息
    public static void LoadConfig()
    {
        //在StreamingAssets文件夹下的文件只能通过www方式读取
        string config = "file://" + Application.streamingAssetsPath + "/cfg/Game_test.txt";

        if ( www == null)
        {
            //安卓平台需要添加一个jar：
#if UNITY_ANDROID
    config = "jar:"+config;
#endif
            www = new WWW(config);
        }

    }
    //检查配置信息是否加载结束
    public static bool IsDone
    {
        
        get
        { 
            //检测是否加载完成
            //第一步先检测是否加载出错
            if( www !=null &&  www.isDone  &&  string.IsNullOrEmpty( www.error) )
            {
                BuildDictionary(www.text);
                //没有加载错误返回true
                return true;
            }
            //如果以上没有返回这里就会返回false
            return false;
        }
    }
    //对外提供提取配置项的接口
    public static string GetString(string mainKey, string subkey)
    {
       return Get(mainKey, subkey);//返回字符串

    }
    public static int GetInt(string mainKey, string subkey)
    {
        string value = Get(mainKey, subkey);
        //判断字符是否为空如果为空返回零否则转换为int返回
        return value != null ? int.Parse(value) : 0;
    }
    public static float GetFloat(string mainKey, string subkey)
    {
        string value = Get(mainKey, subkey);
        //判断字符是否为空如果为空返回零否则转换为int返回
        return value != null ? float.Parse(value) : 0;
    }
    private static string Get(string mainKey, string subKey)
    {
        if (dicConfig.ContainsKey(mainKey) && dicConfig[mainKey].ContainsKey(subKey))
        {
            return dicConfig[mainKey][subKey];
        }
        return null;
    }
    //将配置信息在内存中形成数据结构
    private static void BuildDictionary(string configText)
    {
        //声明读取
        StringReader reader = new StringReader(configText);
        //声明行
        string line = null;
        //声明主键
        string mainKey = null;
        //声明子键
        string subKey = null;
        //声明赋值
        string subValue = null;
        //逐行读取配置信息
        while ((line = reader.ReadLine()) != null)
        {
            //去掉字符串前后的空格
            line = line.Trim();
            //判断如果行为空去掉
            if (!string.IsNullOrEmpty(line))
            {
                //检查是否是主键
                if (line.StartsWith("["))//如果是“[”开头为主键
                {
                    //取得[]中的文本内容.截取字符串设为主键
                    mainKey = line.Substring(1, line.IndexOf("]") - 1);
                    //放置主键到字典并建立子键字典结构
                    dicConfig.Add(mainKey, new Dictionary<string, string>());
                }
                else//则为子键
                {
                    //分割等号两边的字符，（前面是分割标记，后面是剔除空格）
                    var configKeyValue = line.Split(new char[] { '=' }, StringSplitOptions.RemoveEmptyEntries);
                    //
                    subKey = configKeyValue[0].Trim();//后面Trim（）去空格
                    subValue = configKeyValue[1].Trim();
                    //检查subValue是否有“”并跳过
                    subValue = subValue.StartsWith("\"") ? subValue.Substring(1) : subValue;
                    //子键的字典赋值
                    dicConfig[mainKey].Add(subKey, subValue);
                }

            }

            
        }


    }
	
}
