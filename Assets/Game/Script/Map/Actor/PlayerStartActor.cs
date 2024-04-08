
using Game.Script.Attribute;

namespace Game.Script.Map.Actor
{
    public class PlayerStartActor : MapActor
    {
        [ActorDataDes()] public float P1;
        [ActorDataDes()] public string P2;
        [ActorDataDes()] public int P3;
        [ActorDataDes()] public bool P4;
    }
}