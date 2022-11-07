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

                if(hit.collider.GetComponent<Rigidbody2D>() != null) {

                    if(hit.collider.gameObject.transform.parent) {
                        grabbedObject = hit.collider.gameObject.transform.parent.GetComponent<Rigidbody2D>();
                    } else {
                        grabbedObject = hit.collider.GetComponent<Rigidbody2D>();
                    }
                    
                    //grabbedObject.gravityScale = 0;
                }
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

}
