using System;
using Game.Script.Attribute;
using Game.Script.Game;
using Game.Script.Level;
using Mirror;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Script.UI.Frames
{
    public class HallFrame : Frame
    {
        protected override string ResPath => "Assets/Game/Res/UI/HallFrame.prefab";
        [UIPath("offset/InputIp")]
        private InputField _inputIp;
        [UIPath("offset/btnFight")]
        private Button _btnFight;
        [UIPath("offset/btnJoin")]
        private Button _btnJoin;
        [UIPath("offset/btnEdit")]
        private Button _btnEdit;
        public override void Init(Transform parent)
        {
            base.Init(parent);
            _btnFight.onClick.AddListener(() =>
            {
                NetworkManager.singleton.networkAddress = "localhost";
                Game.GameInstance.Instance.Mode = GameMode.Host;
                LevelManager.Instance.Enter(LevelType.Fight);
            });
            _btnJoin.onClick.AddListener(() =>
            {
                if (!string.IsNullOrEmpty(_inputIp.text))
                {
                    NetworkManager.singleton.networkAddress = _inputIp.text;
                }
                else
                {
                    NetworkManager.singleton.networkAddress = "localhost";
                }
                Game.GameInstance.Instance.Mode = GameMode.Client;
                LevelManager.Instance.Enter(LevelType.Fight);
            });
            _btnEdit.onClick.AddListener(() =>
            {
                Game.GameInstance.Instance.Mode = GameMode.Edit;
                LevelManager.Instance.Enter(LevelType.Edit);
            });
        }
    }
}