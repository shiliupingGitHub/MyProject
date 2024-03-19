using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Script.UI.Frames
{
    public class EditFrame : Frame
    {
        protected override string ResPath => "Assets/Game/Res/UI/EditFrame.prefab";

        private Dropdown mapDropDown;
        

        void InitActors()
        {
            var actorTemplate = FrameGo.transform.Find("ActorTemplate").gameObject;
            var contentRoot = FrameGo.transform.Find("svActors/Viewport/Content");
            var actorConfigs = ActorConfig.dic;

            foreach (var actorConfig in actorConfigs)
            {
                var actorGo = Object.Instantiate(actorTemplate, contentRoot) as GameObject;
                actorGo.name = actorConfig.Value.id.ToString();
                var text = actorGo.transform.Find("Name").GetComponent<Text>();
                text.text = actorConfig.Value.name;
                actorGo.SetActive(true);
            }
        }

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
            InitActors();
        }
    }
}