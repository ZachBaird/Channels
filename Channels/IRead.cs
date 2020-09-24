using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Channels
{
    /// <summary>
    /// Interface for reading from a channel.
    /// </summary>
    /// <typeparam name="T">Data of some type that we'll be reading from the channel.</typeparam>
    public interface IRead<T>
    {
        Task<T> Read();
        bool IsComplete();
    }
}
