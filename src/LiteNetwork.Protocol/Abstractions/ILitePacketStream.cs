using System;
using System.IO;

namespace LiteNetwork.Protocol.Abstractions
{
    public interface ILitePacketStream : IDisposable
    {
        /// <summary>
        /// Gets the packet stream mode.
        /// </summary>
        LitePacketMode Mode { get; }

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
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1819:Properties should not return arrays", Justification = "<Pending>")]
        byte[] Buffer { get; }

        /// <summary>
        /// Reads the next byte from the current packet stream and advances the current position of the packet stream by one byte.
        /// </summary>
        /// <returns>The next byte read from the current stream.</returns>
        byte ReadByte();

        /// <summary>
        /// Reads the next signed byte from the current packet stream and advances the current position of the packet stream by one byte.
        /// </summary>
        /// <returns></returns>
        sbyte ReadSByte();

        /// <summary>
        /// Reads the next character from the current packet stream and advances the current position of the packet stream by one byte.
        /// </summary>
        /// <returns></returns>
        char ReadChar();

        /// <summary>
        /// Reads the next boolean value from the current packet stream and advances the current position of the packet stream by one byte.
        /// </summary>
        /// <returns>True if non-zero; false otherwise.</returns>
        bool ReadBoolean();

        /// <summary>
        /// Reads a 2-bytes signed numeric value from the current packet stream and advances the current position of the packet stream by two bytes.
        /// </summary>
        /// <returns></returns>
        short ReadInt16();

        /// <summary>
        /// Reads a 2-bytes unsigned numeric value from the current packet stream and advances the current position of the packet stream by two bytes.
        /// </summary>
        /// <returns></returns>
        ushort ReadUInt16();

        /// <summary>
        /// Reads a 4-bytes signed numeric value from the current packet stream and advances the current position of the packet stream by four bytes.
        /// </summary>
        /// <returns></returns>
        int ReadInt32();

        /// <summary>
        /// Reads a 4-bytes unsigned numeric value from the current packet stream and advances the current position of the packet stream by four bytes.
        /// </summary>
        /// <returns></returns>
        uint ReadUInt32();

        /// <summary>
        /// Reads a 8-bytes signed numeric value from the current packet stream and advances the current position of the packet stream by eight bytes.
        /// </summary>
        /// <returns></returns>
        long ReadInt64();

        /// <summary>
        /// Reads a 8-bytes unsigned numeric value from the current packet stream and advances the current position of the packet stream by eight bytes.
        /// </summary>
        /// <returns></returns>
        ulong ReadUInt64();

        /// <summary>
        /// Reads a 4-bytes floating numeric value from the current packet stream and advances the current position of the packet stream by four bytes.
        /// </summary>
        /// <returns></returns>
        float ReadSingle();

        /// <summary>
        /// Reads a 8-bytes floating numeric value from the current packet stream and advances the current position of the packet stream by eight bytes.
        /// </summary>
        /// <returns></returns>
        double ReadDouble();

        /// <summary>
        /// Reads a string from the current packet stream, where the first 4-byte represents the string length, then advances the current position of the packet stream by the length of the string plus four bytes.
        /// </summary>
        /// <returns></returns>
        string ReadString();

        /// <summary>
        /// Reads a given amount of bytes from the current packet stream and advances the current position of the packet stream by the number of read bytes.
        /// </summary>
        /// <param name="count">Byte amount to read.</param>
        /// <returns></returns>
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

        /// <summary>
        /// Writes a byte to the current packet stream and advances the current packet stream position by one byte.
        /// </summary>
        /// <param name="value"></param>
        void WriteByte(byte value);

        /// <summary>
        /// Writes a signed byte to the current packet stream and advances the current packet stream position by one byte.
        /// </summary>
        /// <param name="value"></param>
        void WriteSByte(sbyte value);

        /// <summary>
        /// Writes a character to the current packet stream and advances the current packet stream position by one byte.
        /// </summary>
        /// <param name="value"></param>
        void WriteChar(char value);

        /// <summary>
        /// Writes a boolean value to the current packet stream and advances the current packet stream position by one byte.
        /// </summary>
        /// <param name="value"></param>
        void WriteBoolean(bool value);

        /// <summary>
        /// Writes a 2-bytes signed numeric value to the current packet stream and advances the current packet stream position byte two bytes.
        /// </summary>
        /// <param name="value"></param>
        void WriteInt16(short value);

        /// <summary>
        /// Writes a 2-bytes unsigned numeric value to the current packet stream and advances the current packet stream position byte two bytes.
        /// </summary>
        /// <param name="value"></param>
        void WriteUInt16(ushort value);

        /// <summary>
        /// Writes a 4-bytes signed numeric value to the current packet stream and advances the current packet stream position byte four bytes.
        /// </summary>
        /// <param name="value"></param>
        void WriteInt32(int value);

        /// <summary>
        /// Writes a 4-bytes signed numeric value to the current packet stream and advances the current packet stream position byte four bytes.
        /// </summary>
        /// <param name="value"></param>
        void WriteUInt32(uint value);

        /// <summary>
        /// Writes a 4-bytes floating numeric value to the current packet stream and advances the current packet stream position byte four bytes.
        /// </summary>
        /// <param name="value"></param>
        void WriteSingle(float value);

        /// <summary>
        /// Writes a 8-bytes floating numeric value to the current packet stream and advances the current packet stream position byte eight bytes.
        /// </summary>
        /// <param name="value"></param>
        void WriteDouble(double value);

        /// <summary>
        /// Writes a 8-bytes floating signed value to the current packet stream and advances the current packet stream position byte eight bytes.
        /// </summary>
        /// <param name="value"></param>
        void WriteInt64(long value);

        /// <summary>
        /// Writes a 8-bytes unsigned numeric value to the current packet stream and advances the current packet stream position byte eight bytes.
        /// </summary>
        /// <param name="value"></param>
        void WriteUInt64(ulong value);

        /// <summary>
        /// Writes a string to the current packet stream, where the first four bytes represents the string length and advances the current packet stream position by the string length + four bytes.
        /// </summary>
        /// <param name="value"></param>
        void WriteString(string value);

        /// <summary>
        /// Writes a given byte array to the current packet stream and advances the current packet stream position by the array length.
        /// </summary>
        /// <param name="values"></param>
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
