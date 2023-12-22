using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bond : MonoBehaviour {
    public enum BondType { Single, Double, Triple };
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

    private Vector2 parentCurrentVelocity;


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
        //yield return new WaitForSeconds(1f);
        anim.Play("bondStretch");
        GameObject parent = transform.parent.gameObject;
        if(group1 == null) {
            group1 = atom1.gameObject;
        }

        group1.transform.parent = newParent;
        transform.parent = newParent;
        

        int childCount = 0;
        foreach(Transform child in atom2.transform.parent) {
            childCount += 1;
            if(childCount > 1) {
                break;
            }
        }
        if(childCount == 1) {
            atom2.transform.parent = newParent;
            Destroy(parent);
        }

        Rigidbody2D rigid1 = group1.GetComponent<Rigidbody2D>();
        if(rigid1 == null) {
            rigid1 = group1.AddComponent<Rigidbody2D>();
        }
        Rigidbody2D rigid2 = atom2.gameObject.transform.parent.GetComponent<Rigidbody2D>();
        if(rigid2 == null) {
            rigid2 = atom2.gameObject.AddComponent<Rigidbody2D>();
        }

        //Vector3 v1 = (group1.transform.position - group2.transform.position) * forceMagnitude;
        //Vector3 v2 = (group2.transform.position - group1.transform.position) * forceMagnitude;

        Vector3 v1 = (atom1.transform.position - atom2.transform.position) * forceMagnitude;
        Vector3 v2 = (atom2.transform.position - atom1.transform.position) * forceMagnitude;


        //Rigidbody2D rigid1 = group1.AddComponent<Rigidbody2D>();
        //Rigidbody2D rigid2 = group2.AddComponent<Rigidbody2D>();

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

        atom1.ToggleBonding(atom1CanBond);
        atom2.ToggleBonding(atom2CanBond);
        atom1.bondCount -= 1;
        atom2.bondCount -= 1;

        SolutionController.Instance.AddToSolution(new List<Solute>() { atom1, atom2 });
        Destroy(gameObject);
    }

    public Bond CreateNeighboringBond(BondType newBondType, GameObject existingGroup, GameObject attachingGroup, Atom existingAtom, Atom attachingAtom) {
        GameObject newBondGO = Instantiate(gameObject, transform.parent);
        Bond newBond = newBondGO.GetComponent<Bond>();
        if(attachingGroup.GetComponent<Rigidbody2D>() != null) {
            Destroy(attachingGroup.GetComponent<Rigidbody2D>());
        }
        
        if(newBond.currentBondType != newBondType) {
            newBond.ChangeBondType(newBondType);
        }

        newBond.transform.position = transform.position;

        bool existingIsAtom1 = ReferenceEquals(existingAtom, atom1);
        newBond.SetPivot(newBond, existingIsAtom1 ? atom1_Pivot : atom2_Pivot);

        float bondAngle = existingAtom.bondCount < 3 ? 120 : 60;
        newBond.GetComponent<RectTransform>().localEulerAngles = new Vector3(0, 0, GetComponent<RectTransform>().localEulerAngles.z + bondAngle);

        if(existingAtom.bondCount == 2) {
            foreach(Transform child in existingGroup.transform) {
                if(child.GetComponent<Bond>() != null) {
                    Bond b = child.GetComponent<Bond>();
                    if(b.transform.position == newBond.transform.position && !ReferenceEquals(newBond, b)) {
                        newBond.GetComponent<RectTransform>().localEulerAngles = new Vector3(0, 0, newBond.GetComponent<RectTransform>().localEulerAngles.z + bondAngle);
                        break;
                    }
                }
            }
        }

        SetNewBond(newBond, attachingAtom, !existingIsAtom1, bondAngle);
        existingAtom.bondCount += 1;
        attachingAtom.bondCount += 1;

        Rigidbody2D rigidbody = newBond.GetComponentInParent<Rigidbody2D>();
        rigidbody.velocity = (SolutionController.Instance.centerOfContainer + rigidbody.worldCenterOfMass) / -2;
        rigidbody.angularVelocity = Random.Range(-600f, 600f);
        
        return newBond;
    }

    private void SetNewBond(Bond newBond, Atom attachingAtom, bool isAtom1, float bondAngle) {
        if(attachingAtom.transform.parent.GetComponent<SolutionController>() == null) {
            if(isAtom1) {
                newBond.atom1 = attachingAtom;
                newBond.group1 = attachingAtom.transform.parent.gameObject;
            } else {
                newBond.atom2 = attachingAtom;
                newBond.group2 = attachingAtom.transform.parent.gameObject;
            }
        } else {
            if(isAtom1) {
                newBond.atom1 = attachingAtom;
                newBond.group1 = attachingAtom.gameObject;
            } else {
                newBond.atom2 = attachingAtom;
                newBond.group2 = attachingAtom.gameObject;
            }
        }

        newBond.SetBondPivots();
        newBond.SetPivot(newBond, isAtom1 ? newBond.atom1_Pivot : newBond.atom2_Pivot);

        GameObject attachingGroup = isAtom1 ? newBond.group1 : newBond.group2;
        Vector3 scale = attachingGroup.transform.localScale;
        attachingGroup.transform.parent = newBond.transform;
        attachingGroup.transform.localPosition = new Vector3(0, 0, 0);
        attachingGroup.transform.rotation = Quaternion.identity;
        attachingGroup.transform.parent = newBond.transform.parent;
        attachingGroup.transform.localScale = scale;
    }

    public void SetPivot(Bond bond, Vector2 pivot) {
        RectTransform rectTransform = bond.GetComponent<RectTransform>();
        if(rectTransform == null) {
            return;
        }

        Vector3 newAnch = GetNewAnchor(rectTransform, pivot);
        rectTransform.pivot = pivot;
        rectTransform.anchoredPosition = newAnch;
    }

    private Vector3 GetNewAnchor(RectTransform rt, Vector2 newPivot) {
        Vector3 op = new Vector3(rt.rect.width * newPivot.x - rt.rect.width * rt.pivot.x, rt.rect.height * newPivot.y - rt.rect.height * rt.pivot.y, 0);
        Vector3 pt = rt.TransformPoint(op);
        return rt.parent.InverseTransformPoint(pt);
    }
}
