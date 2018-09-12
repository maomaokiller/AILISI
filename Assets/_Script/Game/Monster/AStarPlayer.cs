using UnityEngine;
using System.Collections;
using Pathfinding;

public class AStarPlayer : MonoBehaviour
{
    //目标位置;  
    Vector3 targetPosition;

    Seeker seeker;

    CharacterController characterController;

    //怪物警戒范围
    public const int AI_Distance_In = 2;
    public const int AI_Distance_out = 4;
    //计算出来的路线;  
    Path path;
    //移动速度;  
    float playerMoveSpeed = 1f;

    //当前点  
    int currentWayPoint = 0;

    bool stopMove = true;

    //怪物中心点;  
    float MonsterCenterY = 1.0f;
    //计数器
    int a = 0;

    int b = 0;

    void Start()
    {

        seeker = GetComponent<Seeker>();

        MonsterCenterY = transform.localPosition.y;
    }

    //寻路结束;  
    public void OnPathComplete(Path p)
    {
        Debug.Log("OnPathComplete error = " + p.error);

        if (!p.error)
        {
            currentWayPoint = 0;
            path = p;
            stopMove = false;
        }

        for (int index = 0; index < path.vectorPath.Count; index++)
        {
            Debug.Log("path.vectorPath[" + index + "]=" + path.vectorPath[index]);
        }
    }

    void Update()
    {
        GameObject Player = GameObject.FindGameObjectWithTag("Player");

        float distance = Vector2.Distance(Player.transform.position, gameObject.transform.position);

        if (distance <= AI_Distance_In)
        {
            a++;
            if (a == 1)
            {
                Debug.Log("我发现你了！别想跑");
                //转入追逐模式
                b = 0;

            }

        }
        else if (distance >= AI_Distance_out)
        {
            b++;

            if (b == 1)
            {
                Debug.Log("你跑的太快了 我追不上了！");
                //转入巡逻状态
                a = 0;

            }


        }
    }

    void FixedUpdate()
    {
        if (path == null || stopMove)
        {
            return;
        }

        //根据Player当前位置和 下一个寻路点的位置，计算方向;  
        Vector3 currentWayPointV = new Vector3(path.vectorPath[currentWayPoint].x, path.vectorPath[currentWayPoint].y + MonsterCenterY, path.vectorPath[currentWayPoint].z);
        Vector3 dir = (currentWayPointV - transform.position).normalized;

        //计算这一帧要朝着 dir方向 移动多少距离;  
        dir *= playerMoveSpeed * Time.fixedDeltaTime;

        //计算加上这一帧的位移，是不是会超过下一个节点;  
        float offset = Vector3.Distance(transform.localPosition, currentWayPointV);

        if (offset < 0.1f)
        {
            transform.localPosition = currentWayPointV;

            currentWayPoint++;

            if (currentWayPoint == path.vectorPath.Count)
            {
                stopMove = true;

                currentWayPoint = 0;
                path = null;
            }
        }
        else
        {
            if (dir.magnitude > offset)
            {
                Vector3 tmpV3 = dir * (offset / dir.magnitude);
                dir = tmpV3;

                currentWayPoint++;

                if (currentWayPoint == path.vectorPath.Count)
                {
                    stopMove = true;

                    currentWayPoint = 0;
                    path = null;
                }
            }
            transform.localPosition += dir;
        }
    }
    //以下是辅助线圈的
    float m_Radius = AI_Distance_In; // 圆环的半径 
    public float m_Theta = 0.1f; // 值越低圆环越平滑  
    public Color m_Color = Color.green; // 线框颜色  

    void OnDrawGizmos()
    {

        if (m_Theta < 0.0001f) m_Theta = 0.0001f;

        // 设置矩阵  
        Matrix4x4 defaultMatrix = Gizmos.matrix;
        Gizmos.matrix = transform.localToWorldMatrix;

        // 设置颜色  
        Color defaultColor = Gizmos.color;
        Gizmos.color = m_Color;

        // 绘制圆环  
        Vector3 beginPoint = Vector3.zero;
        Vector3 firstPoint = Vector3.zero;
        for (float theta = 0; theta < 2 * Mathf.PI; theta += m_Theta)
        {
            float x = m_Radius * Mathf.Cos(theta);
            float y = m_Radius * Mathf.Sin(theta);
            Vector3 endPoint = new Vector3(x, y, 0);
            if (theta == 0)
            {
                firstPoint = endPoint;
            }
            else
            {
                Gizmos.DrawLine(beginPoint, endPoint);
            }
            beginPoint = endPoint;
        }

        // 绘制最后一条线段  
        Gizmos.DrawLine(firstPoint, beginPoint);

        // 恢复默认颜色  
        Gizmos.color = defaultColor;

        // 恢复默认矩阵  
        Gizmos.matrix = defaultMatrix;
    }
}