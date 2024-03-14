namespace Game.Script.Character.Skill
{
    public class SkillAction
    {
        private float _executeTime = 0;
        public virtual string GetDefaultParam()
        {
            return string.Empty;
        }
        public virtual void ParseParam(string param){}

        public virtual string OnGui(string param)
        {
            return string.Empty;
        }

        public float ExecuteTime => _executeTime;
        
        public virtual void Init(float time, string param)
        {
            _executeTime = time;
            ParseParam(param);
        }
        public virtual void Execute(BaseController controller){}
    }
}