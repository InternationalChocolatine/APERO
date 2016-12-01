using UnityEngine;
using Leap;
using Sanford.Multimedia.Midi;

public class PotentiometerBehaviour : MonoBehaviour {
    private bool isRightHand;
    private bool grabbed;
    private Controller controller;
    private float refAngle;
    private float totalRotation = 0;
    private byte value = 0;
    const float minAngle = -(Mathf.PI * 5/6);
    const float maxAngle = - minAngle;
    OutputDevice outputDevice;
    public byte midiID;


    // Use this for initialization
    void Start () {
        controller = new Controller();
        grabbed = false;
        outputDevice = MidiScript.outputDevice;
        Debug.Log("min is " + minAngle);
        Debug.Log("max is " + maxAngle);
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
                    float roll = handList[i].Direction.Roll;
                    currentAngle = -roll + (roll / Mathf.Abs(roll)) * Mathf.PI;
                    updateOrientation(2*(currentAngle - refAngle));
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
                float roll = handList[i].Direction.Roll;
                refAngle = -roll + (roll / Mathf.Abs(roll)) * Mathf.PI;
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

    private void updateOrientation(float angle)
    {
        // Bounded rotation
        if(totalRotation + angle < minAngle)
        {
            transform.Rotate(Vector3.up, 180/Mathf.PI * (minAngle - totalRotation));
            totalRotation = minAngle;
        }
        else if (totalRotation + angle > maxAngle)
        {
            transform.Rotate(Vector3.up, 180/Mathf.PI * (maxAngle - totalRotation));
            totalRotation = maxAngle;
        }
        else
        {
            transform.Rotate(Vector3.up, 180 / Mathf.PI * (angle));
            totalRotation += angle;
        }
        value = (byte)((totalRotation + maxAngle)/(2*maxAngle) * 127); // okay as long interval is symetric
        Debug.Log("value is " + value);
        ChannelMessage message = new ChannelMessage(ChannelCommand.Controller, 0, midiID, value);
        outputDevice.Send(message);
    }
}
