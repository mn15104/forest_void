using System.Collections.Generic;
using UnityEngine;

public class RetreiveKey : OVRGrabber 
{

    public GameObject key;
    public Transform headTransform;

    protected override void Awake()
    {
        m_anchorOffsetPosition = transform.localPosition;
        m_anchorOffsetRotation = transform.localRotation;

        // If we are being used with an OVRCameraRig, let it drive input updates, which may come from Update or FixedUpdate.

        OVRCameraRig rig = null;
        if (transform.parent != null && transform.parent.parent != null)
            rig = transform.parent.parent.GetComponent<OVRCameraRig>();

        if (rig != null)
        {
            rig.UpdatedAnchors += (r) => { OnUpdatedAnchors(); };
            operatingWithoutOVRCameraRig = false;
        }
    }

    protected override void Start()
    {
        m_lastPos = transform.position;
        m_lastRot = transform.rotation;
        if (m_parentTransform == null)
        {
            if (gameObject.transform.parent != null)
            {
                m_parentTransform = gameObject.transform.parent.transform;
            }
            else
            {
                m_parentTransform = new GameObject().transform;
                m_parentTransform.position = Vector3.zero;
                m_parentTransform.rotation = Quaternion.identity;
            }
        }
    }

    protected override void FixedUpdate()
    {
        if (operatingWithoutOVRCameraRig)
            OnUpdatedAnchors();
        CheckHandInPocket();
    }

    // Hands follow the touch anchors by calling MovePosition each frame to reach the anchor.
    // This is done instead of parenting to achieve workable physics. If you don't require physics on 
    // your hands or held objects, you may wish to switch to parenting.
    protected override void OnUpdatedAnchors()
    {
        
        Vector3 handPos = OVRInput.GetLocalControllerPosition(m_controller);
        Quaternion handRot = OVRInput.GetLocalControllerRotation(m_controller);
        Vector3 destPos = m_parentTransform.TransformPoint(m_anchorOffsetPosition + handPos);
        Quaternion destRot = m_parentTransform.rotation * handRot * m_anchorOffsetRotation;
        GetComponent<Rigidbody>().MovePosition(destPos);
        GetComponent<Rigidbody>().MoveRotation(destRot);

        if (!m_parentHeldObject)
        {
            MoveGrabbedObject(destPos, destRot);
        }
        m_lastPos = transform.position;
        m_lastRot = transform.rotation;

        float prevFlex = m_prevFlex;
        // Update values from inputs
        m_prevFlex = OVRInput.Get(OVRInput.Axis1D.PrimaryHandTrigger, m_controller);

        CheckForGrabOrRelease(prevFlex);
    
    }


    protected override void CheckForGrabOrRelease(float prevFlex)
    {
        if ((m_prevFlex >= grabBegin) && (prevFlex < grabBegin) && CheckHandInPocket())
        {
            KeyAppear();
            OVRGrabbable grabbable = key.GetComponent<OVRGrabbable>() ?? key.GetComponentInParent<OVRGrabbable>();
            if (grabbable == null) return;

            // Add the grabbable
            int refCount = 0;
            m_grabCandidates.TryGetValue(grabbable, out refCount);
            m_grabCandidates[grabbable] = refCount + 1;

            base.GrabBegin();
        }
        else if ((m_prevFlex <= grabEnd) && (prevFlex > grabEnd))
        {
            base.GrabEnd();
        }

    }

    void KeyAppear()
    {
        key.transform.position = transform.position;
    }

    bool CheckHandInPocket()
    {
        Debug.Log(headTransform.position.z - transform.position.z);
        float xDiff = headTransform.position.x - transform.position.x;
        float zDiff = headTransform.position.z - transform.position.z;
        //FOR SITTING DOWN
        return ((headTransform.position.y - transform.position.y) > 0.4) && (xDiff > -0.2)  && (xDiff < 0.2) && (zDiff > -0.2) && (zDiff < 0.4);
    }
}
