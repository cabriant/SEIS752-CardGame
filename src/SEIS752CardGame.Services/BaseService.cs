
namespace SEIS752CardGame.Services
{
    public abstract class BaseService<T, I>
        where T : new()
        where I : class
    {
        private static I _instance = new T() as I;
        public static I Instance { get { return _instance; } }

        public static void SetInstance(I instance)
        {
            _instance = instance;
        }
    }
}
