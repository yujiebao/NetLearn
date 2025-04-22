using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class Lesson6 : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

        #region 知识点一 回顾客户端需要做的事情
        //1.创建套接字socket
        //2.用connect方法与服务端相连
        //3.用send和Receive日关方法收发数据
        //4.用shutdown方法释放连接
        //5.关闭套接字
        #endregion

        #region 知识点二 实现客户端的基本逻辑
        //1.创建套接字Socket
        Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        //2.用Connect方法与服务端相连
        IPEndPoint iPEndPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 8080);
        //确定服务端的IP和端口
        try
        {
            socket.Connect(iPEndPoint);
        }
        catch (SocketException e)
        {
             if(e.ErrorCode == 10061)
            {
                Debug.Log("连接失败，服务端未开启！");
            }
            else
            {
                Debug.Log("连接失败，错误码:" + e.ErrorCode);
            }
        }
        //3.用Send和Receive相关方法收发数据
        //接收数据
        byte[] bytes = new byte[1024];
        int receiveNum  = socket.Receive(bytes);   //一直使用原有的socket进行通信 而客户端则是返回一个新的socket进行通信

        
        //首先解析信息的ID
        //使用字节数组的前4个字节,得到消息ID
        int msgID = System.BitConverter.ToInt32(bytes, 0);
        Debug.Log("消息ID:" + msgID); //打印消息ID
        switch (msgID)
        {
            case 1:
                PlayerMsg msg = new PlayerMsg();
                msg.Reading(bytes, 4); //从第5个字节开始读取数据
                print("收到消息ID为1的消息,"+"序号为:"+ msg.playerID + "玩家姓名:" + msg.PlayerData.name + ",攻击力:" + msg.PlayerData.atk + ",防御力:" + msg.PlayerData.def);
                break;
            case 1002:
                Debug.Log("收到消息ID为1002的消息");
                break;
            default:
                Debug.Log("未知消息ID:" + msgID);
                break;
        }

        print("接收到服务端的数据:" + System.Text.Encoding.UTF8.GetString(bytes, 0, receiveNum)); //将字节数组转换为字符串
        //发送信息
        socket.Send(Encoding.UTF8.GetBytes("我是客户端"));
        //4.用Shutdown方法释放连接
        socket.Shutdown(SocketShutdown.Both);
        //5.关闭套接字
        socket.Close();
        #endregion

        #region 总结
        //1.客户端连接的流程每次都是相同的
        //2.客户端的 Connect、send、Receive是会阻塞主线程的 ,要等到执行完毕才会继续执行后面的内容
        //抛出问题:
        //如何让客户端的socket不影响主线程，并且可以随时收发消息?
        //我们会在之后的综合练习题讲解
        #endregion
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
