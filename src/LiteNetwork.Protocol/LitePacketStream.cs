using LiteNetwork.Protocol.Abstractions;
using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

namespace LiteNetwork.Protocol
{
    /// <summary>
    /// Provides a mechanism to read inside a packet stream.
    /// </summary>
    public class LitePacketStream : MemoryStream, ILitePacketStream
    {
        private readonly BinaryReader _reader = null!;
        private readonly BinaryWriter _writer = null!;

        /// <inheritdoc />
        public LitePacketMode Mode { get; }

        /// <inheritdoc />
        public bool IsEndOfStream => Position >= Length;

        /// <inheritdoc />
        public virtual byte[] Buffer => TryGetBuffer(out ArraySegment<byte> buffer) ? buffer.ToArray() : Array.Empty<byte>();

        /// <summary>
        /// Gets the encoding used to encode strings when writing on the packet stream.
        /// </summary>
        /// <remarks>
        /// Default encoding is UTF-8.
        /// </remarks>
        protected virtual Encoding WriteEncoding => Encoding.UTF8;

        /// <summary>
        /// Gets the encoding used to decode strings when reading from the packet stream.
        /// </summary>
        /// <remarks>
        /// Default encoding is UTF-8.
        /// </remarks>
        protected virtual Encoding ReadEncoding => Encoding.UTF8;

        /// <summary>
        /// Creates and initializes a new <see cref="LitePacketStream"/> instance in write-only mode.
        /// </summary>
        public LitePacketStream()
        {
            _writer = new BinaryWriter(this, WriteEncoding);
            Mode = LitePacketMode.Write;
        }

        /// <summary>
        /// Creates and initializes a new <see cref="LitePacketStream"/> instance in read-only mode.
        /// </summary>
        /// <param name="buffer">Input buffer</param>
        public LitePacketStream(byte[] buffer)
            : base(buffer, 0, buffer?.Length ?? throw new ArgumentNullException(nameof(buffer)), false, true)
        {
            _reader = new BinaryReader(this, ReadEncoding);
            Mode = LitePacketMode.Read;
        }

        /// <inheritdoc />
        public virtual new byte ReadByte() => Read<byte>();

        /// <inheritdoc />
        public virtual sbyte ReadSByte() => Read<sbyte>();

        /// <inheritdoc />
        public virtual char ReadChar() => Read<char>();

        /// <inheritdoc />
        public virtual bool ReadBoolean() => Read<bool>();

        /// <inheritdoc />
        public virtual short ReadInt16() => Read<short>();

        /// <inheritdoc />
        public virtual ushort ReadUInt16() => Read<ushort>();

        /// <inheritdoc />
        public virtual int ReadInt32() => Read<int>();

        /// <inheritdoc />
        public virtual uint ReadUInt32() => Read<uint>();

        /// <inheritdoc />
        public virtual long ReadInt64() => Read<long>();

        /// <inheritdoc />
        public virtual ulong ReadUInt64() => Read<ulong>();

        /// <inheritdoc />
        public virtual float ReadSingle() => Read<float>();

        /// <inheritdoc />
        public virtual double ReadDouble() => Read<double>();

        /// <inheritdoc />
        public virtual string ReadString() => Read<string>();

        /// <inheritdoc />
        public virtual byte[] ReadBytes(int count) => _reader.ReadBytes(count);

        /// <inheritdoc />
        public virtual T Read<T>()
        {
            if (Mode != LitePacketMode.Read)
            {
                throw new InvalidOperationException($"The current packet stream is in write-only mode.");
            }

            if (typeof(T).IsPrimitive || typeof(T) == typeof(string))
            {
                return ReadPrimitive<T>();
            }

            throw new NotImplementedException($"Cannot read a {typeof(T)} value from the packet stream.");
        }

        /// <inheritdoc />
        public virtual T[] Read<T>(int amount)
        {
            if (Mode != LitePacketMode.Read)
            {
                throw new InvalidOperationException($"The current packet stream is in write-only mode.");
            }

            if (amount <= 0)
            {
                throw new ArgumentException($"Amount is '{amount}' and must be grather than 0.", nameof(amount));
            }

            if (typeof(T) == typeof(byte))
            {
                return _reader.ReadBytes(amount) as T[] ?? throw new IOException("An error occurred while reading a packet stream.");
            }

            var array = new T[amount];

            for (var i = 0; i < amount; i++)
            {
                array[i] = Read<T>();
            }

            return array;
        }

        /// <inheritdoc />
        public virtual void WriteSByte(sbyte value) => Write(value);

        /// <inheritdoc />
        public virtual void WriteChar(char value) => Write(value);

        /// <inheritdoc />
        public virtual void WriteBoolean(bool value) => Write(value);

        /// <inheritdoc />
        public virtual void WriteInt16(short value) => Write(value);

        /// <inheritdoc />
        public virtual void WriteUInt16(ushort value) => Write(value);

        /// <inheritdoc />
        public virtual void WriteInt32(int value) => Write(value);

        /// <inheritdoc />
        public virtual void WriteUInt32(uint value) => Write(value);

        /// <inheritdoc />
        public virtual void WriteSingle(float value) => Write(value);

        /// <inheritdoc />
        public virtual void WriteDouble(double value) => Write(value);

        /// <inheritdoc />
        public virtual void WriteInt64(long value) => Write(value);

        /// <inheritdoc />
        public virtual void WriteUInt64(ulong value) => Write(value);

        /// <inheritdoc />
        public virtual void WriteString(string value) => Write(value);

        /// <inheritdoc />
        public virtual void WriteBytes(byte[] values) => Write(values);

        /// <inheritdoc />
        public virtual void Write<T>(T value)
        {
            if (Mode != LitePacketMode.Write)
            {
                throw new InvalidOperationException($"The current packet stream is in read-only mode.");
            }

            if (value is null)
            {
                throw new ArgumentNullException("Cannot write a null value into the packet stream.");
            }

            if (typeof(T).IsPrimitive || typeof(T) == typeof(string))
            {
                WritePrimitive<T>(value);
            }
            else
            {
                throw new NotImplementedException($"Cannot write a {typeof(T)} value into the packet stream.");
            }
        }

        /// <summary>
        /// Read a primitive type from the packet stream.
        /// </summary>
        /// <typeparam name="T">Type to read.</typeparam>
        /// <returns></returns>
        private T ReadPrimitive<T>()
        {
            object primitiveValue = Type.GetTypeCode(typeof(T)) switch
            {
                TypeCode.Byte => _reader.ReadByte(),
                TypeCode.SByte => _reader.ReadSByte(),
                TypeCode.Boolean => _reader.ReadBoolean(),
                TypeCode.Char => _reader.ReadChar(),
                TypeCode.Int16 => _reader.ReadInt16(),
                TypeCode.UInt16 => _reader.ReadUInt16(),
                TypeCode.Int32 => _reader.ReadInt32(),
                TypeCode.UInt32 => _reader.ReadUInt32(),
                TypeCode.Single => _reader.ReadSingle(),
                TypeCode.Double => _reader.ReadDouble(),
                TypeCode.Int64 => _reader.ReadInt64(),
                TypeCode.UInt64 => _reader.ReadUInt64(),
                TypeCode.String => InternalReadString(),
                _ => throw new NotImplementedException(),
            };

            return (T)primitiveValue;
        }

        /// <summary>
        /// Reads a string from the current packet stream.
        /// </summary>
        /// <returns></returns>
        private string InternalReadString()
        {
            int stringLength = ReadInt32();
            byte[] stringBytes = ReadBytes(stringLength);

            return ReadEncoding.GetString(stringBytes);
        }

        /// <summary>
        /// Writes a primitive type to the packet stream.
        /// </summary>
        /// <typeparam name="T">Type to write.</typeparam>
        /// <param name="value">Value to write.</param>
        private void WritePrimitive<T>(object value)
        {
            switch (Type.GetTypeCode(typeof(T)))
            {
                case TypeCode.Byte:
                    _writer.Write(Convert.ToByte(value, CultureInfo.DefaultThreadCurrentCulture));
                    break;
                case TypeCode.SByte:
                    _writer.Write(Convert.ToSByte(value, CultureInfo.DefaultThreadCurrentCulture));
                    break;
                case TypeCode.Boolean:
                    _writer.Write(Convert.ToBoolean(value, CultureInfo.DefaultThreadCurrentCulture));
                    break;
                case TypeCode.Char:
                    _writer.Write(Convert.ToChar(value, CultureInfo.DefaultThreadCurrentCulture));
                    break;
                case TypeCode.Int16:
                    _writer.Write(Convert.ToInt16(value, CultureInfo.DefaultThreadCurrentCulture));
                    break;
                case TypeCode.UInt16:
                    _writer.Write(Convert.ToUInt16(value, CultureInfo.DefaultThreadCurrentCulture));
                    break;
                case TypeCode.Int32:
                    _writer.Write(Convert.ToInt32(value, CultureInfo.DefaultThreadCurrentCulture));
                    break;
                case TypeCode.UInt32:
                    _writer.Write(Convert.ToUInt32(value, CultureInfo.DefaultThreadCurrentCulture));
                    break;
                case TypeCode.Single:
                    _writer.Write(Convert.ToSingle(value, CultureInfo.DefaultThreadCurrentCulture));
                    break;
                case TypeCode.Int64:
                    _writer.Write(Convert.ToInt64(value, CultureInfo.DefaultThreadCurrentCulture));
                    break;
                case TypeCode.UInt64:
                    _writer.Write(Convert.ToUInt64(value, CultureInfo.DefaultThreadCurrentCulture));
                    break;
                case TypeCode.Double:
                    _writer.Write(Convert.ToDouble(value, CultureInfo.DefaultThreadCurrentCulture));
                    break;
                case TypeCode.String:
                    {
                        if (value == null)
                        {
                            return;
                        }

                        string stringValue = value.ToString();

                        _writer.Write(stringValue.Length);

                        if (stringValue.Length > 0)
                        {
                            _writer.Write(WriteEncoding.GetBytes(stringValue));
                        }
                    }
                    break;
            }
        }
    }
}
