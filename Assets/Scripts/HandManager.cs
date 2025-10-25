using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OculusSampleFramework;

public class HandManager : MonoBehaviour
{
    public GameObject hand;
    private OVRHand ovrHand;
    public GameObject controller;

    public float grabRadius = 0.1f; // Radius to detect objects
    public LayerMask grabbableLayer;
    private GameObject heldObject = null;

    private Transform grabPoint;

    void Start()
    {
        ovrHand = hand.GetComponent<OVRHand>();
        grabPoint = new GameObject("GrabPoint").transform;
        grabPoint.SetParent(hand.transform);
        grabPoint.localPosition = Vector3.zero;
    }

    void Update()
    {
        if (ovrHand.IsTracked)
        {
            hand.transform.GetChild(0).gameObject.SetActive(true);
            controller.SetActive(false);

            bool isPinching = ovrHand.GetFingerIsPinching(OVRHand.HandFinger.Index);

            if (isPinching)
            {
                if (heldObject == null)
                {
                    Collider[] hits = Physics.OverlapSphere(grabPoint.position, grabRadius, grabbableLayer);

                    if (hits.Length > 0)
                    {
                        heldObject = hits[0].gameObject;
                        heldObject.transform.SetParent(grabPoint);
                        Rigidbody rb = heldObject.GetComponent<Rigidbody>();
                        if (rb) rb.isKinematic = true;
                    }
                }
            }
            else
            {
                if (heldObject != null)
                {
                    Rigidbody rb = heldObject.GetComponent<Rigidbody>();
                    if (rb) rb.isKinematic = false;
                    heldObject.transform.SetParent(null);
                    heldObject = null;
                }
            }
        }
        else if (OVRInput.IsControllerConnected(OVRInput.Controller.LTouch))
        {
            controller.SetActive(true);
            hand.transform.GetChild(0).gameObject.SetActive(false);
        }
    }

    void OnDrawGizmosSelected()
    {
        if (grabPoint != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(grabPoint.position, grabRadius);
        }
    }
}
