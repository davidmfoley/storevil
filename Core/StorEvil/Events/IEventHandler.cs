namespace StorEvil.Events
{
    public interface IEventHandler<T>
    {
        void Handle(T eventToHandle);
    }

    public interface IEventHandler<T1, T2> 
    {
        void Handle(T1 eventToHandle);
        void Handle(T2 eventToHandle);
    }

    public interface IEventHandler<T1, T2, T3>
    {
        void Handle(T1 eventToHandle);
        void Handle(T2 eventToHandle);
        void Handle(T3 eventToHandle);
    }

    public interface IEventHandler<T1, T2, T3, T4>
    {
        void Handle(T1 eventToHandle);
        void Handle(T2 eventToHandle);
        void Handle(T3 eventToHandle);
        void Handle(T4 eventToHandle);
    }
}