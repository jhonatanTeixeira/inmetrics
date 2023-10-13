namespace Application.Service
{
    public interface IMessagingService<T>
    {
        public Task SendMessage(string eventName, T data);
    }
}