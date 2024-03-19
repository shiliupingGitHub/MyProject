using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Script.UI.Frames
{
    public class EditFrame : Frame
    {
        protected override string ResPath => "Assets/Game/Res/UI/EditFrame.prefab";

        private Dropdown mapDropDown;

        void InitMaps()
        {
            mapDropDown = FrameGo.transform.Find("ddMap").GetComponent<Dropdown>();
            var mapConfigs = MapConfig.dic;

            List<string> mapDDContent = new();
            mapDropDown.ClearOptions();
            for (int i = 1; i <= mapConfigs.Count; i++)
            {
                var mapConfig = mapConfigs[i];
                
                mapDDContent.Add(mapConfig.name);
                
            }
            mapDropDown.AddOptions(mapDDContent);
        }
        public override void Init(Transform parent)
        {
            base.Init(parent);
            FrameGo.transform.Find("btnNew").GetComponent<Button>().onClick.AddListener(() =>
            {
                
            });
            InitMaps();
        }
    }
}