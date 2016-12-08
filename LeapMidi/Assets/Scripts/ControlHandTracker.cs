using UnityEngine;
using Leap;

public class ControlHandTracker : MonoBehaviour
{
    private static float yRef;
    private static float rollRef;
    private static bool track = false;
    private static bool rightHand;
    private static bool handClosed;
    private static Controller controller;
    private static float positionThreshold = 150f;
    private static float rotationThreshold = 0.7f;


    // Use this for initialiyation
    void Start()
    {
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
                        Debug.Log("DeltaRoll :" + DeltaRoll);
                        rollRef = roll;
                    }

                    MidiController.setValues(DeltaPos, DeltaRoll);                    
                }
            }
        }
    }

    public static void setControllingHandIsRight(bool isRight)
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

    public static void setStaticHandIsClosed(bool closed)
    {
        handClosed = closed;
    }

    public static void stopTracking()
    {
        track = false;
    }
}