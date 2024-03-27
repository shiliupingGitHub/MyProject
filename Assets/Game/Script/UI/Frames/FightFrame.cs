using System.Collections.Generic;
using Game.Script.Attribute;
using Game.Script.Game;
using Game.Script.Game.Subsystem;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Script.UI.Frames
{
    public class FightFrame : Frame
    {
        protected override string ResPath => "Assets/Game/Res/UI/FightFrame.prefab";

        [UIPath("offset/btnTestPath")] private Button btnTestPath;

        private LineRenderer _lineRenderer;
        public override void Init(Transform parent)
        {
            base.Init(parent);
            btnTestPath.onClick.AddListener(() =>
            {
                // var pathSystem = GameInstance.Instance.GetSubsystem<PathSubsystem>();
                //
                // var points = pathSystem.DoPath(new Vector3(4.55f, 0.53f, 0), GameInstance.Instance.MyController.transform.position, GameInstance.Instance.MapScript);
                //
                // if (_lineRenderer == null)
                // {
                //     GameObject go = new GameObject("testpath");
                //     _lineRenderer = go.AddComponent<LineRenderer>();
                //     
                // }
                //
                // List<Vector3> linePoints = new();
                // _lineRenderer.positionCount = points.Count;
                // foreach (var point in points)
                // {
                //     var o = GameInstance.Instance.MapScript.transform.position;
                //     var grid = GameInstance.Instance.MapScript.MyGrid;
                //     var p = new Vector3(point.Item1 * grid.cellSize.x  +  grid.cellSize.x * 0.5f, point.Item2 * grid.cellSize.y  + grid.cellSize.y * 0.5f, -1);
                //     p += o;
                //     linePoints.Add(p);
                // }
                // _lineRenderer.SetPositions(linePoints.ToArray());
            });
        }
    }
}