using System.Collections;
using System.Collections.Generic;
using Google.Protobuf.Protocol;
using UnityEngine;

public class UI_MailPopup : UI_ServicePopup
{
    public override void Init()
    {
        base.Init();
        C_MailItemList c_MailItemList= new C_MailItemList();
        Managers.Network.Send(c_MailItemList);

        ContentItemClear();
        Managers.Mail.SetCallBack(MakeItemList);
    }

    public void MakeItemList()
    {
        ContentItemClear();
        if (Managers.Mail.MailItems.Count > 0)
        {
            foreach (var mail in Managers.Mail.MailItems)
            {
                GameObject go = Managers.Resource.Instantiate("UI/Popup/UI_MailItem", sharedContentGrid);
                var mailItem = go.GetOrAddComponent<UI_MailItem>();
                mailItem.SetItem(mail);
            }
        }
        else
        {
            Debug.Log($"아이템 리스트를 갖고올수없음");
        }
    }

    public override void ContentItemClear()
    {
        base.ContentItemClear();
    }
}
