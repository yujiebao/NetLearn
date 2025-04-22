using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using System.Text;
using UnityEngine;

public class Lessson4 : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        #region 将字二进制节数组转换为类对象
        PlayerInfo playerInfo = new PlayerInfo();
        playerInfo.name = "小明";
        playerInfo.age = 18;
        playerInfo.height = 1.8f;
        playerInfo.isMale = true;
        byte[] playerBytes = playerInfo.GetBytes(); //获取字节数组

        //2.将字节数组按照序列化时的顺序进行反序列化(将对应字节分组转换为对应类型变量)
        PlayerInfo info2 = new PlayerInfo(); //创建一个新的对象
        int index = 0;
        //age
        info2.age = BitConverter.ToInt32(playerBytes, 0);
        index += sizeof(int);
        //name
        int nameLength = BitConverter.ToInt32(playerBytes, index); //获取字符串的字节长度
        index += sizeof(int); //int类型的nameLength占用4个字节
        info2.name = Encoding.UTF8.GetString(playerBytes, index, nameLength); //获取字符串
        index += nameLength;
        //height
        info2.height = BitConverter.ToSingle(playerBytes, index); //获取float类型的身高
        index += sizeof(float);
        //isMale
        info2.isMale = BitConverter.ToBoolean(playerBytes, index); //获取bool类型的性别
        index += sizeof(bool);

        print(info2);
        #endregion

        #region 总结
        //我们对类对象的2进制反序列化主要用到的知识点是
        //1.Bitconverter转换字节数组为非字符串的类型的变量
        //2.Encoding.UTF8转换字节数组为字符串类型的变量(注意:先读长度，再读字符串）
        //转换流程是
        //1.获取到对应的字节数组
        //2.将字节数组按照序列化时的顺序进行反序列化(将对应字节分组转换为对应类型变量)
        #endregion
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
