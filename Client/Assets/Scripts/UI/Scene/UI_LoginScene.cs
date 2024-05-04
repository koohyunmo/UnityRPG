using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_LoginScene : UI_Scene
{
    enum GameObjects
    {
        AccountName,
        Password
    }

    enum Images
    {
        CreateBtn,
        LoginBtn
    }
    public override void Init()
    {
        base.Init();

        Bind<GameObject>(typeof(GameObjects));
        Bind<Image>(typeof(Images));

        GetImage((int)Images.CreateBtn).gameObject.BindEvent(OnClickCreateButton);
        GetImage((int)Images.LoginBtn).gameObject.BindEvent(OnClickLoginButton);
    }

    public void OnClickCreateButton(PointerEventData data)
    {
        Debug.Log("OnClickCreateButton");
        string account = Get<GameObject>((int)GameObjects.AccountName).GetComponent<InputField>().text;
        string Password = Get<GameObject>((int)GameObjects.Password).GetComponent<InputField>().text;

        CreateAccountPacketReq packet = new CreateAccountPacketReq()
        {
            AccountName = account,
            Password = Password
        };

        Managers.Web.SendPostRequest<CreateAccountPacketRes>("account/create", packet, (res) =>
        {
            Debug.Log($"Create Account : {account} | {res.CreateOk}");
            Get<GameObject>((int)GameObjects.AccountName).GetComponent<InputField>().text = "";
            Get<GameObject>((int)GameObjects.Password).GetComponent<InputField>().text = "";
        });
    }
    public void OnClickLoginButton(PointerEventData data)
    {
        Debug.Log("OnClickLoginButton");

        string account = Get<GameObject>((int)GameObjects.AccountName).GetComponent<InputField>().text;
        string Password = Get<GameObject>((int)GameObjects.Password).GetComponent<InputField>().text;

        LoginAccountPacketReq packet = new LoginAccountPacketReq()
        {
            AccountName = account,
            Password = Password
        };

        Debug.Log($"{account} login...");



        Managers.Web.SendPostRequest<LoginAccountPacketRes>("account/login", packet, (res) =>
        {
            
            if (res.LoginOk)
            {
                Managers.Network.AccountId = res.AccountId;
                Managers.Network.Token = res.Token;
                
                Debug.Log($"Login Account : {account} | {res.LoginOk} |  {res.AccountId}: {res.Token}");
                UI_SelectServerPopup popup = Managers.UI.ShowPopupUI<UI_SelectServerPopup>();
                popup.SetServer(res.ServerList);
            }
            else
            {
                Get<GameObject>((int)GameObjects.AccountName).GetComponent<InputField>().text = "";
                Get<GameObject>((int)GameObjects.Password).GetComponent<InputField>().text = "";
                Debug.Log($"Login Account : {account} | {res.LoginOk}");
            }


        });


    }
}
