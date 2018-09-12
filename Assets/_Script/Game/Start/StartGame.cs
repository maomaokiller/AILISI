using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartGame : MonoBehaviour {

    //Canvas动画控制器的引用
    Animator canvasAnim;

    private void Awake()
    {
        GameObject canvas = GameObject.FindGameObjectWithTag("Canvas");

        canvasAnim = canvas.GetComponent<Animator>();
        //调用某方法在多少时间以后
        Invoke ("ShowStartBtn",0.7f );
     
    }
    void ShowStartBtn()
    {
        canvasAnim.SetTrigger("ShowButton");
    }
    public void EnterGame()
    {

        //场景切换调用场景X
        SceneManager.LoadScene(1);
        
    }
}
