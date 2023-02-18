using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingleBond : MonoBehaviour {
    public GameObject group1;
    public GameObject group2;
    public bool canBreak = false;

    private Vector2 v1 = new Vector2(-60f, 5f);
    private Vector2 v2 = new Vector2(60f, -5f);

    private Animation anim;

    private void Start() {
        anim = GetComponentInChildren<Animation>();
    }
    public void BondCanBreak(bool value) {
        canBreak = value;
    }

    public void BondBreak() {
        if(canBreak) {
            //yield return new WaitForSeconds(1f);
            anim.Play("bondStretch");

            Rigidbody2D rigid1 = group1.AddComponent<Rigidbody2D>();
            Rigidbody2D rigid2 = group2.AddComponent<Rigidbody2D>();

            rigid1.gravityScale = 0;
            rigid2.gravityScale = 0;
            rigid1.drag = 1;
            rigid2.drag = 1;

            rigid1.AddForce(v1);
            rigid2.AddForce(v2);
            // return new WaitForSeconds(0.5f);

            group1.transform.parent = null;
            group2.transform.parent = null;
            Destroy(transform.parent.gameObject);
        }
    }
}
