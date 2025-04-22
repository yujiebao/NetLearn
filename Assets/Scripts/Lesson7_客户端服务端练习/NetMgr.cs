using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using Unity.VisualScripting;
using UnityEngine;

public class NetMgr : MonoBehaviour
{
    private static NetMgr instance;
    public static NetMgr Instance =>instance;
    private byte[] cacheBytes = new byte[1024*1024]; //缓存字节数组 用于处理分包
    private int cacheNum = 0; //缓存字节数组的长度  分包问题
    // 客户端Socket
    private Socket socket;
    
    //用于发送消息的队列 公共容器  主线程往里面放 发送线程从里面取
    // private Queue<string> sendQueue = new Queue<string>();
    //用于接收消息的对象 公共容器 子线程往里面放 主线程从里面取
    // private Queue<string> receiveQueue = new Queue<string>();

    //改为发送和接收自定义的类
    private Queue<BaseMsg> sendQueue = new Queue<BaseMsg>();
    private Queue<BaseMsg> receiveQueue = new Queue<BaseMsg>();
    
    // private Thread sendThread; //发送线程
    // private Thread receiveThread; //接收线程   注释了 改用线程池实现
    //用于收消息的水桶(公共容器) 线程往里面放 主线程去读取
    // private byte[] receiveBytes = new byte[1024*1024]; //接收数据的字节数组
    //接收到的字节数
    // private int receiveCount;
    private bool isConnect = false;

    //发送心跳消息的时间间隔
    private int SEND_MESSAGE_TIME = 2;
    void Awake()
    {
        instance = this;
        DontDestroyOnLoad(this.gameObject); //不销毁这个对象
        //客户端循环定时给服务端发送心跳消息
        InvokeRepeating("SendHeartMsg",0,SEND_MESSAGE_TIME);
    }

    private void SendHeartMsg()
    {
        if(isConnect)
        Send(new HeartMsg());
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (receiveQueue.Count > 0)
        {
            //从水桶中取出数据
            BaseMsg msg = receiveQueue.Dequeue();
            if (msg is PlayerMsg) //如果是玩家消息
            {
                PlayerMsg playerMsg = msg as PlayerMsg; //将基类对象转换为子类对象
                Debug.Log("收到玩家消息:" + playerMsg.GetID()+"ID:"+playerMsg.playerID+"玩家姓名:"+playerMsg.PlayerData.name+",攻击力:"+playerMsg.PlayerData.atk+",防御力:"+playerMsg.PlayerData.def); //打印消息内容
            }
            else
            {
                Debug.Log("未知消息类型！");
            }
        }
    }

    //连接服务端
    public void ConnectServer(string ip,int port)
    {
        if(isConnect)
        {
            Debug.Log("已经连接过了！");
            return;
        }

        if (socket != null && socket.Connected)
        {
            Debug.Log("已经连接过了！");
            return;
        }
        socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        IPEndPoint ipPoint = new IPEndPoint(IPAddress.Parse(ip), port);
        try
        {
            socket.Connect(ipPoint);
            Debug.Log("连接成功！");
            isConnect = true;

            // sendThread = new Thread(sendMsg); //创建发送线程
            // sendThread.Start();
            // receiveThread = new Thread(ReceiveMsg); //创建接收线程
            // receiveThread.Start();

            //改为使用线程池
            ThreadPool.QueueUserWorkItem(sendMsg); //开启线程池  线程池会自动分配线程
            ThreadPool.QueueUserWorkItem(ReceiveMsg); 
        }
        catch (SocketException e)
        {
            if (e.ErrorCode == 10061)
            {
                Debug.Log("连接失败，服务端未开启！");
            }
            else
            {
                Debug.Log("连接失败，错误码:" + e.ErrorCode);
            }
        }
    }

    //发送消息
    public void Send(BaseMsg msg)
    {
        if(socket != null)
        {
            // socket.Send(Encoding.UTF8.GetBytes(info)); 
            sendQueue.Enqueue(msg);
        }
    }

    private void sendMsg(System.Object data) //发送消息
    {
        while (isConnect)
        {
            if (sendQueue.Count > 0)
            {
                socket.Send(sendQueue.Dequeue().Writing());
            }
        }
    }

    //不停地接收消息
    private void ReceiveMsg(System.Object data) //接收消息
    {
        while (isConnect)
        {
            if (socket.Available > 0)
            {
                byte[] receiveBytes = new byte[1024*1024];
                int receiveNum = socket.Receive(receiveBytes);
                HandleReceiveMsg(receiveBytes, receiveNum);
                // receiveCount = socket.Receive(receiveBytes); //接收数据
                // //首先把收到字节的前4个字节解析为int类型的消息ID
                // int msgID = System.BitConverter.ToInt32(receiveBytes, 0); //解析消息ID
                // BaseMsg baseMsg = null; //定义一个基类对象
                // switch (msgID)
                // {
                //     case 1:
                //         PlayerMsg msg = new PlayerMsg();
                //         msg.Reading(receiveBytes, 4); //从第5个字节开始读取数据
                //         baseMsg = msg; //将子类对象赋值给基类对象
                //         break;
                // }
                // if (baseMsg == null) //如果没有解析到消息ID
                // {
                //     Debug.Log("未知消息ID:" + msgID);
                //     continue; //跳过本次循环
                // }
                // //收到信息 解析为字符串 并放入水桶中 
                // receiveQueue.Enqueue(baseMsg); //将字节数组转换为字符串
            }
        }
    }

    private void HandleReceiveMsg(byte[] receiveBytes,int receiveNum) //处理收到的字节数组  加上处理分包，黏包相关逻辑
    {
        int msgID = 0;
        int msgLength = 0;    //主要用于处理分包  黏包问题不需要msgLength也能解决
        int nowIndex = 0;

        //收到消息时 先看看之前有没有缓存的 如果有的话 我们直接拼接到后面
        receiveBytes.CopyTo(cacheBytes, cacheNum); //将缓存的字节数组拼接到后面
        cacheNum += receiveNum; //更新缓存的字节数组长度

        while(true)
        {
            msgLength = -1;   //每次将长度设置为-1 是为了上一次的数据 不影响这一次的判断
            if(cacheNum - nowIndex > 8)
            {
                msgID = BitConverter.ToInt32(cacheBytes, nowIndex); //解析消息ID
                nowIndex += sizeof(int); //跳过消息ID的字节数 
                msgLength = BitConverter.ToInt32(cacheBytes, nowIndex); //解析消息体的长度
                nowIndex += sizeof(int); //跳过消息体长度的字节数}
            }

            if(cacheNum - nowIndex >= msgLength && msgLength != -1)   //刚好匹配或者黏包  
            {
                //解析消息体
                BaseMsg baseMsg = null; //定义一个基类对象
                switch (msgID)
                {
                    case 1:
                        PlayerMsg msg = new PlayerMsg();
                        msg.Reading(cacheBytes, nowIndex); //从第5个字节开始读取数据
                        baseMsg = msg; //将子类对象赋值给基类对象
                        break;
                }
                if (baseMsg == null) //如果没有解析到消息ID
                {
                    Debug.Log("未知消息ID:" + msgID);
                    continue; //跳过本次循环
                }
                //收到信息 解析为字符串 并放入水桶中 
                receiveQueue.Enqueue(baseMsg); //将字节数组转换为字符串
                nowIndex += msgLength;
                if(nowIndex == cacheNum)    //读取的长度(cache)和信息的长度()一致
                {
                    cacheNum = 0; //清空缓存的字节数组长度
                    break;
                }
            }
            else 
            {
                //如果不满足 证明有分包
                //那么我们需要把当前收到的内容 记录下来
                //有待下次接受到消息后 再做处理

                //如果进行了id和长度的解析 但是 没有成功解析消息体 那么我们需要减去nowIndex移动的位置(恢复到之前 保证存储后面完整的数据)
                if(msgLength != -1)  //没有解析到消息体
                {
                    nowIndex -=8;
                }
                //把剩余没有解析的字节数组内容 移动到前面 用于缓存下次继续解析
                Array.Copy(cacheBytes, nowIndex, cacheBytes, 0, cacheNum - nowIndex);
                cacheNum = cacheNum - nowIndex;   //也是标记了有用的位数
                break;                
            }
        }
    }


    public void Close()
    {
        if (socket != null)
        {
            print("客户端主动断开连接");

            // //自动发送一条断开的消息给服务器
            // QuitMsg quitMsg = new QuitMsg();
            // // socket.Send(quitMsg.Writing());
            // socket.Send(quitMsg.Writing());
            // socket.Shutdown(SocketShutdown.Both);
            // socket.Disconnect(false);  //写在Close前面
            // socket.Close();
            // socket = null;
            // isConnect = false;
            // sendThread = null;
            // receiveThread = null;
        }
    }

    private void OnDestroy() {
        System.Console.WriteLine("对象销毁");
        Close();
    }

    ///<summary>
    ///用于测试 直接发字节数组的方法
    ///</summary>
    public void SendTest(byte[] bytes)
    {
        socket.Send(bytes);
    }
}
