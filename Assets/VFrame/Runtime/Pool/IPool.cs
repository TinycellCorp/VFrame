namespace VFrame.Pool
{
    public interface IPool<T>
    {
        T Rent();
        void Return(T instance);
    }
}