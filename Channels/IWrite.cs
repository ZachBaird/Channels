namespace Channels
{
    /// <summary>
    /// Interface for pushing onto a channel.
    /// </summary>
    /// <typeparam name="T">The data of some type we're pushing onto the channel.</typeparam>
    public interface IWrite<T>
    {
        void Push(T msg);
        void Complete();
    }
}
