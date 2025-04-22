using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class PlayerInfo
{
    public string name;
    public int age;
    public float height;
    public bool isMale;
    private byte[] playerBytes;

    public byte[] GetBytes()
    {
        int num = sizeof(int) + sizeof(int) + sizeof(float) + sizeof(bool) + Encoding.UTF8.GetBytes(name).Length;
        playerBytes = new byte[num]; //创建一个字节数组容器
        int index = 0;  //从playerBytes的第几个位置开始存储
        //存储age
        BitConverter.GetBytes(age).CopyTo(playerBytes, index); //int类型的age
        index += sizeof(int); //int类型的age占用4个字节
        //存储name  先存长度再存内容
        byte[] nameBytes = Encoding.UTF8.GetBytes(name); 
        int nameLength = nameBytes.Length; //获取字符串的字节长度
        BitConverter.GetBytes(nameLength).CopyTo(playerBytes, index);
        index += sizeof(int); //int类型的nameLength占用4个字节
        nameBytes.CopyTo(playerBytes, index);
        index += nameLength; //nameBytes的长度
        //存储height
        BitConverter.GetBytes(height).CopyTo(playerBytes, index);
        index += sizeof(float); //float类型的height占用4个字节
        //存储isMale
        BitConverter.GetBytes(isMale).CopyTo(playerBytes, index);
        index += sizeof(bool); //bool类型的isMale占用1个字节
        return playerBytes;
    }

    public override string ToString()
    {
        return string.Format("姓名:{0},年龄:{1},身高:{2},性别:{3}", name, age, height, isMale);
    }
}

public class Lesson3 : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        #region 如何将一个类对象转换为二进制 
        // 注意:网络通信中我们不能直接使用数据持久化2进制知识点中的
        // BinaryFormatter 2进制格式化类
        // 因为客户端和服务器使用的语言可能不一样，BinaryFormatter是c#的序列化规则，和其它语言之间的兼容性不好
        // 如果使用它，那么其它语言开发的服务器无法对其进行反序列化
        // 我们需要自己来处理将类对象数据序列化为字节数组、
        
        //单纯的转换一个变量为字节数组非常的简单
        //但是我们如何将一个类对象携带的所有信息放入到一个字节数组中呢
        //我们需要做以下几步
        //1.明确字节数组的容量(注意:在确定字符串字节长度时要考虑解析时如何处理)
        PlayerInfo playerInfo = new PlayerInfo();
        playerInfo.name = "小明";
        playerInfo.age = 18;
        playerInfo.height = 1.8f;
        playerInfo.isMale = true;
        int indexNum = sizeof(int) + sizeof(int)+ sizeof(float) + sizeof(bool) + Encoding.UTF8.GetBytes(playerInfo.name).Length;
        
        //2.中明一个装载信息的字节数组容器
        byte[] playerBytes = new byte[indexNum]; 
        
        //3.将对象中的所有信息转为字节数组并放入该容器当中(可以利用数组中的copeTo方法转存字节数组)
        //copyTo方法的第二个参数代表 从容器的第几个位置开始存储
        int index = 0;  //从playerBytes的第几个位置开始存储
        //存储age
        BitConverter.GetBytes(playerInfo.age).CopyTo(playerBytes, index); //int类型的age
        index += sizeof(int); //int类型的age占用4个字节
        //存储name  先存长度再存内容
        byte[] nameBytes = Encoding.UTF8.GetBytes(playerInfo.name); 
        int nameLength = nameBytes.Length; //获取字符串的字节长度
        BitConverter.GetBytes(nameLength).CopyTo(playerBytes, index);
        index += sizeof(int); //int类型的nameLength占用4个字节
        nameBytes.CopyTo(playerBytes, index);
        index += nameLength; //nameBytes的长度
        //存储height
        BitConverter.GetBytes(playerInfo.height).CopyTo(playerBytes, index);
        index += sizeof(float); //float类型的height占用4个字节
        //存储isMale
        BitConverter.GetBytes(playerInfo.isMale).CopyTo(playerBytes, index);
        index += sizeof(bool); //bool类型的isMale占用1个字节
        print(indexNum);
        print(playerBytes.Length);

        print("---------------------------------");
        print(playerInfo.GetBytes().Length);
        #endregion

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
