using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lesson9 : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        #region 知识点一 什么是分包和黏包
        //分包、黏包指在网络通信中由于各种因素(网络环境、API规则等)造成的消息与消息之间出现的两种状态
        //分包:一个消息分成了多个消息进行发送
        //黏包:一个消息和另一个消息黏在了一起

        //注意:分包和黏包可能同时发生
        #endregion

        #region 知识点二 如何解决分包和黏包的问题
        //现在的处理:
        //我们收到的消息都是以字节数组的形式在程序中体现
        //目前我们的处理规则是默认传过来的消息就是正常情况
        //前4个字节是消息ID
        //后面的字节数组全部用来反序列化
        //如果出现分包、黏包会导致我们反序列化报错

        //思考:
        //那么通过接收到的字节数组我们应该如何判断收到的字节数组处于以下状态
        //1.正常
        //2.分包
        //3.黏包

        //突破点:
        //如何判断一个消息没有出现分包或者黏包呢?
        //答案->消息长度
        //我们可以如同处理 区分消息类型 的逻辑一样
        //为消息添加头部，头部记录消息的长度
        //当我们接收到消息时，通过消息长度来判断是否分包、黏包合并处理
        //对消息进行拆分处理、合并处理
        //我们每次只处理完整的消息
        #endregion

        #region 知识点三 实践解决
        //1.为所有消息添加头部信息，用于存储其消息长度
        //2.根据分包、黏包的表现情况，修改接收消息处的逻辑
        #endregion
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
