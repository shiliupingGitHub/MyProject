namespace Game.Script.Character.Skill
{
    public class SkillAction
    {
        public virtual string GetDefaultParam()
        {
            return string.Empty;
        }
        public virtual void ParseParam(string param){}

        public virtual string OnGui(string param)
        {
            return string.Empty;
        }
    }
}