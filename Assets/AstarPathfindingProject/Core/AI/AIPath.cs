using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Pathfinding;
using Pathfinding.RVO;
using Pathfinding.Util;

/** AI for following paths.
 * This AI is the default movement script which comes with the A* Pathfinding Project.
 * It is in no way required by the rest of the system, so feel free to write your own. But I hope this script will make it easier
 * to set up movement for the characters in your game.
 * This script works well for many types of units, but if you need the highest performance (for example if you are moving hundreds of characters) you
 * may want to customize this script or write a custom movement script to be able to optimize it specifically for your game.
 * \n
 * \n
 * This script will try to follow a target transform. At regular intervals, the path to that target will be recalculated.
 * It will in the #Update method try to move towards the next point in the path.
 * However it will only move in roughly forward direction (Z+ axis) of the character, but it will rotate around it's Y-axis
 * to make it possible to reach the target.
 *
 * \section variables Quick overview of the variables
 * In the inspector in Unity, you will see a bunch of variables. You can view detailed information further down, but here's a quick overview.\n
 * The #repathRate determines how often it will search for new paths, if you have fast moving targets, you might want to set it to a lower value.\n
 * The #target variable is where the AI will try to move, it can be a point on the ground where the player has clicked in an RTS for example.
 * Or it can be the player object in a zombie game.\n
 * The speed is self-explanatory, so is #rotationSpeed, however #slowdownDistance might require some explanation.
 * It is the approximate distance from the target where the AI will start to slow down.\n
 * #pickNextWaypointDist is simply determines within what range it will switch to target the next waypoint in the path.\n
 *
 * Below is an image illustrating several variables as well as some internal ones, but which are relevant for understanding how it works.
 * \note The image is slightly outdated, replace forwardLook with pickNextWaypointDist in the image and ignore the circle for pickNextWaypointDist.
 *
 * \shadowimage{aipath_variables.png}
 * This script has many movement fallbacks.
 * If it finds an RVOController attached to the same GameObject as this component, it will use that. If it fins a character controller it will also use that.
 * Lastly if will fall back to simply modifying Transform.position which is guaranteed to always work and is also the most performant option.
 */
[RequireComponent(typeof(Seeker))]
[AddComponentMenu("Pathfinding/AI/AIPath (2D,3D)")]
[HelpURL("http://arongranberg.com/astar/docs/class_a_i_path.php")]
public class AIPath : AIBase {
	/** Determines how often it will search for new paths.搜索新路径的频率
	 * If you have fast moving targets or AIs, you might want to set it to a lower value.
	 * The value is in seconds between path requests.
	 */
	public float repathRate = 0.5F;

	/** Target to move towards.目标
	 * The AI will try to follow/move towards this target.
	 * It can be a point on the ground where the player has clicked in an RTS for example, or it can be the player object in a zombie game.
	 */
	public Transform target;

	/** Enables or disables searching for paths.
	 * Setting this to false does not stop any active path requests from being calculated or stop it from continuing to follow the current path.
	 * \see #canMove
	 */
	public bool canSearch = true;

	/** Enables or disables movement.
	 * \see #canSearch
	 */
	public bool canMove = true;

	/** Maximum velocity.
	 * This is the maximum speed in world units per second.
	 */
	public float speed = 3;

	/** Rotation speed.
	 * Rotation is calculated using Quaternion.RotateTowards. This variable represents the rotation speed in degrees per second.
	 * The higher it is, the faster the character will be able to rotate.
	 */
	[UnityEngine.Serialization.FormerlySerializedAs("turningSpeed")]
	public float rotationSpeed = 360;

    /** Distance from the target point where the AI will start to slow down.
     * 与AI开始减速的目标点的距离。
	 * Note that this doesn't only affect the end point of the path
	 * but also any intermediate points, so be sure to set #pickNextWaypointDist to a higher value than this
	 */
    public float slowdownDistance = 0.6F;

	/** Determines within what range it will switch to target the next waypoint in the path */
    /** 确定在什么范围内切换到目标路径中的下一个航点 */
	public float pickNextWaypointDist = 2;

    /** Distance to the end point to consider the end of path to be reached.
     * 到终点的距离，以考虑到达路径的终点。
	 * When the end is within this distance then #OnTargetReached will be called and #TargetReached will return true.
	 */
    public float endReachedDistance = 0.2F;

	/** Draws detailed gizmos constantly in the scene view instead of only when the agent is selected and settings are being modified */
    /** 在场景视图中不断地绘制详细的小控件，而不是只在选择了代理并且正在修改设置的时候 */
	public bool alwaysDrawGizmos;

	/** Time when the last path request was sent */
    /** 最后一个路径请求发送的时间 */
	protected float lastRepath = -9999;

	/** Current path which is followed */
    /** 当前路径 */
	protected Path path;

	protected PathInterpolator interpolator = new PathInterpolator();

	/** Only when the previous path has been returned should be search for a new path */
    /** 只有当前一个路径已经返回应该搜索一个新的路径 */
	protected bool canSearchAgain = true;

	/** True if the end of the path has been reached */
    /** 如果到达路径的末端，则为真 */
	public bool TargetReached { get; protected set; }

    /** True if the Start function has been executed.
     * 如果启动功能已被执行则为真。
	 * Used to test if coroutines should be started in OnEnable to prevent calculating paths
     * 用于测试是否应在OnEnable中启动协程以防止计算路径
	 * in the awake stage (or rather before start on frame 0).
     * 在awake阶段（或者更确切地说，在0帧开始之前）。
	 */
    private bool startHasRun = false;

	/** Point to where the AI is heading */
    /** 指向AI的标题 */
	protected Vector3 targetPoint;

	protected Vector3 velocity;

	/** Rotation speed.
	 * \deprecated This field has been renamed to #rotationSpeed and is now in degrees per second instead of a damping factor.
	 */
	[System.Obsolete("This field has been renamed to #rotationSpeed and is now in degrees per second instead of a damping factor")]
	public float turningSpeed { get { return rotationSpeed/90; } set { rotationSpeed = value*90; } }

	/** Starts searching for paths.
	 * If you override this function you should in most cases call base.Start () at the start of it.
	 * \see #Init
	 */
	protected virtual void Start () {
		startHasRun = true;
		Init();
    }

	/** Called when the component is enabled */
    /** 启用组件时调用 */
	protected virtual void OnEnable () {
        // Make sure we receive callbacks when paths are calculated
        //确保在计算路径时收到回调
        seeker.pathCallback += OnPathComplete;
		Init();
	}

	void Init () {
		if (startHasRun) {
			lastRepath = float.NegativeInfinity;
			StartCoroutine(RepeatTrySearchPath());
		}
	}

	public void OnDisable () {
		seeker.CancelCurrentPathRequest();

        // Release current path so that it can be pooled
        //释放当前路径，使其可以合并
        if (path != null) path.Release(this);
		path = null;

        // Make sure we no longer receive callbacks when paths complete
        //确保路径完成后不再接收回调
        seeker.pathCallback -= OnPathComplete;
	}

    /** Tries to search for a path every #repathRate seconds.
     * 尝试每#repathRate秒搜索一个路径。
	 * \see TrySearchPath
	 */
    protected IEnumerator RepeatTrySearchPath () {
		while (true) yield return new WaitForSeconds(TrySearchPath());
	}

	/** Tries to search for a path.
	 * Will search for a new path if there was a sufficient time since the last repath and both
	 * #canSearchAgain and #canSearch are true and there is a target.
	 *
	 * \returns The time to wait until calling this function again (based on #repathRate)
	 */
	public float TrySearchPath () {
		if (Time.time - lastRepath >= repathRate && canSearchAgain && canSearch && target != null) {
			SearchPath();
			return repathRate;
		} else {
			float v = repathRate - (Time.time-lastRepath);
			return v < 0 ? 0 : v;
		}
    }

	/** Requests a path to the target */
    /** 请求一个路径到目标 */
	public virtual void SearchPath () {
		if (target == null) throw new System.InvalidOperationException("Target is null");

		lastRepath = Time.time;
        // This is where we should search to
        // 这是我们应该搜索的地方
        Vector3 targetPosition = target.position;

		canSearchAgain = false;

        // Alternative way of requesting the path
        //ABPath p = ABPath.Construct(GetFeetPosition(), targetPosition, null);
        //seeker.StartPath(p);

        // We should search from the current position
        // 我们应该从当前位置搜索
        seeker.StartPath(GetFeetPosition(), targetPosition);
	}

	public virtual void OnTargetReached () {
        // The end of the path has been reached.
        // 路径的末尾已经达到。
        // If you want custom logic for when the AI has reached it's destination
        // 当AI达到目的地时，如果你想要自定义逻辑
        // add it here.
        // You can also create a new script which inherits from this one
        // and override the function in that script
    }

    /** Called when a requested path has finished calculation.
	 * A path is first requested by #SearchPath, it is then calculated, probably in the same or the next frame.
	 * Finally it is returned to the seeker which forwards it to this function.\n
	 */
    public virtual void OnPathComplete (Path _p) {
		ABPath p = _p as ABPath;

		if (p == null) throw new System.Exception("This function only handles ABPaths, do not use special path types");

		canSearchAgain = true;

		// Claim the new path
		p.Claim(this);

		// Path couldn't be calculated of some reason.
		// More info in p.errorLog (debug string)
		if (p.error) {
			p.Release(this);
			return;
		}

		// Release the previous path
		if (path != null) path.Release(this);

		// Replace the old path
		path = p;

		// Make sure the path contains at least 2 points
		if (path.vectorPath.Count == 1) path.vectorPath.Add(path.vectorPath[0]);
		interpolator.SetPath(path.vectorPath);

		var graph = AstarData.GetGraph(path.path[0]) as ITransformedGraph;
		movementPlane = graph != null ? graph.transform : GraphTransform.identityTransform;

		// Reset some variables
		TargetReached = false;

		// Simulate movement from the point where the path was requested
		// to where we are right now. This reduces the risk that the agent
		// gets confused because the first point in the path is far away
		// from the current position (possibly behind it which could cause
		// the agent to turn around, and that looks pretty bad).
		interpolator.MoveToLocallyClosestPoint((GetFeetPosition() + p.originalStartPoint) * 0.5f);
		interpolator.MoveToLocallyClosestPoint(GetFeetPosition());
	}

	public virtual Vector3 GetFeetPosition () {
		if (rvoController != null && rvoController.enabled && rvoController.movementPlane == MovementPlane.XZ) {
			return tr.position + tr.up*(rvoController.center - rvoController.height*0.5f);
		}
		if (controller != null && controller.enabled) {
			return tr.TransformPoint(controller.center) - Vector3.up*controller.height*0.5F;
		}

		return tr.position;
    }

	/** Called during either Update or FixedUpdate depending on if rigidbodies are used for movement or not */
    /** 在更新或固定更新过程中调用，取决于刚体是否用于运动 */
	protected override void MovementUpdate (float deltaTime) {
		if (!canMove) return;
        //
		if (!interpolator.valid) {
			velocity2D = Vector3.zero;
		} else {
			var currentPosition = tr.position;

			interpolator.MoveToLocallyClosestPoint(currentPosition, true, false);
			interpolator.MoveToCircleIntersection2D(currentPosition, pickNextWaypointDist, movementPlane);
			targetPoint = interpolator.position;
			var dir = movementPlane.ToPlane(targetPoint-currentPosition);

			var distanceToEnd = dir.magnitude + interpolator.remainingDistance;
			// How fast to move depending on the distance to the target.
			// Move slower as the character gets closer to the target.
			float slowdown = slowdownDistance > 0 ? distanceToEnd / slowdownDistance : 1;

			// a = v/t, should probably expose as a variable
			float acceleration = speed / 0.4f;
			velocity2D += MovementUtilities.CalculateAccelerationToReachPoint(dir, dir.normalized*speed, velocity2D, acceleration, speed) * deltaTime;
			velocity2D = MovementUtilities.ClampVelocity(velocity2D, speed, slowdown, true, movementPlane.ToPlane(rotationIn2D ? tr.up : tr.forward));

			ApplyGravity(deltaTime);

			if (distanceToEnd <= endReachedDistance && !TargetReached) {
				TargetReached = true;
				OnTargetReached();
			}

			// Rotate towards the direction we are moving in
			var currentRotationSpeed = rotationSpeed * Mathf.Clamp01((Mathf.Sqrt(slowdown) - 0.3f) / 0.7f);
			RotateTowards(velocity2D, currentRotationSpeed * deltaTime);

			if (rvoController != null && rvoController.enabled) {
				// Send a message to the RVOController that we want to move
				// with this velocity. In the next simulation step, this
				// velocity will be processed and it will be fed back to the
				// rvo controller and finally it will be used by this script
				// when calling the CalculateMovementDelta method below

				// Make sure that we don't move further than to the end point
				// of the path. If the RVO simulation FPS is low and we did
				// not do this, the agent might overshoot the target a lot.
				var rvoTarget = currentPosition + movementPlane.ToWorld(Vector2.ClampMagnitude(velocity2D, distanceToEnd), 0f);
				rvoController.SetTarget(rvoTarget, velocity2D.magnitude, speed);
			}
			var delta2D = CalculateDeltaToMoveThisFrame(movementPlane.ToPlane(currentPosition), distanceToEnd, deltaTime);
			Move(currentPosition, movementPlane.ToWorld(delta2D, verticalVelocity * deltaTime));

			velocity = movementPlane.ToWorld(velocity2D, verticalVelocity);
		}
	}

	/** Direction that the agent wants to move in (excluding physics and local avoidance).
	 * \deprecated Only exists for compatibility reasons.
	 */
	[System.Obsolete("Only exists for compatibility reasons.")]
	public Vector3 targetDirection {
		get {
			return (targetPoint - tr.position).normalized;
		}
	}

	/** Current desired velocity of the agent (excluding physics and local avoidance but it includes gravity).
	 * \deprecated This method no longer calculates the velocity. Use the #velocity property instead.
	 */
	[System.Obsolete("This method no longer calculates the velocity. Use the velocity property instead")]
	public Vector3 CalculateVelocity (Vector3 position) {
		return velocity;
	}

#if UNITY_EDITOR
	[System.NonSerialized]
	int gizmoHash = 0;

	[System.NonSerialized]
	float lastChangedTime = float.NegativeInfinity;

	protected static readonly Color GizmoColor = new Color(46.0f/255, 104.0f/255, 201.0f/255);

	protected override void OnDrawGizmos () {
		base.OnDrawGizmos();
		if (alwaysDrawGizmos) OnDrawGizmosInternal();
	}

	void OnDrawGizmosSelected () {
		if (!alwaysDrawGizmos) OnDrawGizmosInternal();
	}

	void OnDrawGizmosInternal () {
		var newGizmoHash = pickNextWaypointDist.GetHashCode() ^ slowdownDistance.GetHashCode() ^ endReachedDistance.GetHashCode();

		if (newGizmoHash != gizmoHash && gizmoHash != 0) lastChangedTime = Time.realtimeSinceStartup;
		gizmoHash = newGizmoHash;
		float alpha = alwaysDrawGizmos ? 1 : Mathf.SmoothStep(1, 0, (Time.realtimeSinceStartup - lastChangedTime - 5f)/0.5f) * (UnityEditor.Selection.gameObjects.Length == 1 ? 1 : 0);

		if (alpha > 0) {
			// Make sure the scene view is repainted while the gizmos are visible
			if (!alwaysDrawGizmos) UnityEditor.SceneView.RepaintAll();
			if (targetPoint != Vector3.zero) Draw.Gizmos.Line(transform.position, targetPoint, GizmoColor * new Color(1, 1, 1, alpha));
			Gizmos.matrix = Matrix4x4.TRS(transform.position, transform.rotation, Vector3.one);
			Draw.Gizmos.CircleXZ(Vector3.zero, pickNextWaypointDist, GizmoColor * new Color(1, 1, 1, alpha));
			Draw.Gizmos.CircleXZ(Vector3.zero, slowdownDistance, Color.Lerp(GizmoColor, Color.red, 0.5f) * new Color(1, 1, 1, alpha));
			Draw.Gizmos.CircleXZ(Vector3.zero, endReachedDistance, Color.Lerp(GizmoColor, Color.red, 0.8f) * new Color(1, 1, 1, alpha));
		}
	}
#endif

	protected override int OnUpgradeSerializedData (int version) {
		// Approximately convert from a damping value to a degrees per second value.
		if (version < 1) rotationSpeed *= 90;
		return 1;
	}
}
