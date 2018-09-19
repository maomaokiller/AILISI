using UnityEngine;

namespace Pathfinding.Examples {
	/** Example script used in the example scenes */
	[HelpURL("http://arongranberg.com/astar/docs/class_pathfinding_1_1_examples_1_1_door_controller.php")]
	public class BoxController : MonoBehaviour {
		private bool open = false;

		public int opentag = 1;
		public int closedtag = 1;
		public bool updateGraphsWithGUO = true;
		public float yOffset = 5;

		Bounds bounds;

		public void Start () {
            // Capture the bounds of the collider while it is closed
            //关闭时捕获对撞机的边界
            //bounds = GetComponent<BoxCollider2D>().bounds;
            bounds = GetComponent<BoxCollider2D>().bounds;

            //默认关闭
            SetState(open);
		}

		//void OnGUI () {
		//	// Show a UI button for opening and closing the door
		//	if (GUI.Button(new Rect(5, yOffset, 100, 22), "Toggle Door")) {
		//		SetState(!open);
		//	}
		//}

		public void SetState (bool open) {
			this.open = open;

			if (updateGraphsWithGUO) {
                // Update the graph below the door
                //更新门下的图表
                // Set the tag of the nodes below the door
                //设置门下节点的标签
                // To something indicating that the door is open or closed
                GraphUpdateObject guo = new GraphUpdateObject(bounds);
				int tag = open ? opentag : closedtag;

				// There are only 32 tags
				if (tag > 31) { Debug.LogError("tag > 31"); return; }

				guo.modifyTag = true;
				guo.setTag = tag;
				guo.updatePhysics = false;

				AstarPath.active.UpdateGraphs(guo);
			}

			//// Play door animations
			//if (open) {
			//	GetComponent<Animation>().Play("Open");
			//} else {
			//	GetComponent<Animation>().Play("Close");
			//}
		}
	}
}
