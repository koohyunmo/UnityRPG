using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
public class UI_ServicePopup : UI_Popup
{
    protected enum SharedImages
    {
        BG
    }
    protected enum SharedGameObjects
    {
        Content
    }
    protected enum SharedButtons
    {
        UI_CloseButton,
        ItemSearchButton,
        ItemRefreshButton
    }

    protected enum SharedInputs
    {
        InputField
    }

    protected Transform sharedContentGrid;
    protected InputField sharedInputField;

    public override void Init()
    {
        base.Init();

        Bind<Button>(typeof(SharedButtons));
        Bind<Image>(typeof(SharedImages));
        Bind<GameObject>(typeof(SharedGameObjects));
        Bind<InputField>(typeof(SharedInputs));

        sharedContentGrid = Get<GameObject>((int)SharedGameObjects.Content).transform;
        sharedInputField = Get<InputField>((int)SharedInputs.InputField);


        Get<Button>((int)SharedButtons.UI_CloseButton).gameObject.BindEvent((e) => { Managers.UI.ClosePopupUI(); });
        Get<Image>((int)SharedImages.BG).gameObject.BindEvent((e) => { Managers.UI.ClosePopupUI(); });
        Get<Button>((int)SharedButtons.ItemSearchButton).gameObject.BindEvent(OnClickSearch);

        ContentItemClear();

    }

    public virtual void OnClickSearch(PointerEventData data)
    {

    }

    public virtual void ContentItemClear()
    {
        if(sharedContentGrid == null)
        {
            sharedContentGrid = Get<GameObject>((int)SharedGameObjects.Content).transform;
        }

        foreach (Transform child in sharedContentGrid)
            Managers.Resource.Destroy(child.gameObject);
    }

}
