using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_GameScene : UI_Scene
{
    public UI_Inventory InvenUI {get => _inven; private set => _inven = value; }
    public UI_Stat StatUI {get => _stat; private set => _stat = value; }
    public UI_Minimap MiniMap { get => _manimap; private set => _manimap = value; }
    public UI_ServiceContent ServiceContent { get => _service; private set => _service = value; }
    public UI_Chat Chat { get => _chat; private set => _chat = value; }
    public UI_HUD HUD { get => _hud; private set => _hud = value; }
    public UI_UserInfo UserInfo { get => _userInfo; private set => _userInfo = value; }
    public UI_QuickSlot QuickSlot { get => _quickSlot; private set => _quickSlot = value; }

    [SerializeField]private UI_Inventory _inven;
    [SerializeField]private UI_Stat _stat;
    [SerializeField] private UI_Minimap _manimap;
    [SerializeField] private UI_ServiceContent _service;
    [SerializeField] private UI_Chat _chat;
    [SerializeField] private UI_HUD _hud;
    [SerializeField] private UI_UserInfo _userInfo;

    [SerializeField] private UI_QuickSlot _quickSlot;

    public override void Init()
    {
        base.Init();

        InvenUI = GetComponentInChildren<UI_Inventory>();
        InvenUI.gameObject.SetActive(false);

        StatUI= GetComponentInChildren<UI_Stat>();
        StatUI.gameObject.SetActive(false);

        MiniMap = Managers.UI.ShowSceneUI<UI_Minimap>();
        ServiceContent = Managers.UI.ShowSceneUI<UI_ServiceContent>();
        Chat = Managers.UI.ShowSceneUI<UI_Chat>();
        HUD = Managers.UI.ShowSceneUI<UI_HUD>();
        UserInfo = Managers.UI.ShowSceneUI<UI_UserInfo>();
        QuickSlot = Managers.UI.ShowSceneUI<UI_QuickSlot>();

    }
}
