using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bond : MonoBehaviour {
    public enum BondType { Single, Double };
    public BondType currentBondType;

    public Atom atom1;
    public Atom atom2;

    public GameObject group1;
    public GameObject group2;

    public SpriteRenderer spriteRenderer;
    public Sprite singleBond;
    public Sprite doubleBond;

    public bool canBreak = false;

    private Vector2 atom1_Pivot;
    //public Vector2 atom1_ColliderOffset;
    private Vector2 atom2_Pivot;
    //public Vector2 atom2_ColliderOffset;
    //private Vector2 colliderOffset = 

    //private Vector2 v1 = new Vector2(-60f, 5f);
    //private Vector2 v2 = new Vector2(60f, -5f);
    private float forceMagnitude = 30;

    private RectTransform rect;
    private Animation anim;
    

    private void Start() {
        rect = GetComponent<RectTransform>();
        anim = GetComponentInChildren<Animation>();

        SetBondPivots();
    }
    public void BondCanBreak(bool value) {
        canBreak = value;
    }

    public void SetBondPivots() {
        //CapsuleCollider2D collider = GetComponent<CapsuleCollider2D>();
        float atom1_pivotX = atom1.pivot;
        float atom2_pivotX = atom2.pivot;

        atom1_Pivot = new Vector2(atom1_pivotX, 0);
        //atom1_ColliderOffset = new Vector2(collider.offset.x - (atom1_pivotX * 2), 0);

        atom2_Pivot = new Vector2(1 - atom2_pivotX, 0);
        //atom2_ColliderOffset = new Vector2(collider.offset.x - 2 + (atom2_pivotX * 2), 0);
    }

    /// <summary>
    /// adds or removes bond number by one
    /// </summary>
    /// <param name="add">add = true will make a single go to double, ect</param>
    public void ChangeBondType(bool add) {
        switch(currentBondType) {
            case BondType.Single:
                if(add) {
                    spriteRenderer.sprite = doubleBond;
                    currentBondType = BondType.Double;
                }
                break;
            case BondType.Double:
                if(!add) {
                    spriteRenderer.sprite = singleBond;
                    currentBondType = BondType.Single;
                }
                break;
        }
    }

    public void BondBreak() {
        if(canBreak) {
            //yield return new WaitForSeconds(1f);
            anim.Play("bondStretch");

            Vector3 v1 = (group1.transform.position - group2.transform.position) * forceMagnitude;
            Vector3 v2 = (group2.transform.position - group1.transform.position) * forceMagnitude;

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
