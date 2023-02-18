using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseManager:MonoBehaviour {

    float dragSpeed = 8f;

    Rigidbody2D grabbedObject = null;

    void Update() {
        if(Input.GetMouseButtonDown(0)) {
            // we clicked, but on what?
            Vector3 mouseWorldPos3D = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 mousePos2D = new Vector2(mouseWorldPos3D.x, mouseWorldPos3D.y);

            Vector2 dir = Vector2.zero;

            RaycastHit2D hit = Physics2D.Raycast(mousePos2D, dir);
            if(hit && hit.collider != null) {
                // we clicked on SOMETHING that has a collider

                //if(hit.collider.transform.parent?.GetComponent<Rigidbody2D>() != null) {
                //    grabbedObject = hit.collider.transform.parent.GetComponent<Rigidbody2D>();
                //} else if(hit.collider.GetComponent<Rigidbody2D>() != null) { 
                //    grabbedObject = hit.collider.GetComponent<Rigidbody2D>();
                //}

                grabbedObject = hit.collider.GetComponentInParent<Rigidbody2D>();
            }
        }

        if(Input.GetMouseButtonUp(0)) {
            grabbedObject = null;
        }
    }

    void FixedUpdate() {
        if(grabbedObject != null) {
            // move object with mouse
            Vector3 mouseWorldPos3D = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 mousePos2D = new Vector2(mouseWorldPos3D.x - 0.5f, mouseWorldPos3D.y + 0.5f);

            Vector2 dir = mousePos2D - grabbedObject.position;

            dir *= dragSpeed; // dir multiplied by dragSpeed (10f) 

            grabbedObject.velocity = dir;
        }
    }

    void LateUpdate() {
        if(grabbedObject != null) {
            Vector3 mouseWorldPos3D = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 mousePos2D = new Vector2(mouseWorldPos3D.x, mouseWorldPos3D.y);

        }
    }

    public List<GameObject> TwoFingerTouch() {
        List<GameObject> hits = new List<GameObject>();
        // Store both touches.
        Touch touchOne = Input.GetTouch(0);
        Touch touchTwo = Input.GetTouch(1);
        // Get World Position of both touches
        Vector3 touchOneWorldPos3D = Camera.main.ScreenToWorldPoint(touchOne.position);
        Vector3 touchTwoWorldPos3D = Camera.main.ScreenToWorldPoint(touchTwo.position);
        // Convert touches to 2D
        Vector2 touchOne2D = new Vector2(touchOneWorldPos3D.x, touchOneWorldPos3D.y);
        Vector2 touchTwo2D = new Vector2(touchTwoWorldPos3D.x, touchTwoWorldPos3D.y);

        Vector2 dir = Vector2.zero;

        // Send out ray
        RaycastHit2D hit1 = Physics2D.Raycast(touchOne2D, dir);
        RaycastHit2D hit2 = Physics2D.Raycast(touchTwo2D, dir);

        if(hit1.collider != null) {
            hits.Add(hit1.transform.gameObject);
        }

        if(hit2.collider != null) {
            hits.Add(hit2.transform.gameObject);
        }

        return hits;
    }

}
