using UnityEngine;
using System.Collections;
using TobiasErichsen.teVirtualMIDI;
using System;
using System.Threading;
using System.Runtime.InteropServices;
using Sanford.Multimedia.Midi;

public class MidiScript : MonoBehaviour
{

    public TeVirtualMIDI midiPort;
    public string portName;
    Thread midiThread;
    public static OutputDevice outputDevice;


    // Use this for initialization
    void Start()
    {
        midiPort = startMidiPort();
        midiThread = new Thread(new ThreadStart(SendReceiveMIDI));
        midiThread.Start();
        outputDevice = new OutputDevice(getMidiPortID());
    }

    // Update is called once per frame
    void Update()
    {
        //SendReceiveMIDI();
    }

    void OnApplicationQuit()
    {
        midiThread.Abort();
        midiPort.shutdown();
        Debug.Log("Application ending after " + Time.time + " seconds");
    }

    private TeVirtualMIDI startMidiPort()
    {
        Debug.Log("Up and running");
        //LaunchMidi();
        Console.WriteLine("teVirtualMIDI C# loopback sample");
        Console.WriteLine("using dll-version:    " + TeVirtualMIDI.versionString);
        Console.WriteLine("using driver-version: " + TeVirtualMIDI.driverVersionString);

        TeVirtualMIDI.logging(TeVirtualMIDI.TE_VM_LOGGING_MISC | TeVirtualMIDI.TE_VM_LOGGING_RX | TeVirtualMIDI.TE_VM_LOGGING_TX);

        Guid manufacturer = new Guid("aa4e075f-3504-4aab-9b06-9a4104a91cf0");
        Guid product = new Guid("bb4e075f-3504-4aab-9b06-9a4104a91cf0");

        TeVirtualMIDI port = new TeVirtualMIDI(portName, 65535, TeVirtualMIDI.TE_VM_FLAGS_PARSE_RX, ref manufacturer, ref product);
        Debug.Log("Midi Port started !");
        return port;
    }


    public void SendReceiveMIDI()
    {
        byte[] command;
        try
        {
            while (true)
            {
                command = midiPort.getCommand();
                midiPort.sendCommand(command);
                //Debug.Log("command: " + byteArrayToString(command));
            }
        }
        catch (Exception ex)
        {
            Debug.Log("Fail while sending / retrieving message: " + ex.Message);
        }
    }

    public static string byteArrayToString(byte[] ba)
    {
        string hex = BitConverter.ToString(ba);
        return hex.Replace("-", ":");

    }

    [DllImport("winmm.dll")]
    protected static extern int midiOutGetNumDevs();

    public static int getMidiPortID()
    {
        return midiOutGetNumDevs() - 1;
    }

}
