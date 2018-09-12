using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(SingleNodeBlocker))]
[HelpURL("http://arongranberg.com/astar/docs/class_turn_based_door.php")]
public class TurnBasedDoor : MonoBehaviour {
	Animator animator;
	SingleNodeBlocker blocker;

	bool open;

	void Awake () {
		animator = GetComponent<Animator>();
		blocker = GetComponent<SingleNodeBlocker>();
	}

	void Start () {
		// Make sure the door starts out blocked
		blocker.BlockAtCurrentPosition();
		animator.CrossFade("close", 0.2f);
	}

	public void Close () {
		StartCoroutine(WaitAndClose());
	}

	IEnumerator WaitAndClose () {
		var selector = new List<SingleNodeBlocker>() { blocker };
		var node = AstarPath.active.GetNearest(transform.position).node;

        // Wait while there is another SingleNodeBlocker occupying the same node as the door
        //等待另一个SingleNodeBlocker占用与门相同的节点
        // this is likely another unit which is standing on the door node, and then we cannot
        //这可能是站在门口的另一个单位，那么我们就不能
        // close the door
        // 关门
        if (blocker.manager.NodeContainsAnyExcept(node, selector)) {
			// Door is blocked
			animator.CrossFade("blocked", 0.2f);
		}

		while (blocker.manager.NodeContainsAnyExcept(node, selector)) {
			yield return null;
		}

		open = false;
		animator.CrossFade("close", 0.2f);
		blocker.BlockAtCurrentPosition();
	}

	public void Open () {
        // Stop WaitAndClose if it is running
        //停止WaitAndClose，如果它正在运行
        StopAllCoroutines();

		// Play the open door animation
		animator.CrossFade("open", 0.2f);
		open = true;

        // Unblock the door node so that units can traverse it again
        //解锁门节点，使单位可以再次穿过它
        blocker.Unblock();
	}

	public void Toggle () {
		if (open) {
			Close();
		} else {
			Open();
		}
	}
}
