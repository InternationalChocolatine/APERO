using UnityEngine;
using TobiasErichsen.teVirtualMIDI;
using System;
using System.Threading;
using System.Runtime.InteropServices;
using Sanford.Multimedia.Midi;
using UnityEngine.UI;

public class MidiController : MonoBehaviour
{
    private TeVirtualMIDI midiVirtualDevice;
    public string virtualPortName;
    private Thread midiThread;
    private static OutputDevice outputDevice;
    private static ChannelSwitch activeSwitch;
    private bool handIsRight = true;
    private static int offset;
    public static bool palmFacingUp;
    public Text text0;
    private static Text st_text0;

    //Rotation
    private static float totalRotation = 0;
    private static byte rotationValue = 0;
    const float minAngle = -(Mathf.PI / 4);
    const float maxAngle = -minAngle;

    //Position
    private static float totalPosition = 0;
    private static byte positionValue = 0;
    const float minPosition = -150.0f;
    const float maxPosition = -minPosition;


    // Use this for initialization
    void Start()
    {
        midiVirtualDevice = startMidiPort();
        midiThread = new Thread(new ThreadStart(SendReceiveMIDI));
        midiThread.Start();
        outputDevice = new OutputDevice(getMidiPortID());
        st_text0 = text0;
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

    public static void setChannelGroup(ChannelSwitch activated)
    {
        if(activeSwitch != null)
        {
            activeSwitch.Deactivate();
        }
        activeSwitch = activated;
    }

    public static int getChannelGroup()
    {
        if(activeSwitch == null)
        {
            return -1;
        }
        return activeSwitch.channelGroup;
    }

    public static void setPalmFacingUp(bool palmFacing)
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

    public static void setValues(float height, float roll)
    {
        if(activeSwitch != null)
        {
            updatePosition(height);
            updateOrientation(roll);

        }

    }

    private static void updatePosition(float position)
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
            st_text0.text = positionValue.ToString();
            outputDevice.Send(message);
        }
    }

    private static void updateOrientation(float angle)
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

}
