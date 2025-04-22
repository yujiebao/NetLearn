using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

/// <summary>
/// 玩家数据类
/// </summary> <summary>
/// 
/// </summary>
public class PlayerData : BaseData
{
    public string name; //玩家姓名
    public int atk;
    public int def;
    public override int GetBytesNum()
    {
        return sizeof(int) + sizeof(int) + sizeof(int) + Encoding.UTF8.GetBytes(name).Length;  //有一个int存储的是字符串的长度
    }

    public override int Reading(byte[] bytes, int beginIndex = 0)
    {
        int index = beginIndex;
        name = ReadString(bytes, ref index);
        atk = ReadInt(bytes, ref index); //读取int类型的atkl
        def = ReadInt(bytes, ref index); //读取int类型的defl
        return index - beginIndex; //返回读取的字节数
    }

    public override byte[] Writing()
    {
        int index = 0;
        byte[] bytes = new byte[GetBytesNum()];
        WriteString(bytes, name, ref index); //写入字符串
        WriteInt(bytes, atk, ref index); //写入int类型的atkl
        WriteInt(bytes, def, ref index); //写入int类型的defl
        return bytes; //返回字节数组
    }
}
