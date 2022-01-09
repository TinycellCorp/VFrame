namespace VFrame.Pool
{
    public interface IRentable<in TParam>
    {
        void Rented(TParam p1);
    }
    
    public interface IRentable<in TParam1, in TParam2>
    {
        void Rented(TParam1 p1, TParam2 p2);
    }
}