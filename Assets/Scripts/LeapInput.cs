using UnityEngine;
using System.Collections;
using Leap;
using Leap.Unity;
using DualLeap;
using System;
using System.Text;

public class LeapInput : MonoBehaviour
{
    [SerializeField]
    bool DebugMode = false;

    [SerializeField]
    TestUdp udp;

    Frame frame;

    [SerializeField]
    LeapServiceProvider provider;

    Controller controller;

    bool leapEnabled = true;

    public bool LeapEnabled
    {
        get { return leapEnabled; }
    }

	void Start()
    {
        controller = provider.GetLeapController();
        leapEnabled = controller.IsConnected;
	}
	
	void Update()
    {
        frame = controller.Frame();
        leapEnabled = controller.IsConnected;

        if (leapEnabled) udp.Send(GetLeapBtyes());
	}

    public byte[] GetLeapBtyes()
    {
        return LeapDisassembly.Disassembly(frame.Hands);
    }
}
