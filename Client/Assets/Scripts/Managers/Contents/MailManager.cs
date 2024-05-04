using System;
using System.Collections;
using System.Collections.Generic;
using Google.Protobuf.Protocol;
using UnityEngine;

public class MailManager 
{
    public List<MailItem> MailItems{ get; private set;} = new List<MailItem>();
    private Action _callback;


    public void SetCallBack(Action ac)
    {
        _callback = null;
        _callback = ac;
    }

    public void SetMailItemList(List<MailItem> newMails)
    {
        MailItems.Clear();

        if(newMails != null)
        {
            foreach (MailItem mailItem in newMails)
            {
                MailItem mail = new MailItem();
                mail.MergeFrom(mailItem);
                MailItems.Add(mail);
            }
        }
        _callback?.Invoke();
    }
}
