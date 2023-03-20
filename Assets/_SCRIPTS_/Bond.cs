using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bond : MonoBehaviour {
    public enum BondType { Single, Double };
    public BondType currentBondType;

    public GameObject bondPrefab;

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
    public void ChangeBondType(BondType newBondType) {
        if(newBondType == BondType.Single) {
            spriteRenderer.sprite = singleBond;
            currentBondType = BondType.Single;
        } else if(newBondType == BondType.Double) {
            spriteRenderer.sprite = doubleBond;
            currentBondType = BondType.Double;
        }
    }

    /// <summary>
    /// breaks bond between two atoms
    /// </summary>
    /// <param name="parent"></param>
    /// <param name="atom1Charge">"+" to place a positive charge, "-" for negative, "" for none</param>
    /// <param name="atom2Charge">"+" to place a positive charge, "-" for negative, "" for none</param>
    public void BondBreak(Transform newParent, bool atom1CanBond = false, string atom1Charge = "", bool atom2CanBond = false, string atom2Charge = "") {
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

            if(atom1Charge == "+") {
                atom1.charge.GivePositiveCharge();
            } else if(atom1Charge == "-") {
                atom1.charge.GiveNegativeCharge();
            }

            if(atom2Charge == "+") {
                atom2.charge.GivePositiveCharge();
            } else if(atom2Charge == "-") {
                atom2.charge.GiveNegativeCharge();
            }

            if(atom1CanBond) {
                atom1.ToggleBonding(true);
            } else {
                atom1.ToggleBonding(false);
            }

            if(atom2CanBond) {
                atom2.ToggleBonding(true);
            } else {
                atom2.ToggleBonding(false)
                ;
            }

            group1.transform.parent = newParent;
            group2.transform.parent = newParent;
            Destroy(transform.parent.gameObject);
        }
    }

    public void CreateNeighboringBond(BondType newBondType, GameObject existingGroup, GameObject attachingGroup, Atom existingAtom, Atom attachingAtom) {
        GameObject newBondGO = Instantiate(gameObject, transform.parent);
        Bond newBond = newBondGO.GetComponent<Bond>();
        Vector3 bondPosition = GetComponent<RectTransform>().localPosition;
        if(attachingGroup.GetComponent<Rigidbody2D>() != null) {
            Destroy(attachingGroup.GetComponent<Rigidbody2D>());
        }
        
        if(newBond.currentBondType != newBondType) {
            newBond.ChangeBondType(newBondType);
        }

        //newBond.spriteRenderer.enabled = false;
        newBondGO.transform.position = transform.position;
        Vector3 scale = attachingGroup.transform.localScale;

        if(ReferenceEquals(existingAtom, atom2)) {
            SetPivot(newBondGO.GetComponent<RectTransform>(), atom2_Pivot);
            newBondGO.GetComponent<RectTransform>().localEulerAngles = new Vector3(0, 0, GetComponent<RectTransform>().localEulerAngles.z - 120);
            attachingGroup.transform.parent = newBond.spriteRenderer.transform;
            attachingGroup.transform.localPosition = new Vector3(-2 + attachingAtom.pivot, 0, 0);
            attachingGroup.transform.rotation = Quaternion.identity;
        }

        if(ReferenceEquals(existingAtom, atom1)) {
            SetPivot(newBondGO.GetComponent<RectTransform>(), atom1_Pivot);
            newBondGO.GetComponent<RectTransform>().localEulerAngles = new Vector3(0, 0, GetComponent<RectTransform>().localEulerAngles.z + 120);
            attachingGroup.transform.parent = newBond.spriteRenderer.transform;
            attachingGroup.transform.localPosition = new Vector3(2 - attachingAtom.pivot, 0, 0);
            attachingGroup.transform.rotation = Quaternion.identity;
        }

        attachingGroup.transform.parent = newBondGO.transform.parent;
        attachingGroup.transform.localScale = scale;
        //newBond.spriteRenderer.enabled = true;
    }

    public static void SetPivot(RectTransform rectTransform, Vector2 pivot) {
        if(rectTransform == null) {
            return;
        }

        Vector2 size = rectTransform.rect.size;
        Vector3 scale = rectTransform.localScale;
        Vector2 deltaPivot = rectTransform.pivot - pivot;
        Vector3 deltaPosition = new Vector3(deltaPivot.x * size.x * scale.x, deltaPivot.y * size.y * scale.y);
        rectTransform.pivot = pivot;
        Debug.Log(deltaPosition);
        rectTransform.localPosition -= deltaPosition;
    }
}
