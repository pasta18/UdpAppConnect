using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Net;
using System;

public class TestUdp : MonoBehaviour
{
    UdpDataClient udp;

    [SerializeField]
    Button send;
    [SerializeField]
    Button receive;
    [SerializeField]
    Text text;
    [SerializeField]
    Button textSend;

    [SerializeField]
    Text[] Address;

    [SerializeField]
    Button AddressButton;
    [SerializeField]
    Text AddressButtonText;

    private bool AddressFlag = false;
    private string ipAddress;

    public void Start()
    {
        AddressButton.interactable = false;
    }

    public void Update()
    {
        if (Address.Length == 4)
        {
            try
            {
                foreach (var address in Address)
                {
                    if (address.text.Length <= 0 || int.Parse(address.text) > 255 || int.Parse(address.text) < 0)
                    {
                        AddressButton.interactable = false;
                        return;
                    }
                }
            }
            catch(FormatException e)
            {
                Debug.Log("記号はNG");
                return;
            }
            AddressButton.interactable = true;
        }
    }

    public void SendButtonSet()
    {
        if (AddressFlag)
        {
            Debug.Log(ipAddress);
            udp = new UdpDataClient(false, ipAddress);
        }
        else udp = new UdpDataClient(false);
        send.interactable = false;
        receive.interactable = false;
        textSend.interactable = true;
    }

    public void ReceiveButtonSet()
    {
        udp = new UdpDataClient(true);
        send.interactable = false;
        receive.interactable = false;
        udp.AddReceiveEvent(OnReceive);
    }

    public void AddressSetButton()
    {
        AddressFlag = !AddressFlag;

        if (AddressFlag)
        {
            AddressButtonText.text = "Cancel";
            SetAddress();
        }
        else
        {
            AddressButtonText.text = "Set";
        }

        foreach(var address in Address)
        {
            var inputField = address.GetComponent<InputField>();

            if(inputField != null)
            {
                address.GetComponent<InputField>().interactable = !inputField.interactable;
            }
        }
    }

    public void TextSendButtonClick()
    {
        udp.Send(text.text);
    }

    public void OnApplicationQuit()
    {
        if (udp != null) udp.EndSocket();
    }

    public void OnReceive(string data)
    {
        Debug.Log(data);
    }

    public void SetAddress()
    {
        ipAddress = "";
        ipAddress += Address[0].text;

        for(int i = 1; i < Address.Length;i++)
        {
            ipAddress += "." + Address[i].text;
        }
    }
}
