using Game.Script.Attribute;

namespace Game.Script.Map
{
    [MapActionDes(MapActionType.BornMonster)]
    public class MapActonBornMonster : MapAction
    {
        [Label("test1")]
        public int p1;
        [Label("test2")]
        public string p2;
        [Label("test3")]
        public float p3;
        public override void Execute()
        {
            
            
        }
    }
}