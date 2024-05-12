using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NotiManager
{
    UI_HUD _hud;
    UI_UserInfo _userInfo;

    public void ChangeUserInfo()
    {
        ChangeHp();
        ChangeExp();
        ChangeLevel();
    }
    
    public void GoldChange(int amount)
    {
        if(_hud == null)
        {
            UI_GameScene gameScene = Managers.UI.SceneUI as UI_GameScene;
            if (gameScene)
            {
                _hud = gameScene.HUD;

                if (_hud != null)
                {
                    _hud.UpdateGold(amount);
                }
            }

        }
        else
        {
            _hud.UpdateGold(amount);
        }

    }

    public void ChangeHp()
    {
        if (_userInfo == null)
        {
            UI_GameScene gameScene = Managers.UI.SceneUI as UI_GameScene;
            if (gameScene)
            {
                _userInfo = gameScene.UserInfo;

                if (_userInfo != null)
                {
                    //TODO
                    _userInfo.ChangeHp();
                }
            }

        }
        else
        {
            _userInfo.ChangeHp();
            //TODO
        }

    }

    public void ChangeExp()
    {
        if (_userInfo == null)
        {
            UI_GameScene gameScene = Managers.UI.SceneUI as UI_GameScene;
            if (gameScene)
            {
                _userInfo = gameScene.UserInfo;

                if (_userInfo != null)
                {
                    //TODO
                    _userInfo.ChangeExp();
                }
            }

        }
        else
        {
            _userInfo.ChangeExp();
            //TODO
        }

    }

    public void ChangeLevel()
    {
        if (_userInfo == null)
        {
            UI_GameScene gameScene = Managers.UI.SceneUI as UI_GameScene;
            if (gameScene)
            {
                _userInfo = gameScene.UserInfo;

                if (_userInfo != null)
                {
                    //TODO
                    _userInfo.ChangeLevel();
                }
            }

        }
        else
        {
            _userInfo.ChangeLevel();
            //TODO
        }

    }

}
