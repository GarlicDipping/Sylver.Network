using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Schema;

namespace Sylver.Network.Data
{
    /// <summary>
    /// Provides a mechanism to read inside a packet stream.
    /// </summary>
    public class NetPacketStream : MemoryStream, INetPacketStream
    {
        private readonly BinaryReader _reader;
        private readonly BinaryWriter _writer;

        /// <inheritdoc />
        public NetPacketStateType State { get; }

        /// <inheritdoc />
        public bool IsEndOfStream => Position >= Length;

        /// <inheritdoc />
        public virtual byte[] Buffer => TryGetBuffer(out ArraySegment<byte> buffer) ? buffer.ToArray() : Array.Empty<byte>();

        /// <summary>
        /// Gets the encoding used to encode strings when writing on the packet stream.
        /// </summary>
        protected virtual Encoding WriteEncoding => Encoding.UTF8;

        /// <summary>
        /// Gets the encoding used to decode strings when reading from the packet stream.
        /// </summary>
        protected virtual Encoding ReadEncoding => Encoding.UTF8;

        /// <summary>
        /// Creates and initializes a new <see cref="NetPacketStream"/> instance in write-only mode.
        /// </summary>
        public NetPacketStream()
        {
            _writer = new BinaryWriter(this, WriteEncoding);
            State = NetPacketStateType.Write;
        }

        /// <summary>
        /// Creates and initializes a new <see cref="NetPacketStream"/> instance in read-only mode.
        /// </summary>
        /// <param name="buffer">Input buffer</param>
        public NetPacketStream(byte[] buffer)
            : base(buffer, 0, buffer?.Length ?? throw new ArgumentNullException(nameof(buffer)), false, true)
        {
            _reader = new BinaryReader(this, ReadEncoding);
            State = NetPacketStateType.Read;
        }

        /// <inheritdoc />
        public virtual sbyte ReadSByte() => _reader.ReadSByte();

        /// <inheritdoc />
        public virtual char ReadChar() => _reader.ReadChar();

        /// <inheritdoc />
        public virtual bool ReadBoolean() => _reader.ReadBoolean();

        /// <inheritdoc />
        public virtual short ReadInt16() => _reader.ReadInt16();

        /// <inheritdoc />
        public virtual ushort ReadUInt16() => _reader.ReadUInt16();

        /// <inheritdoc />
        public virtual int ReadInt32() => _reader.ReadInt32();

        /// <inheritdoc />
        public virtual uint ReadUInt32() => _reader.ReadUInt32();

        /// <inheritdoc />
        public virtual long ReadInt64() => _reader.ReadInt64();

        /// <inheritdoc />
        public virtual ulong ReadUInt64() => _reader.ReadUInt64();

        /// <inheritdoc />
        public virtual float ReadSingle() => _reader.ReadSingle();

        /// <inheritdoc />
        public virtual double ReadDouble() => _reader.ReadDouble();

        /// <inheritdoc />
        public virtual string ReadString()
        {
            int stringLength = ReadInt32();
            byte[] stringBytes = ReadBytes(stringLength);

            return ReadEncoding.GetString(stringBytes);
        }

        /// <inheritdoc />
        public virtual byte[] ReadBytes(int count) => _reader.ReadBytes(count);

        /// <inheritdoc />
        public virtual T Read<T>()
        {
            if (State != NetPacketStateType.Read)
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
            if (State != NetPacketStateType.Read)
            {
                throw new InvalidOperationException($"The current packet stream is in write-only mode.");
            }

            if (amount <= 0)
            {
                throw new ArgumentException($"Amount is '{amount}' and must be grather than 0.", nameof(amount));
            }

            Type type = typeof(T);
            var array = new T[amount];

            if (type == typeof(byte))
            {
                array = _reader.ReadBytes(amount) as T[];
            }
            else
            {
                for (var i = 0; i < amount; i++)
                {
                    array[i] = Read<T>();
                }
            }

            return array;
        }

        /// <inheritdoc />
        public virtual void WriteSByte(sbyte value) => _writer.Write(value);

        /// <inheritdoc />
        public virtual void WriteChar(char value) => _writer.Write(value);

        /// <inheritdoc />
        public virtual void WriteBoolean(bool value) => _writer.Write(value);

        /// <inheritdoc />
        public virtual void WriteInt16(short value) => _writer.Write(value);

        /// <inheritdoc />
        public virtual void WriteUInt16(ushort value) => _writer.Write(value);

        /// <inheritdoc />
        public virtual void WriteInt32(int value) => _writer.Write(value);

        /// <inheritdoc />
        public virtual void WriteUInt32(uint value) => _writer.Write(value);

        /// <inheritdoc />
        public virtual void WriteSingle(float value) => _writer.Write(value);

        /// <inheritdoc />
        public virtual void WriteDouble(double value) => _writer.Write(value);

        /// <inheritdoc />
        public virtual void WriteInt64(long value) => _writer.Write(value);

        /// <inheritdoc />
        public virtual void WriteUInt64(ulong value) => _writer.Write(value);

        /// <inheritdoc />
        public virtual void WriteString(string value)
        {
            if (value == null)
            {
                return;
            }

            WriteInt32(value.Length);

            if (value.Length > 0)
            {
                WriteBytes(WriteEncoding.GetBytes(value));
            }
        }

        /// <inheritdoc />
        public virtual void WriteBytes(byte[] values) => Write(values);

        /// <inheritdoc />
        public virtual void Write<T>(T value)
        {
            if (State != NetPacketStateType.Write)
            {
                throw new InvalidOperationException($"The current packet stream is in read-only mode.");
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
                TypeCode.Byte => (byte)ReadByte(),
                TypeCode.SByte => ReadSByte(),
                TypeCode.Boolean => ReadBoolean(),
                TypeCode.Char => ReadChar(),
                TypeCode.Int16 => ReadInt16(),
                TypeCode.UInt16 => ReadUInt16(),
                TypeCode.Int32 => ReadInt32(),
                TypeCode.UInt32 => ReadUInt32(),
                TypeCode.Single => ReadSingle(),
                TypeCode.Double => ReadDouble(),
                TypeCode.Int64 => ReadInt64(),
                TypeCode.UInt64 => ReadUInt64(),
                TypeCode.String => ReadString(),
                _ => default
            };

            return (T)primitiveValue;
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
