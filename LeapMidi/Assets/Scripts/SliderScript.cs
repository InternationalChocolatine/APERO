using UnityEngine;
using System.Collections;
using Sanford.Multimedia.Midi;

public class SliderScript : MonoBehaviour {
    private bool pinched;
    private GameObject hand;
    public GameObject slider;
    private Vector3 sliderSize;
    private Vector3 sliderPosition;
    private byte midiValue;
    public byte midiID;
    private OutputDevice outputDevice;
    private GameObject nullObject;

    // Use this for initialization
    void Start () {
        this.transform.parent = slider.transform;
        sliderSize = slider.GetComponent<Renderer>().bounds.size;
        nullObject = new GameObject();
        nullObject.transform.parent = slider.transform;

        sliderPosition = slider.transform.position;

        if (outputDevice == null)
        {
            outputDevice = MidiScript.outputDevice;
        }
    }
	
	// Update is called once per frame
	void Update () {
	    if(pinched && (hand != null)){
            nullObject.transform.position = hand.transform.position;
            Vector3 handY = nullObject.transform.localPosition;
            handY = new Vector3(0, handY.y, 0);
            if(handY.y > 0.5)
            {
                handY.y = 0.5f;
            } else if (handY.y < -0.5f)
            {
                handY.y = -0.5f;
            }
            this.transform.localPosition = handY;
            byte value = (byte)((handY.y + 0.5f) * 127);
            if(value != midiValue)
            {
                midiValue = value;
                ChannelMessage message = new ChannelMessage(ChannelCommand.Controller, 0, midiID, value);
                outputDevice.Send(message);
                Debug.Log("value : " + value);
            }
        }
	}

    public void changeColorIn()
    {
        this.gameObject.GetComponent<MeshRenderer>().material.color = new Color(0, 0, 1);
    }

    public void changeColorOut()
    {
        this.gameObject.GetComponent<MeshRenderer>().material.color = new Color(1, 0, 0);
    }

    public void changeColorPinch(GameObject pinchingHand)
    {
        pinched = true;
        this.hand = pinchingHand;
        this.gameObject.GetComponent<MeshRenderer>().material.color = new Color(0, 1, 0);        
    }

    public void changeColorUnPinch()
    {
        pinched = false;
        this.hand = null;
        this.gameObject.GetComponent<MeshRenderer>().material.color = new Color(0, 0, 1);
    }

    void OnApplicationQuit()
    {
        outputDevice.Dispose();
    }
}
