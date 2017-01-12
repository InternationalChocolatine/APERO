using UnityEngine;
using Leap;
using Leap.Unity;
using System.Collections.Generic;

public class EasterEggs : MonoBehaviour {
    public GameObject dollarBillPrefab;
    private bool handsJoined = false;
    private Controller controller;
    private const float PROXIMITY_THRESHOLD = 90;

    // Use this for initialization
    void Start () {
        controller = new Controller();
	}
	
	// Update is called once per frame
	void Update () {
        List<Hand> hands = controller.Frame().Hands;

        if (hands.Count == 2)
        {
            bool palmFacingOpposite = Mathf.Sign(Vector3.Cross(hands[0].PalmNormal.ToVector3(), Vector3.forward).x) !=
                    Mathf.Sign(Vector3.Cross(hands[1].PalmNormal.ToVector3(), Vector3.forward).x);

            if (hands[0].IsLeft != hands[1].IsLeft && palmFacingOpposite)
            {
                if (!handsJoined && hands[0].PalmPosition.DistanceTo(hands[1].PalmPosition) < PROXIMITY_THRESHOLD)
                {
                    handsJoined = true;
                    Debug.Log("ready");
                }
                else if (handsJoined && hands[0].PalmPosition.DistanceTo(hands[1].PalmPosition) > PROXIMITY_THRESHOLD)
                {
                    handsJoined = false;
                    spawnDollarBill();
                    Debug.Log("go");
                }
            }
            else
            {
                handsJoined = false;
            }
        }
        else
        {
            handsJoined = false;
        }
	}

    private void spawnDollarBill()
    {
        GameObject dollarBill = Instantiate(dollarBillPrefab);
    }
}
