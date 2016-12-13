using UnityEngine;
using System.Collections;

public class ChannelSwitch : MonoBehaviour
{
    public int channelGroup;

    // Use this for initialization
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnTriggerEnter()
    {
        if (MidiController.getInstance().getChannelGroup() != channelGroup)
        {
            GetComponent<MeshRenderer>().material.color = Color.green;
            MidiController.getInstance().setChannelGroup(this);

        }
    }

    public void Deactivate()
    {
        GetComponent<MeshRenderer>().material.color = Color.grey;
    }
}