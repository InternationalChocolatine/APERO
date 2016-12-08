using UnityEngine;
using Leap;

public class ControlHandTracker : MonoBehaviour
{
    private static float yRef;
    private static float rollRef;
    private static bool track = false;
    private static bool rightHand;
    private static Controller controller;
    public static bool pinched = false;
    public const int PINCH_DISTANCE_ON_THRESHOLD = 15;
    public const int PINCH_DISTANCE_OFF_THRESHOLD = 20;

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
                    //float currentAngle = -roll + (roll / Mathf.Abs(roll)) * Mathf.PI;
                    MidiController.setValues(yPos - yRef, (roll - rollRef));
                    //Debug.Log("height difference is " + (yPos - yRef));
                    //Debug.Log("roll difference is " + (roll - rollRef));
                    yRef = yPos;
                    rollRef = roll;
                }

                if (!pinched && hand.PinchDistance < PINCH_DISTANCE_ON_THRESHOLD)
                {
                    pinched = true;
                    MidiController.setPinched();
                } 
                else if (pinched && hand.PinchDistance >= PINCH_DISTANCE_OFF_THRESHOLD)
                {
                    pinched = false;
                    MidiController.setUnpinched();
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
                //float currentAngle = -roll + (roll / Mathf.Abs(roll)) * Mathf.PI;

                yRef = yPos;
                rollRef = roll;
            }
        }
    }

    public static void stopTracking()
    {
        track = false;
    }
}