using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Pathfinding;
using Pathfinding.Util;

/** Linearly interpolating movement script.
 * This movement script will follow the path exactly, it uses linear interpolation to move between the waypoints in the path.
 * This is desirable for some types of games.
 * It also works in 2D.
 *
 * Recommended setup:
 *
 * This depends on what type of movement you are aiming for.
 * If you are aiming for movement where the unit follows the path exactly (you are likely using a grid or point graph)
 * the default settings on this component should work quite well, however I recommend that you adjust the StartEndModifier
 * on the Seeker component: set the 'Exact Start Point' field to 'NodeConnection' and the 'Exact End Point' field to 'SnapToNode'.
 *
 * If you on the other hand want smoother movement I recommend adding the Simple Smooth Modifier to the GameObject as well.
 * You may also want to tweak the #rotationSpeed.
 *
 * \ingroup movementscripts
 */
[RequireComponent(typeof(Seeker))]
[AddComponentMenu("Pathfinding/AI/AILerp (2D,3D)")]
[HelpURL("http://arongranberg.com/astar/docs/class_a_i_lerp.php")]
public class AILerp : VersionedMonoBehaviour {
    //添加一个方向指向点
    public Vector2 DirPos;
    /** Determines how often it will search for new paths.
     * 确定搜索新路径的频率。
	 * If you have fast moving targets or AIs, you might want to set it to a lower value.
     * 如果你有快速移动的目标或AI，你可能想把它设置为一个较低的值。
	 * The value is in seconds between path requests.
     * 路径请求之间的值以秒为单位。
	 */
    public float repathRate = 0.2F;

    /** Target to move towards.
     * 目标朝着。
	 * The AI will try to follow/move towards this target.
     * AI会试图跟随/朝着这个目标前进。
	 * It can be a point on the ground where the player has clicked in an RTS for example, or it can be the player object in a zombie game.
     * 它可以是玩家在RTS中点击的地点，或者可以是僵尸游戏中的玩家对象。
	 */
    public Transform target;

    /** Enables or disables searching for paths.
     * 启用或禁用搜索路径。
	 * Setting this to false does not stop any active path requests from being calculated or stop it from continuing to follow the current path.
     * 将其设置为false不会停止计算任何活动路径请求，也不会停止继续跟踪当前路径。
	 * \see #canMove
	 */
    public bool canSearch = true;

    /** Enables or disables movement.
     * 启用或禁用移动。
	 * \see #canSearch */
    public bool canMove = true;

    /** Speed in world units */
    public float speed = 3;

    /** If true, the AI will rotate to face the movement direction */
    /** 如果为真，则AI将旋转以面对移动方向 */
    public bool enableRotation = true;

    /** If true, rotation will only be done along the Z axis so that the Y axis is the forward direction of the character.
     * 如果为true，则仅沿Z轴旋转，以使Y轴为角色的前进方向。
	 * This is useful for 2D games in which one often want to have the Y axis as the forward direction to get sprites and 2D colliders to work properly.
     * *这对于2D游戏非常有用，在这些游戏中，人们经常希望将Y轴作为前进方向，以使精灵和2D对撞机正常工作。
	 * \shadowimage{aibase_forward_axis.png}
	 */
    public bool rotationIn2D = false;

    /** How quickly to rotate */
    /**旋转的速度有多快*/
    public float rotationSpeed = 10;

    /** If true, some interpolation will be done when a new path has been calculated.
     */
    /** 如果为true，则在计算新路径时将进行一些插值。
    * This is used to avoid short distance teleportation.
    * 这用于避免短距离远距传送。
    */
    public bool interpolatePathSwitches = true;

    /** How quickly to interpolate to the new path */
    /** 插入到新路径的速度有多快 */
    public float switchPathInterpolationSpeed = 5;

    /** Cached Seeker component */
    /** 缓存的搜索者组件 */
    protected Seeker seeker;

    /** Cached Transform component */
    /** 缓存转换组件 */
    public Transform tr;

    /** Time when the last path request was sent */
    /** 发送最后一个路径请求的时间 */
    protected float lastRepath = -9999;

    /** Current path which is followed */
    /** 当前路径 */
    protected ABPath path;

    /** True if the end-of-path is reached.
	 * \see TargetReached */
     /**如果到达路径终点，则为真。
     * \请参阅TargetReached */
    public bool targetReached { get; private set; }

    /** Only when the previous path has been returned should be search for a new path */
    /** 只有在返回上一个路径时才应搜索新路径 */
    protected bool canSearchAgain = true;

    /** When a new path was returned, the AI was moving along this ray.
     * 当返回一条新路径时，AI正沿着这条光线移动。
	 * Used to smoothly interpolate between the previous movement and the movement along the new path.
     * 用于在先前的移动和沿新路径的移动之间平滑插值。
	 * The speed is equal to movement direction.
     * 速度等于运动方向。
	 */
    protected Vector3 previousMovementOrigin;
    protected Vector3 previousMovementDirection;
    protected float previousMovementStartTime = -9999;

    protected PathInterpolator interpolator = new PathInterpolator();

    /** Holds if the Start function has been run.
	 * Used to test if coroutines should be started in OnEnable to prevent calculating paths
	 * in the awake stage (or rather before start on frame 0).
	 */
    private bool startHasRun = false;

    /** Initializes reference variables.
	 * If you override this function you should in most cases call base.Awake () at the start of it.
	 * */
    protected override void Awake() {
        base.Awake();
        //This is a simple optimization, cache the transform component lookup
        tr = transform;

        seeker = GetComponent<Seeker>();

        // Tell the StartEndModifier to ask for our exact position when post processing the path This
        //告诉StartEndModifier在后处理路径时请求我们的确切位置
        // is important if we are using prediction and requesting a path from some point slightly ahead
        //如果我们正在使用预测，并且稍微提前请求一个路径，那么这是非常重要的
        // of us since then the start point in the path request may be far from our position when the
        //从那时起，我们的路径请求中的起始点可能远离我们的位置
        // path has been calculated. This is also good because if a long path is requested, it may take
        // a few frames for it to be calculated so we could have moved some distance during that time
        seeker.startEndModifier.adjustStartPoint = () => tr.position;
    }

    /** Starts searching for paths.
	 * If you override this function you should in most cases call base.Start () at the start of it.
	 * \see #Init
	 * \see #RepeatTrySearchPath
	 */
    protected virtual void Start() {
        startHasRun = true;
        Init();
    }

    /** Called when the component is enabled */
    protected virtual void OnEnable() {
        // Make sure we receive callbacks when paths complete
        seeker.pathCallback += OnPathComplete;
        Init();
    }

    void Init() {
        if (startHasRun) {
            lastRepath = float.NegativeInfinity;
            StartCoroutine(RepeatTrySearchPath());
        }
    }

    public void OnDisable() {
        // Abort any calculations in progress
        //中止正在进行的任何计算
        if (seeker != null) seeker.CancelCurrentPathRequest();
        canSearchAgain = true;

        // Release the current path so that it can be pooled
        if (path != null) path.Release(this);
        path = null;

        // Make sure we no longer receive callbacks when paths complete
        seeker.pathCallback -= OnPathComplete;
    }

    /** Tries to search for a path every #repathRate seconds.
	 * \see TrySearchPath
	 */
    protected IEnumerator RepeatTrySearchPath() {
        while (true) {
            float v = TrySearchPath();
            yield return new WaitForSeconds(v);
        }
    }

    /** Tries to search for a path.
	 * Will search for a new path if there was a sufficient time since the last repath and both
	 * #canSearchAgain and #canSearch are true and there is a target.
	 *
	 * \returns The time to wait until calling this function again (based on #repathRate)
	 */
    public float TrySearchPath() {
        if (Time.time - lastRepath >= repathRate && canSearchAgain && canSearch && target != null) {
            SearchPath();
            return repathRate;
        } else {
            return Mathf.Max(0, repathRate - (Time.time - lastRepath));
        }
    }

    /** Requests a path to the target.
	 * Some inheriting classes will prevent the path from being requested immediately when
	 * this function is called, for example when the AI is currently traversing a special path segment
	 * in which case it is usually a bad idea to search for a new path.
	 */
    public virtual void SearchPath() {
        ForceSearchPath();
    }

    /** Requests a path to the target.请求目标的路径。
	 * Bypasses 'is-it-a-good-time-to-request-a-path' checks.
	 */
    public virtual void ForceSearchPath() {
        if (target == null) throw new System.InvalidOperationException("Target is null");

        lastRepath = Time.time;
        // This is where we should search to
        //这是我们应该搜索的地方
        var targetPosition = target.position;
        var currentPosition = GetFeetPosition();

        // If we are following a path, start searching from the node we will
        //如果我们沿着一条路径，从节点开始搜索
        // reach next this can prevent odd turns right at the start of the path
        //到达下一个，这可以防止路径开始时的右转
        if (interpolator.valid) {
            var prevDist = interpolator.distance;
            // Move to the end of the current segment
            //移动到当前段的末尾
            interpolator.MoveToSegment(interpolator.segmentIndex, 1);
            currentPosition = interpolator.position;
            // Move back to the original position
            //回到原来的位置
            interpolator.distance = prevDist;
        }

        canSearchAgain = false;
        // Alternative way of requesting the path
        //ABPath p = ABPath.Construct (currentPosition,targetPoint,null);
        //seeker.StartPath (p);

        // We should search from the current position
        seeker.StartPath(currentPosition, targetPosition);
    }

    /** The end of the path has been reached.到达路径末端
	 * If you want custom logic for when the AI has reached it's destination
	 * add it here.
	 * You can also create a new script which inherits from this one
	 * and override the function in that script.
	 */
    public virtual void OnTargetReached() {
    }

    /** Called when a requested path has finished calculation.
	 * A path is first requested by #SearchPath, it is then calculated, probably in the same or the next frame.
	 * Finally it is returned to the seeker which forwards it to this function.\n
	 */
    public virtual void OnPathComplete(Path _p) {
        ABPath p = _p as ABPath;

        if (p == null) throw new System.Exception("This function only handles ABPaths, do not use special path types");

        canSearchAgain = true;

        // Increase the reference count on the path.
        // This is used for path pooling
        p.Claim(this);

        // Path couldn't be calculated of some reason.
        // More info in p.errorLog (debug string)
        if (p.error) {
            p.Release(this);
            return;
        }

        if (interpolatePathSwitches) {
            ConfigurePathSwitchInterpolation();
        }

        // Release the previous path
        // This is used for path pooling.
        // Note that this will invalidate the interpolator
        // since the vectorPath list will be pooled.
        if (path != null) path.Release(this);

        // Replace the old path
        path = p;
        targetReached = false;

        // Just for the rest of the code to work, if there
        // is only one waypoint in the path add another one
        if (path.vectorPath != null && path.vectorPath.Count == 1) {
            path.vectorPath.Insert(0, GetFeetPosition());
        }

        // Reset some variables
        ConfigureNewPath();
    }

    protected virtual void ConfigurePathSwitchInterpolation() {
        bool reachedEndOfPreviousPath = interpolator.valid && interpolator.remainingDistance < 0.0001f;

        if (interpolator.valid && !reachedEndOfPreviousPath) {
            previousMovementOrigin = interpolator.position;
            previousMovementDirection = interpolator.tangent.normalized * interpolator.remainingDistance;
            previousMovementStartTime = Time.time;
        } else {
            previousMovementOrigin = Vector3.zero;
            previousMovementDirection = Vector3.zero;
            previousMovementStartTime = -9999;
        }
    }

    public virtual Vector3 GetFeetPosition() {
        return tr.position;
    }

    /** Finds the closest point on the current path and configures the #interpolator */
    /** 在当前路径上查找最近的点并配置#interpolator */
    protected virtual void ConfigureNewPath() {
        var hadValidPath = interpolator.valid;
        var prevTangent = hadValidPath ? interpolator.tangent : Vector3.zero;

        interpolator.SetPath(path.vectorPath);
        interpolator.MoveToClosestPoint(GetFeetPosition());

        if (interpolatePathSwitches && switchPathInterpolationSpeed > 0.01f && hadValidPath) {
            var correctionFactor = Mathf.Max(-Vector3.Dot(prevTangent.normalized, interpolator.tangent.normalized), 0);
            interpolator.distance -= speed * correctionFactor * (1f / switchPathInterpolationSpeed);
        }
    }

    protected virtual void Update() {
        if (canMove) {
            Vector3 direction;
            Vector3 nextPos = CalculateNextPosition(out direction);

            // Rotate unless we are really close to the target
            //除非我们真的接近目标，否则旋转
            if (enableRotation && direction != Vector3.zero) {
                if (rotationIn2D) {
                    float angle = Mathf.Atan2(direction.x, -direction.y) * Mathf.Rad2Deg + 180;
                    Vector3 euler = tr.eulerAngles;
                    euler.z = Mathf.LerpAngle(euler.z, angle, Time.deltaTime * rotationSpeed);
                    //tr.eulerAngles = euler;

                    DirPos = (Vector2)direction;
                    //Debug.Log("dir=" + gameObject.GetComponent<AI>().dir);
                } else {
                    Quaternion rot = tr.rotation;
                    Quaternion desiredRot = Quaternion.LookRotation(direction);

                    tr.rotation = Quaternion.Slerp(rot, desiredRot, Time.deltaTime * rotationSpeed);

                }
            }
            //发现这个处理不能用直接位置z轴的方式处理，需要做一做一个替身带动
            transform.parent.gameObject.GetComponent<AI>().nexPos = nextPos;

            tr.position = nextPos;

            
            
        }
    }

    /** Calculate the AI's next position (one frame in the future).
     * 计算AI的下一个位置（未来一帧）。
	 * \param direction The tangent of the segment the AI is currently traversing. Not normalized.
     * \参数方向AI当前正在运行的线段的切线。 没有标准化。
	 */
    protected virtual Vector3 CalculateNextPosition(out Vector3 direction) {
        if (!interpolator.valid) {
            direction = Vector3.zero;
            return tr.position;
        }

        interpolator.distance += Time.deltaTime * speed;
        if (interpolator.remainingDistance < 0.0001f && !targetReached) {
            targetReached = true;
            OnTargetReached();
        }

        direction = interpolator.tangent;
        float alpha = switchPathInterpolationSpeed * (Time.time - previousMovementStartTime);
        if (interpolatePathSwitches && alpha < 1f) {
            // Find the approximate position we would be at if we
            //找到我们所处的大概位置
            // would have continued to follow the previous path
            //将继续沿着之前的路线前进
            Vector3 positionAlongPreviousPath = previousMovementOrigin + Vector3.ClampMagnitude(previousMovementDirection, speed * (Time.time - previousMovementStartTime));

            // Interpolate between the position on the current path and the position
            // 在当前路径上的位置和位置之间进行插值
            // we would have had if we would have continued along the previous path.
            // 如果我们继续沿着前面的道路走，我们会有的。
            return Vector3.Lerp(positionAlongPreviousPath, interpolator.position, alpha);
        } else {
            return interpolator.position;
        }
    }
   
}
