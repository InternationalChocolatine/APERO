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

    private int counter = 60;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnTriggerEnter(Collider other)
    {
        int id = 1;
        OutputDevice outputDevice = new OutputDevice(id);
        ChannelMessage message = new ChannelMessage(ChannelCommand.NoteOn, 0, counter, 127);
        outputDevice.Send(message);
        Debug.Log("Message sent !");

        Thread.Sleep(200);
        message = new ChannelMessage(ChannelCommand.NoteOff, 0, counter, 127);
        outputDevice.Send(message);
        Debug.Log("Message stop sent !");
        counter++;
        outputDevice.Dispose();

    }
    }
