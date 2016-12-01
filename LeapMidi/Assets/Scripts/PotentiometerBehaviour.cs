using UnityEngine;
using Leap;

public class PotentiometerBehaviour : MonoBehaviour {
    private bool isRightHand;
    private bool grabbed;
    private Controller controller;
    private float refAngle;

	// Use this for initialization
	void Start () {
        controller = new Controller();
        grabbed = false;
	}
	
	// Update is called once per frame
	void Update () {
        if (grabbed == true)
        {
            float currentAngle;
            System.Collections.Generic.List<Hand> handList = controller.Frame().Hands;
            for(int i = 0; i < handList.Count; i++)
            {
                if(handList[i].IsLeft != isRightHand)
                {
                    currentAngle = -handList[i].Direction.Roll + (handList[i].Direction.Roll / Mathf.Abs(handList[i].Direction.Roll)) * Mathf.PI;
                    Debug.Log("Updated! new value is " + currentAngle);
                    transform.Rotate(Vector3.up, 20 * (currentAngle - refAngle));
                    refAngle = currentAngle;
                }
            }
        }
    }

    public void Pinch(bool rightHand)
    {
        grabbed = true;
        isRightHand = rightHand;

        System.Collections.Generic.List<Hand> handList = controller.Frame().Hands;
        for(int i = 0; i < handList.Count; i++)
        {
            if(handList[i].IsLeft != isRightHand)
            {
                refAngle = -handList[i].Direction.Roll + (handList[i].Direction.Roll / Mathf.Abs(handList[i].Direction.Roll)) * Mathf.PI;
                Debug.Log("Initial value is " + refAngle);
            }
        }

        MeshRenderer mr = GetComponent<MeshRenderer>();
        mr.material.color = Color.red;
    }

    public void Unpinch()
    {
        grabbed = false;
        MeshRenderer mr = GetComponent<MeshRenderer>();
        mr.material.color = Color.white;
    }
}
