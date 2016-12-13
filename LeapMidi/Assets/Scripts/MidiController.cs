using UnityEngine;
using TobiasErichsen.teVirtualMIDI;
using System;
using System.Threading;
using System.Runtime.InteropServices;
using Sanford.Multimedia.Midi;
using UnityEngine.UI;

public class MidiController : MonoBehaviour
{
    private static MidiController instance;
    private TeVirtualMIDI midiVirtualDevice;
    public string virtualPortName;
    private Thread midiThread;
    private static OutputDevice outputDevice;
    private ChannelSwitch activeSwitch;
    private bool handIsRight = true;
    private int offset;
    public bool palmFacingUp;
    public Text text0;

    //Rotation
    private float totalRotation = 0;
    private byte rotationValue = 0;
    const float minAngle = -(Mathf.PI / 4);
    const float maxAngle = -minAngle;

    //Position
    private float totalPosition = 0;
    private byte positionValue = 0;
    const float minPosition = -150.0f;
    const float maxPosition = -minPosition;

    //Channel values
    private float[] channelTotalValues = new float[128];


    public static MidiController getInstance()
    {
        return instance;
    }

    // Use this for initialization
    void Start()
    {
        instance = this;
        midiVirtualDevice = startMidiPort();
        midiThread = new Thread(new ThreadStart(SendReceiveMIDI));
        midiThread.Start();
        outputDevice = new OutputDevice(getMidiPortID());
    }

    // Update is called once per frame
    void Update()
    {
    }

    void OnApplicationQuit()
    {
        midiThread.Abort();
        midiVirtualDevice.shutdown();
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

        TeVirtualMIDI port = new TeVirtualMIDI(virtualPortName, 65535, TeVirtualMIDI.TE_VM_FLAGS_PARSE_RX, ref manufacturer, ref product);
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
                command = midiVirtualDevice.getCommand();
                midiVirtualDevice.sendCommand(command);
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

    public void setChannelGroup(ChannelSwitch activated)
    {
        if (activeSwitch != null)
        {
            activeSwitch.Deactivate();
            channelTotalValues[activeSwitch.channelGroup + offset] = totalPosition;
            channelTotalValues[activeSwitch.channelGroup + offset + 1] = totalRotation;
        }

            activeSwitch = activated;

        totalPosition = channelTotalValues[activeSwitch.channelGroup + offset];
        totalRotation = channelTotalValues[activeSwitch.channelGroup + offset + 1];
    }

    public int getChannelGroup()
    {
        if(activeSwitch == null)
        {
            return -1;
        }
        return activeSwitch.channelGroup;
    }

    public void setPalmFacingUp(bool palmFacing)
    {
        palmFacingUp = palmFacing;
        if (palmFacingUp)
        {
            offset = 3;
        }
        else
        {
            offset = 0;
        }
    }

    public void setValues(float height, float roll)
    {
        if (activeSwitch != null)
        {
            updatePosition(height);
            updateOrientation(roll);
        }
    }

    private void updatePosition(float position)
    {
        if(position != 0)
        {
            if (totalPosition + position < minPosition)
            {
                totalPosition = minPosition;
            }
            else if (totalPosition + position > maxPosition)
            {
                totalPosition = maxPosition;
            }
            else
            {
                totalPosition += position;
            }

            positionValue = (byte)((totalPosition + maxPosition) / (2 * maxPosition) * 127);
            ChannelMessage message = new ChannelMessage(ChannelCommand.Controller, 0, activeSwitch.channelGroup + offset, positionValue);
            text0.text = positionValue.ToString();
            outputDevice.Send(message);
        }
    }

    private void updateOrientation(float angle)
    {
        if(angle != 0)
        {
            // Bounded rotation
            if (totalRotation + angle < minAngle)
            {
                totalRotation = minAngle;
            }
            else if (totalRotation + angle > maxAngle)
            {
                totalRotation = maxAngle;
            }
            else
            {
                totalRotation += angle;
            }
            rotationValue = (byte)((totalRotation + maxAngle) / (2 * maxAngle) * 127); // okay as long interval is symetric

            ChannelMessage message = new ChannelMessage(ChannelCommand.Controller, 0, activeSwitch.channelGroup + offset + 1, rotationValue);
            outputDevice.Send(message);
        }  
    }

    public void setPinched()
    {
        ChannelMessage message = new ChannelMessage(ChannelCommand.NoteOn, 0, activeSwitch.channelGroup + offset + 2, 127);
        outputDevice.Send(message);
    }

    public void setUnpinched()
    {
        ChannelMessage message = new ChannelMessage(ChannelCommand.NoteOff, 0, activeSwitch.channelGroup + offset + 2, 127);
        outputDevice.Send(message);
    }
}
