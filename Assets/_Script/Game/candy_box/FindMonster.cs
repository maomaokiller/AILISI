using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FindMonster : MonoBehaviour {

    void Awake()
    {
        gameObject.layer = 12;

        gameObject.tag = "candy_layer";

        gameObject.transform.position = transform.position;

        gameObject.transform.parent = transform;

        BoxCollider2D boxCollider2D = gameObject.AddComponent<BoxCollider2D>();

        boxCollider2D.size = new Vector2(1f, 0.7f);

        boxCollider2D.offset = new Vector2(0f, 0f);

        boxCollider2D.isTrigger = true;
    }

}
