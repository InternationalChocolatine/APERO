using UnityEngine;
using Sanford.Multimedia.Midi;

public class ButtonTrigger : MonoBehaviour {

    private Collider initialObject;
    private int id;
    private bool oneIn;
    public float force;
    public byte midiID;

    public static OutputDevice outputDevice;

    // Use this for initialization
    void Start () {
        oneIn = false;

        id = MidiScript.getMidiPortID();
        if (outputDevice == null)
        {
            outputDevice = new OutputDevice(id);
        }
    }
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnTriggerEnter(Collider other)
    {
        if (!oneIn)
        {
            ChannelMessage message = new ChannelMessage(ChannelCommand.NoteOn, 0, midiID, 127);
            outputDevice.Send(message);
            initialObject = other;
            oneIn = true;
        }
    }


    void OnTriggerStay(Collider other)
    {
        if(initialObject && other == initialObject)
        {
            initialObject.GetComponent<Rigidbody>().AddForce(new Vector3(0, 0, -force), ForceMode.Acceleration);
            initialObject.GetComponent<MeshRenderer>().material.color = new Color(0, 0, 1);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other == initialObject)
        {
            initialObject.attachedRigidbody.velocity = new Vector3(0, 0, 0);
            initialObject.GetComponent<MeshRenderer>().material.color = new Color(1, 1, 1);
            oneIn = false;
            initialObject = null;
            ChannelMessage message = new ChannelMessage(ChannelCommand.NoteOff, 0, midiID, 127);
            outputDevice.Send(message);
        }
    }

    void OnApplicationQuit()
    {
        outputDevice.Dispose();
    }
}
