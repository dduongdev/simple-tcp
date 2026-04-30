using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace SimpleTcp
{
    /// <summary>
    /// Represents a simple TCP client abstraction that supports sending and receiving
    /// raw messages or serialized objects over a TCP connection.
    /// </summary>
    /// <remarks>
    /// Implementations are responsible for handling message framing (e.g., length-prefix)
    /// and serialization/deserialization of objects.
    /// </remarks>
    public interface ISimpleTcpClient
    {
        /// <summary>
        /// Sends a UTF-8 encoded string message asynchronously.
        /// </summary>
        /// <param name="message">The message to send. Must not be null.</param>
        /// <returns>Total number of bytes sent.</returns>
        /// <exception cref="SimpleTcp.Exceptions.ConnectionClosedException">
        /// Thrown when the connection is closed during sending.
        /// </exception>
        Task<int> SendAsync(string message);

        /// <summary>
        /// Serializes the specified object to JSON and sends it asynchronously.
        /// </summary>
        /// <typeparam name="T">Type of the object to send.</typeparam>
        /// <param name="obj">The object to serialize and send.</param>
        /// <returns>Total number of bytes sent.</returns>
        /// <exception cref="SimpleTcp.Exceptions.ConnectionClosedException">
        /// Thrown when the connection is closed during sending.
        /// </exception>
        Task<int> SendObjectAsync<T>(T obj);

        /// <summary>
        /// Receives a message asynchronously and deserializes it into the specified type.
        /// </summary>
        /// <typeparam name="T">Target type to deserialize into.</typeparam>
        /// <returns>
        /// The deserialized object, or null if deserialization fails or payload is invalid.
        /// </returns>
        /// <exception cref="SimpleTcp.Exceptions.ConnectionClosedException">
        /// Thrown when the connection is closed during receiving.
        /// </exception>
        Task<T?> ReceiveObjectAsync<T>();

        /// <summary>
        /// Receives raw message bytes asynchronously.
        /// </summary>
        /// <returns>The received message as a byte array.</returns>
        /// <exception cref="SimpleTcp.Exceptions.ConnectionClosedException">
        /// Thrown when the connection is closed during receiving.
        /// </exception>
        Task<byte[]> ReceiveAsync();

        /// <summary>
        /// Sends a UTF-8 encoded string message synchronously.
        /// </summary>
        /// <param name="message">The message to send. Must not be null.</param>
        /// <returns>Total number of bytes sent.</returns>
        int Send(string message);

        /// <summary>
        /// Serializes the specified object to JSON and sends it synchronously.
        /// </summary>
        /// <typeparam name="T">Type of the object to send.</typeparam>
        /// <param name="obj">The object to serialize and send.</param>
        /// <returns>Total number of bytes sent.</returns>
        int SendObject<T>(T obj);

        /// <summary>
        /// Receives a message synchronously and deserializes it into the specified type.
        /// </summary>
        /// <typeparam name="T">Target type to deserialize into.</typeparam>
        /// <returns>
        /// The deserialized object, or null if deserialization fails or payload is invalid.
        /// </returns>
        T? ReceiveObject<T>();

        /// <summary>
        /// Receives raw message bytes synchronously.
        /// </summary>
        /// <returns>The received message as a byte array.</returns>
        byte[] Receive();

        /// <summary>
        /// Closes the underlying TCP connection and releases associated resources.
        /// </summary>
        void Close();
    }
}