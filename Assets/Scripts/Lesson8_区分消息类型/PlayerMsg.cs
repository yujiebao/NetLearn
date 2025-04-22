using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMsg : BaseMsg
{
    public int playerID;
    public PlayerData PlayerData;
    public override byte[] Writing()
    {
        int index = 0;
        byte[] bytes = new byte[GetBytesNum()]; //创建一个字节数组容器
        //写消息的ID
        WriteInt(bytes, GetID(), ref index); //写入int类型的消息ID
        //写消息的长度
        WriteInt(bytes, GetBytesNum()-8, ref index); //写入int类型 消息体的具体长度 减去消息ID和消息体长度的字节数
        //写这个消息的成员变量
        WriteInt(bytes, playerID, ref index); //写入int类型的playerID
        WriteData(bytes, PlayerData, ref index); //写入玩家数据的字节数组
        return bytes;
    }

    public override int Reading(byte[] bytes, int beginIndex = 0)
    {
        //反序列化不需要去解析ID 因为在这一步之前 就应该把ID反序列化出来
        //用来判断到底使用哪一个自定义类来反序列化
        int index = beginIndex;
        playerID = ReadInt(bytes, ref index);
        PlayerData = ReadData<PlayerData>(bytes, ref index);
        return index - beginIndex; //返回读取的字节数
    }

    public override int GetBytesNum()
    {
        return  sizeof(int)+ sizeof(int) + sizeof(int) + PlayerData.GetBytesNum(); //消息ID + 信息体的长度(用于处理分包黏包)+ playerID的字节数 +玩家数据的字节数
    }

    /// <summary>
    /// 自定义的消息ID主要用于区分是哪一个消息类
    /// </summary>
    /// <returns></returns> <summary>
    public override int GetID()
    {
        return 1;
    }
}
