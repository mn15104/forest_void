using System.Collections.Generic;
using UnityEngine;

public class RetreiveKey : OVRGrabber 
{

    
    public Transform headTransform;

    private GameObject human;
    private GameObject key;

    protected override void Start()
    {
        base.Start();
        human = transform.root.gameObject;
    }

    protected override void FixedUpdate()
    {
        if (operatingWithoutOVRCameraRig)
            OnUpdatedAnchors();
     
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
        if ((m_prevFlex >= grabBegin) && (prevFlex < grabBegin))
        {
            if (CheckHandInPocket() && human.GetComponent<Inventory>().inventorySize() > 0)
            {
                KeyAppear();
                OVRGrabbable grabbable = key.GetComponent<OVRGrabbable>() ?? key.GetComponentInParent<OVRGrabbable>();
                if (grabbable == null) return;

                // Add the grabbable
                int refCount = 0;
                m_grabCandidates.TryGetValue(grabbable, out refCount);
                m_grabCandidates[grabbable] = refCount + 1;
            }

            base.GrabBegin();
        }
        else if ((m_prevFlex <= grabEnd) && (prevFlex > grabEnd))
        {
            base.GrabEnd();
        }

    }

    void KeyAppear()
    {
        GetComponent<OculusHaptics>().Vibrate(VibrationForce.Hard);
        key = human.GetComponent<Inventory>().peekInventory();
        key.transform.position = transform.position;
        key.transform.rotation = m_lastRot;
        key.transform.Rotate(new Vector3(90, 90, 0));
       

        
    }

    bool CheckHandInPocket()
    {
        //Debug.Log(headTransform.position.z - transform.position.z);
        float xDiff = headTransform.position.x - transform.position.x;
        float zDiff = headTransform.position.z - transform.position.z;
        //FOR SITTING DOWN
        return ((headTransform.position.y - transform.position.y) > 0.4) && (xDiff > -0.2)  && (xDiff < 0.2) && (zDiff > -0.2) && (zDiff < 0.4);
    }
    
}
