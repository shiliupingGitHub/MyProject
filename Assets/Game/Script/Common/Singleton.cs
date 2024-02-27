namespace Game.Script.Common
{
    public interface IOnInstance
    {

        void OnOnInstance();
    }

    public class SingletonWithOnInstance<T>  : IOnInstance where T : IOnInstance, new()
    {
        private static T _instance;

        public virtual void OnOnInstance(){}
        public static T Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new T();
                    _instance.OnOnInstance();
                }

                return _instance;
            }
        }
    }
    public class Singleton<T>   where T:new()
    {
        private static T _instance;

        public static T Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new T();
                }

                return _instance;
            }
        }
    }
}