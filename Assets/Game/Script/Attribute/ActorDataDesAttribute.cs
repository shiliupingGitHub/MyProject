namespace Game.Script.Attribute
{
    public class ActorDataDesAttribute : System.Attribute
    {
        public string Name { get; }

        public ActorDataDesAttribute(string name)
        {
            this.Name = name;
        }
    }
}