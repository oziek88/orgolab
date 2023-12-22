using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MouseManager:MonoBehaviour {
    public delegate void MouseManagerEventHandler(MouseManager mouseManager, List<GameObject> hits);
    public static event MouseManagerEventHandler onTwoFingerTouch;

    private float dragSpeed = 3f;
    private float maxMoveSpeed = 3;
    private float smoothTime = 0.5f;

    private Vector2 currentVelocity;
    private Rigidbody2D grabbedObject = null;
    private GameObject grabPoint = null;

    private float clicked = 0;
    private float clicktime = 0;
    private float clickdelay = 0.5f;

    private bool deleteMode = false;

    private void Awake() {
        OptionsPanel.onDeleteModeToggle += UpdateDeleteMode;
    }

    void Update() {
        if(!deleteMode) {
            // two finger tap for breaking bonds
            if(Input.touchCount == 2) {
                if(!SolutionController.Instance.ionsInSolution) {
                    List<GameObject> hits = TwoFingerTouch();
                    onTwoFingerTouch?.Invoke(this, hits);
                }
            }

            // single click for dragging molecules
            if(Input.GetMouseButtonDown(0)) {
                if(grabPoint == null) {
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
                        grabPoint = new GameObject("grabPoint");
                        grabPoint.transform.position = mousePos2D;
                    }
                }
            }

            if(Input.GetMouseButtonUp(0)) {
                if(grabbedObject != null) {
                    grabbedObject = null;
                }
                Destroy(grabPoint);
            }

            if(grabbedObject != null) {
                Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                grabPoint.transform.position = Vector2.SmoothDamp(grabPoint.transform.position, mousePosition, ref currentVelocity, smoothTime, maxMoveSpeed);

                Vector2 dir = mousePosition - (Vector2)grabPoint.transform.position;
                dir *= dragSpeed;
                grabbedObject.velocity = dir;
            }
        } else {
            // if delete mode
            if(Input.GetMouseButtonDown(0)) {
                Vector3 mouseWorldPos3D = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                Vector2 mousePos2D = new Vector2(mouseWorldPos3D.x, mouseWorldPos3D.y);
                Vector2 dir = Vector2.zero;
                RaycastHit2D hit = Physics2D.Raycast(mousePos2D, dir);
                if(hit && hit.collider != null) {
                    Destroy(hit.transform.gameObject);
                }
            }
        }
    }

    

    public void OnPointerDown(PointerEventData data) {
        clicked++;
        if(clicked == 1)
            clicktime = Time.time;

        if(clicked > 1 && Time.time - clicktime < clickdelay) {
            clicked = 0;
            clicktime = 0;
            Debug.Log("Double CLick: " + this.GetComponent<RectTransform>().name);

        } else if(clicked > 2 || Time.time - clicktime > 1) {
            clicked = 0;
        }
    }

    //public void OnPointerUp(PointerEventData data) {
    //    if(grabbedObject != null) {
    //        grabbedObject = null;
    //    }
    //    Destroy(grabPoint);
    //}

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
            if(hit1.collider.transform.GetComponentInParent<Atom>() != null) {
                hits.Add(hit1.collider.transform.GetComponentInParent<Atom>().gameObject);
            } else if(hit1.collider.transform.GetComponentInParent<Bond>() != null) {
                hits.Add(hit1.collider.transform.GetComponentInParent<Bond>().gameObject);
            }
        }

        if(hit2.collider != null) {
            if(hit2.collider.transform.GetComponentInParent<Atom>() != null) {
                hits.Add(hit2.collider.transform.GetComponentInParent<Atom>().gameObject);
            } else if(hit2.collider.transform.GetComponentInParent<Bond>() != null) {
                hits.Add(hit2.collider.transform.GetComponentInParent<Bond>().gameObject);
            }
        }

        return hits;
    }

    private void UpdateDeleteMode(OptionsPanel op, bool value) {
        deleteMode = value;
    }

    private void OnDestroy() {
        OptionsPanel.onDeleteModeToggle -= UpdateDeleteMode;
    }
}
