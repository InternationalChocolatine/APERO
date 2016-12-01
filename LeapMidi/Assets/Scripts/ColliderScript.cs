using UnityEngine;
using System.Collections;
using Sanford.Multimedia.Midi;
using System.Threading;
using System;

/*
 * Small collider script sending midi message on virtual midi port
 * port is set in id variable
 * port = 0 is the basic microsoft synth, it can be usefull for testing purposes
 * WARNING : to use this script in unity, you have to use the 3 dll provided in the lib folder
 * Theses dll have to be placed in the asset folder of the unity project
 * */

public class ColliderScript : MonoBehaviour {
    int id = 1;
    private static int counter = 60;
    private int note;
    int collisionNumber = 0;
    public static OutputDevice outputDevice;
	// Use this for initialization
	void Start () {
        note = counter;
        counter += 1;        Debug.Log("Device started ! : " + note);
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
        if(collisionNumber == 0)
        {
            ChannelMessage message = new ChannelMessage(ChannelCommand.NoteOn, 0, note, 127);
        
            outputDevice.Send(message);
        }
        collisionNumber++;
    }

    void OnTriggerExit(Collider other)
    {
        collisionNumber--;
        if(collisionNumber == 0)
        {
            ChannelMessage message = new ChannelMessage(ChannelCommand.NoteOff, 0, note, 127);
            outputDevice.Send(message);
        }
        
    }

    void OnApplicationQuit(){
        outputDevice.Dispose();
    }
}
