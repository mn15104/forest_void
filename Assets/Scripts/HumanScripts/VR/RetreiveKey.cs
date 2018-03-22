using System.Collections;
using UnityEngine;

public class RetreiveKey : OVRGrabber 
{

    public Transform headTransform;
    private GameObject human;
    private GameObject key;
    public bool inGeneratorZone = false;
  


    protected override void Awake()
    {
        base.Awake();
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
            if (inGeneratorZone && CheckHandInPocket() && human.GetComponent<Inventory>().inventorySize() > 0)
            {
                KeyAppear();
                OVRGrabbable grabbable = key.GetComponent<OVRGrabbable>() ?? key.GetComponentInParent<OVRGrabbable>();
                if (grabbable == null) return;

                // Add the grabbable
                int refCount = 0;
                m_grabCandidates.TryGetValue(grabbable, out refCount);
                m_grabCandidates[grabbable] = refCount + 1;
            }
           
            GrabBegin();
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
        key.SetActive(true);
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

    private IEnumerator AddKeyToInventory(OVRGrabbable m_grabbedObj)
    {
        
        yield return new WaitForSeconds(0.5f);
        if (m_grabbedObj != null)
        {

            m_grabbedObj.gameObject.SetActive(false);
            GrabbableRelease(Vector3.zero, Vector3.zero);
            human.GetComponent<Inventory>().addKeyToInventory(m_grabbedObj.gameObject);
            

        }
      
    }

    protected override void GrabBegin()
    {
  
        float closestMagSq = float.MaxValue;
        OVRGrabbable closestGrabbable = null;
        Collider closestGrabbableCollider = null;

        // Iterate grab candidates and find the closest grabbable candidate
        foreach (OVRGrabbable grabbable in m_grabCandidates.Keys)
        {
            bool canGrab = !(grabbable.isGrabbed && !grabbable.allowOffhandGrab);
            if (!canGrab)
            {
                continue;
            }

            for (int j = 0; j < grabbable.grabPoints.Length; ++j)
            {
                Collider grabbableCollider = grabbable.grabPoints[j];
                // Store the closest grabbable
                Vector3 closestPointOnBounds = grabbableCollider.ClosestPointOnBounds(m_gripTransform.position);
                float grabbableMagSq = (m_gripTransform.position - closestPointOnBounds).sqrMagnitude;
                if (grabbableMagSq < closestMagSq)
                {
                    closestMagSq = grabbableMagSq;
                    closestGrabbable = grabbable;
                    closestGrabbableCollider = grabbableCollider;
                }
            }
        }

        // Disable grab volumes to prevent overlaps
        GrabVolumeEnable(false);

        if (closestGrabbable != null)
        {
            //WAS COMMENTED OUT
            //if (closestGrabbable.isGrabbed)
            //{
            //    closestGrabbable.grabbedBy.OffhandGrabbed(closestGrabbable);
            //}

            m_grabbedObj = closestGrabbable;
            m_grabbedObj.GrabBegin(this, closestGrabbableCollider);

            //INSERTED
            if(!inGeneratorZone)
              StartCoroutine(AddKeyToInventory(m_grabbedObj));

            m_lastPos = transform.position;
            m_lastRot = transform.rotation;

            // Set up offsets for grabbed object desired position relative to hand.
            if (m_grabbedObj.snapPosition)
            {
                m_grabbedObjectPosOff = m_gripTransform.localPosition;
                if (m_grabbedObj.snapOffset)
                {
                    Vector3 snapOffset = m_grabbedObj.snapOffset.position;
                    if (m_controller == OVRInput.Controller.LTouch) snapOffset.x = -snapOffset.x;
                    m_grabbedObjectPosOff += snapOffset;
                }
            }
            else
            {
                Vector3 relPos = m_grabbedObj.transform.position - transform.position;
                relPos = Quaternion.Inverse(transform.rotation) * relPos;
                m_grabbedObjectPosOff = relPos;
            }

            if (m_grabbedObj.snapOrientation)
            {
                m_grabbedObjectRotOff = m_gripTransform.localRotation;
                if (m_grabbedObj.snapOffset)
                {
                    m_grabbedObjectRotOff = m_grabbedObj.snapOffset.rotation * m_grabbedObjectRotOff;
                }
            }
            else
            {
                Quaternion relOri = Quaternion.Inverse(transform.rotation) * m_grabbedObj.transform.rotation;
                m_grabbedObjectRotOff = relOri;
            }

            // Note: force teleport on grab, to avoid high-speed travel to dest which hits a lot of other objects at high
            // speed and sends them flying. The grabbed object may still teleport inside of other objects, but fixing that
            // is beyond the scope of this demo.
            MoveGrabbedObject(m_lastPos, m_lastRot, true);
            if (m_parentHeldObject)
            {
                m_grabbedObj.transform.parent = transform;
            }
        }
    }


}
