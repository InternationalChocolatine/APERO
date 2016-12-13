using UnityEngine;
using Leap;

public class ControlHandTracker : MonoBehaviour
{
    private static ControlHandTracker instance;
    private float yRef;
    private float rollRef;
    private bool track = false;
    private bool rightHand;
    private bool handClosed;
    private Controller controller;

    private float positionThreshold = 150f;
    private float rotationThreshold = 0.7f;

    public bool pinched = false;
    public const float GRAB_ON_THRESHOLD = 0.8f;
    public const float GRAB_OFF_THRESHOLD = 0.6f;

    public static ControlHandTracker getInstance()
    {
        return instance;
    }

    // Use this for initialiyation
    void Start()
    {
        instance = this;
        controller = new Controller();
    }

    // Update is called once per frame
    void Update()
    {
        if (track)
        {
            foreach (Hand hand in controller.Frame().Hands)
            {
                if (hand.IsRight == rightHand)
                {
                    float yPos = hand.PalmPosition.y;
                    float roll = -hand.PalmNormal.Roll;
                    float DeltaPos = yPos - yRef;
                    float DeltaRoll = roll - rollRef;
                    
                    //Handling setup mode
                    //Position
                    if(handClosed && Mathf.Abs(DeltaPos) < positionThreshold)
                    {
                        DeltaPos = 0.0f;
                    }
                    else
                    {
                        yRef = yPos;
                    }

                    //Rotation
                    if(handClosed && Mathf.Abs(DeltaRoll) < rotationThreshold)
                    {
                        DeltaRoll = 0.0f;
                    }
                    else
                    {
                        rollRef = roll;
                    }

                    MidiController.getInstance().setValues(DeltaPos, DeltaRoll);

                    if (!pinched && hand.GrabStrength > GRAB_ON_THRESHOLD)
                    {
                        Debug.Log("Pinched !");
                        pinched = true;
                        MidiController.getInstance().setPinched();
                    }
                    else if (pinched && hand.GrabStrength <= GRAB_OFF_THRESHOLD)
                    {
                        Debug.Log("Unpiched !");
                        pinched = false;
                        MidiController.getInstance().setUnpinched();
                    }
                }
            }
        }
    }

    public void setControllingHandIsRight(bool isRight)
    {
        rightHand = isRight;
        track = true;
        foreach (Hand hand in controller.Frame().Hands)
        {
            if (hand.IsRight == rightHand)
            {
                float yPos = hand.PalmPosition.y;
                float roll = -hand.PalmNormal.Roll;

                yRef = yPos;
                rollRef = roll;
            }
        }
    }

    public void setStaticHandIsClosed(bool closed)
    {
        handClosed = closed;
    }

    public void stopTracking()
    {
        track = false;
    }
}