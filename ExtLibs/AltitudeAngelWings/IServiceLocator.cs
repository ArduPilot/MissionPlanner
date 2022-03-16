namespace AltitudeAngelWings
{
    public interface IServiceLocator
    {
        T Resolve<T>();
    }
}