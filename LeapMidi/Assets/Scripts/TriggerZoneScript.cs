using UnityEngine;
using System.Collections;
using Leap.Unity;
using Leap;

public class TriggerZoneScript : MonoBehaviour
{
    private bool facingUp;
    private HandModel handModel;
    bool handIsSet;

    // Use this for initialization
    void Start()
    {
        handIsSet = false;
        facingUp = true;
        MidiController.setPalmFacingUp(facingUp);
    }

    // Update is called once per frame
    void Update()
    {
       if(handModel != null)
        {
            if (this.GetComponent<Collider>().bounds.Contains(handModel.GetPalmPosition()))
            {
                if (!handIsSet)
                {
                    ControlHandTracker.setControllingHandIsRight(!handModel.GetLeapHand().IsRight);
                    handIsSet = true;
                }
                bool res = Vector3.Cross(handModel.GetLeapHand().PalmNormal.ToVector3(), Vector3.forward).x > 0;
                if (res != facingUp)
                {
                    Debug.Log("palm facing Up : " + res);
                    facingUp = res;
                    MidiController.setPalmFacingUp(facingUp);
                }
            }
            else if (handIsSet)
            {
                handIsSet = false;
                ControlHandTracker.stopTracking();
                foreach (MeshRenderer child in gameObject.GetComponentsInChildren<MeshRenderer>())
                {
                    child.material.color = Color.white;
                }
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
         handModel = GetHandModel(other); 

        if (handModel != null)
        {
            foreach (MeshRenderer child in gameObject.GetComponentsInChildren<MeshRenderer>())
            {
                child.material.color = Color.cyan;
            }
        }
    }


    private HandModel GetHandModel(Collider other)
    {
        if (other.transform.parent && other.transform.parent.parent.parent &&
            other.transform.parent.parent.GetComponent<HandModel>())
            return other.transform.parent.parent.GetComponent<HandModel>();
        else
            return null;
    }
}