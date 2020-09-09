using System;
using System.IO;

namespace Sylver.Network.Data
{
    public interface INetPacketStream : IDisposable
    {
        /// <summary>
        /// Gets the packet stream state.
        /// </summary>
        NetPacketStateType State { get; }

        /// <summary>
        /// Gets a value that indicates if the current cursor is at the end of the packet stream.
        /// </summary>
        bool IsEndOfStream { get; }

        /// <summary>
        /// Gets the length of the packet stream.
        /// </summary>
        long Length { get; }

        /// <summary>
        /// Gets the current position of the cursor in the packet stream.
        /// </summary>
        long Position { get; }

        /// <summary>
        /// Gets the packet stream buffer.
        /// </summary>
        byte[] Buffer { get; }

        int ReadByte();

        sbyte ReadSByte();

        char ReadChar();

        bool ReadBoolean();

        short ReadInt16();

        ushort ReadUInt16();

        int ReadInt32();

        uint ReadUInt32();

        long ReadInt64();

        ulong ReadUInt64();

        float ReadSingle();

        double ReadDouble();

        string ReadString();

        byte[] ReadBytes(int count);

        /// <summary>
        /// Reads a <typeparamref name="T"/> value from the packet stream.
        /// </summary>
        /// <typeparam name="T">Type of the value to read.</typeparam>
        /// <returns>The value read and converted to the type.</returns>
        T Read<T>();

        /// <summary>
        /// Reads an array of <typeparamref name="T"/> value from the packet.
        /// </summary>
        /// <typeparam name="T">Value type.</typeparam>
        /// <param name="amount">Amount to read.</param>
        /// <returns>An array of type <typeparamref name="T"/>.</returns>
        T[] Read<T>(int amount);

        void WriteByte(byte value);

        void WriteSByte(sbyte value);

        void WriteChar(char value);

        void WriteBoolean(bool value);

        void WriteInt16(short value);

        void WriteUInt16(ushort value);

        void WriteInt32(int value);

        void WriteUInt32(uint value);

        void WriteSingle(float value);

        void WriteDouble(double value);

        void WriteInt64(long value);

        void WriteUInt64(ulong value);

        void WriteString(string value);

        void WriteBytes(byte[] values);

        /// <summary>
        /// Writes a <typeparamref name="T"/> value in the packet stream.
        /// </summary>
        /// <typeparam name="T">Type of the value.</typeparam>
        /// <param name="value">Value to write in the packet stream.</param>
        void Write<T>(T value);

        /// <summary>
        /// Sets the position within the current stream to the specified value.
        /// </summary>
        /// <param name="offset">The new position within the stream. This is relative to the loc parameter, and can be positive or negative.</param>
        /// <param name="loc">A value of type <see cref="SeekOrigin"></see>, which acts as the seek reference point.</param>
        /// <returns>
        /// The new position within the stream, calculated by combining the initial 
        /// reference point and the offset.
        /// </returns>
        long Seek(long offset, SeekOrigin loc);
    }
}
