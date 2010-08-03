namespace StorEvil.Events
{
    public interface IHandle<T>
    {
        void Handle(T eventToHandle);
    }
}