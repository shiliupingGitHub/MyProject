
using Game.Script.Attribute;
using Game.Script.Common;
using Game.Script.Subsystem;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Script.UI.Frames
{
    public class FightFrame : Frame
    {
        protected override string ResPath => "Assets/Game/Res/UI/FightFrame.prefab";
        
        [UIPath("offset/lbLeftTime")] private Text _lbLeftTime;

        private LineRenderer _lineRenderer;
        public override void Init(Transform parent)
        {
            base.Init(parent);
            UpdateStartFightLeftTime();
            var fightSubsystem = Common.Game.Instance.GetSubsystem<FightSubsystem>();
            fightSubsystem.startLeftTimeChanged += UpdateStartFightLeftTime;

        }

        void UpdateStartFightLeftTime()
        {
            var fightSubsystem = Common.Game.Instance.GetSubsystem<FightSubsystem>();

            if (fightSubsystem.StartLeftTime > 0)
            {
                _lbLeftTime.enabled = true;
                _lbLeftTime.text = Mathf.RoundToInt(fightSubsystem.StartLeftTime).ToString();
            }
            else
            {
                _lbLeftTime.enabled = false;
            }
        }
        
        public override void Destroy()
        {
            base.Destroy();
            var fightSubsystem = Common.Game.Instance.GetSubsystem<FightSubsystem>();
            fightSubsystem.startLeftTimeChanged -= UpdateStartFightLeftTime;
        }
    }
}