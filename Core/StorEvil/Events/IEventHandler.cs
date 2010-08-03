namespace StorEvil.Events
{
    public interface IEventHandler<T>
    {
        void Handle(T eventToHandle);
    }
}