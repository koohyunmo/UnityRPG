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

    [SerializeField]private UI_Inventory _inven;
    [SerializeField]private UI_Stat _stat;
    [SerializeField] private UI_Minimap _manimap;
    [SerializeField] private UI_ServiceContent _service;
    [SerializeField] private UI_Chat _chat;

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

    }
}
