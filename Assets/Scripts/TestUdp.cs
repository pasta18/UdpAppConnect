using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Net;
using System;
using DualLeap;

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
    private bool SendFlag = false;
    private string ipAddress;

    public void Start()
    {
        AddressButton.interactable = false;
        for(int i = 0;i < Address.Length; i++)
        {
            if (PlayerPrefs.HasKey("Address" + i))
            {
                Address[i].GetComponent<InputField>().text = PlayerPrefs.GetString("Address" + i);
                Debug.Log("set " + PlayerPrefs.GetString("Address" + i));
            }
        }
    }

    public void Update()
    {
        if (Address.Length == 4 && !SendFlag)
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
            udp = new UdpDataClient(false, ipAddress, LeapDisassembly.MaxSize);
        }
        else udp = new UdpDataClient(false, null, LeapDisassembly.MaxSize);
        send.interactable = false;
        receive.interactable = false;
        textSend.interactable = true;
        AddressButton.interactable = false;
        SendFlag = true;
    }

    public void ReceiveButtonSet()
    {
        udp = new UdpDataClient(true, null, LeapDisassembly.MaxSize);
        send.interactable = false;
        receive.interactable = false;
        udp.AddReceiveEvent(OnReceive);
        AddressButton.interactable = false;
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

        PlayerPrefs.SetString("Address0", Address[0].text);

        for(int i = 1; i < Address.Length;i++)
        {
            ipAddress += "." + Address[i].text;
            PlayerPrefs.SetString("Address" + i, Address[i].text);
        }
    }

    public void Send(byte[] bytes)
    {
        if (!SendFlag) return;

        udp.Send(bytes);
    }
}
