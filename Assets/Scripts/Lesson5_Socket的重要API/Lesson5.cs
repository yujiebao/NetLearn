using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using UnityEngine;

public class Lesson5 : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        #region 知识点一 Socket套接字的使用
        //它是C#提供给我们用于网络通信的一个类(在其它语言当中也有对应的socket类)
        //类名:Socket
        //命名空间:System.Net.Sockets
        
        //Socket套接字是支持TCP/IP网络通信的基本操作单位
        //一个套接字对象包含以下关键信息!
        //1.本机的IP地址和端口
        //2.对方主机的IP地址和端口
        //3.双方通信的协议信息

        //一个socket对象表示一个本地或者远程套接字信息
        //它可以被视为一个数据通道
        //这个通道连接与客户端和服务端之间
        //数据的发送和接受均通过这个通道进行

        //一般在制作长连接游戏时，我们会使用socket套接字作为我们的通信方案
        //我们通过它连接客户端和服务端，通过它来收发消息
        //你可以把它抽象的想象成一根管子，插在客户端和服务端应用程序上，通过这个管子来传递交换信息
        #endregion

        #region 知识点二 Socket的类型
        //Socket套接字有3种不同的类型
        //1.流套接字
        //主要用于实现TCP通信，提供了面向连接、可靠的、有序的、数据无差错且无重复的数据传输服务
        //2.数据报套接字
        // 主要用于实现UDP通信，提供了无连接的通信服务，数据包的长度不能大于32KB，不提供正确性检查，不保证顺序，可能出现重发、丢失等情况
        //3.原始套接字(不常用，不深入讲解)
        // 主要用于实现IP数据包通信，用于直接访问协议的较低层，常用于侦听和分析数据包

        //通过Socket的构造函数 我们可以申明不同类型的套接字
        // Socket s = new Socket(AddressFamily.InterNetwork,SocketType.Stream, ProtocolType.Tcp);
        //参数一:AddressFamily 网络寻址(枚举类型)，决定寻址方案
        //常用:
        //1.InterNetwork IPv4寻址
        //2.InterNetwork6 IPv6寻址
        //3.Unix  Unix域套接字寻址   等等  --不常用
        //参数二:SocketType 套接字类型(枚举类型)，决定套接字的类型
        //常用:
        //1.Dgram       最大长度固定的无连接、不可靠的消息(主要用于UDP通信)
        //2 .Stream     支持数据报支持可靠、双向、基于连接的字节流(主要用于TCP通信)    
        //做了解:
        // 1.Raw
        //2.Rdm
        //3.Seqpacket
        //参数三:ProtocolType 协议类型(枚举类型)，决定套接字使用的协议
        //常用:
        //1.TCP     TCP传输控制协议
        //2.UDP     UDP用户数据报协议
        //其他做了解

        //3参数的常用搭配:
        // SocketType.Dgram + ProtocolType.Udp  = UDP协议通信 (常用，主要学习)
        // SocketType.Stream + ProtocolType.Tcp = TCP协议通信 (常用，主要学习)
        // SocketType.Raw + ProtocolType.Icmp  = Internet控制报文协议(做了解)
        // SocketType.Raw + ProtocolType.Raw  = 简单的IP包通信(做了解)

        //我们必须掌握的
        //TCP流套接字
        Socket sT = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

        //UDP数据报套接字
        Socket sU = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
        #endregion

        #region 知识点三 Socket的常用属性
        //1.套接字的连接状态
        print(sT.Connected);
        //2.获取套接字的类型
        print(sT.SocketType);
        //3.获取套接字的协议类型
        print(sT.ProtocolType);
        //4.获取套接字的寻址方案
        print(sT.AddressFamily);
        //5.从网络中获取准备读取的数据数据量
        print(sT.Available);
        //6.获取本机EndPoint对象(注意:IPEndPoint继承EndPoint)
        print(sT.LocalEndPoint as IPEndPoint);
        //7.获取远程EndPoint对象
        print(sT.RemoteEndPoint as IPEndPoint);
        #endregion

        #region 知识点四 Socket的常用方法
        //1.主要用于服务器
        // 1-1:绑定IP和端口号
        IPEndPoint point = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 8080);
        sT.Bind(point); //绑定IP和端口号

        // 1-2:设置客户端连接的最大数量
        sT.Listen(10);  //  最多10个客户端连接
        // 1-3:等待客户端连入
        sT.Accept(); //阻塞式等待客户端连接  存在异步方法

        //2.主要用于客户端
        //1-1:连接远程服务端
        sT.Connect(point);  //传入ip和端口
        
        //3.客户端服务端都会用的
        //1-1:同步发送和接收数据
        sT.Send(new byte[1024]);   //发送数据
        sT.Receive(new byte[1024]);  //接收数据
        //1-2:异步发送和接收数据
        //1-3:释放连接并关闭socket，先与close调用
        sT.Shutdown(SocketShutdown.Both); //关闭连接，释放所有socket关联资源
        //1-4:关闭连接，释放所有socket关联资源
        sT.Close();
        #endregion
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
