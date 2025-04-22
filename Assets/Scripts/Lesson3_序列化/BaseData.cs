using System;
using System.Text;

public abstract class BaseData
{
    // 获取字节数的方法，子类必须实现
    public abstract int GetBytesNum();

    // 将成员变量序列化为字节数组，子类必须实现
    public abstract byte[] Writing();

    // 从字节数组反序列化为成员变量，子类必须实现
    public abstract int Reading(byte[] bytes, int beginIndex = 0);

    // 将int值写入字节数组
    protected void WriteInt(byte[] bytes, int value, ref int index)
    {
        BitConverter.GetBytes(value).CopyTo(bytes, index);
        index += sizeof(int);
    }

    // 将short值写入字节数组
    protected void WriteShort(byte[] bytes, short value, ref int index)
    {
        BitConverter.GetBytes(value).CopyTo(bytes, index);
        index += sizeof(short);
    }

    // 将long值写入字节数组
    protected void WriteLong(byte[] bytes, long value, ref int index)
    {
        BitConverter.GetBytes(value).CopyTo(bytes, index);
        index += sizeof(long);
    }

    // 将float值写入字节数组
    protected void WriteFloat(byte[] bytes, float value, ref int index)
    {
        BitConverter.GetBytes(value).CopyTo(bytes, index);
        index += sizeof(float);
    }

    // 将byte值写入字节数组
    protected void WriteByte(byte[] bytes, byte value, ref int index)
    {
        bytes[index] = value;
        index += sizeof(byte);
    }

    // 将bool值写入字节数组
    protected void WriteBool(byte[] bytes, bool value, ref int index)
    {
        BitConverter.GetBytes(value).CopyTo(bytes, index);
        index += sizeof(bool);
    }

    // 将string值写入字节数组
    protected void WriteString(byte[] bytes, string value, ref int index)
    {
        byte[] strBytes = Encoding.UTF8.GetBytes(value);
        WriteInt(bytes, strBytes.Length, ref index);
        strBytes.CopyTo(bytes, index);
        index += strBytes.Length;
    }

    // 将BaseData对象写入字节数组
    protected void WriteData(byte[] bytes, BaseData data, ref int index)
    {
        data.Writing().CopyTo(bytes, index);
        index += data.GetBytesNum();
    }

    // 从字节数组读取int值
    protected int ReadInt(byte[] bytes, ref int index)
    {
        int value = BitConverter.ToInt32(bytes, index);
        index += sizeof(int);
        return value;
    }

    // 从字节数组读取short值
    protected short ReadShort(byte[] bytes, ref int index)
    {
        short value = BitConverter.ToInt16(bytes, index);
        index += sizeof(short);
        return value;
    }

    // 从字节数组读取long值
    protected long ReadLong(byte[] bytes, ref int index)
    {
        long value = BitConverter.ToInt64(bytes, index);
        index += sizeof(long);
        return value;
    }

    // 从字节数组读取float值
    protected float ReadFloat(byte[] bytes, ref int index)
    {
        float value = BitConverter.ToSingle(bytes, index);
        index += sizeof(float);
        return value;
    }

    // 从字节数组读取byte值
    protected byte ReadByte(byte[] bytes, ref int index)
    {
        byte value = bytes[index];
        index += sizeof(byte);
        return value;
    }

    // 从字节数组读取bool值
    protected bool ReadBool(byte[] bytes, ref int index)
    {
        bool value = BitConverter.ToBoolean(bytes, index);
        index += sizeof(bool);
        return value;
    }

    // 从字节数组读取string值
    protected string ReadString(byte[] bytes, ref int index)
    {
        int length = ReadInt(bytes, ref index);
        string value = Encoding.UTF8.GetString(bytes, index, length);
        index += length;
        return value;
    }

    // 从字节数组读取BaseData对象
    protected T ReadData<T>(byte[] bytes, ref int index) where T:BaseData,new()
    {
        T value = new T();
        index += value.Reading(bytes, index);
        return value;
    }
}