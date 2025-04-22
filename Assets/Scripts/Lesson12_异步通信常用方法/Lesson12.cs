using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

public class Lesson12 : MonoBehaviour
{
    private byte[] resultBytes = new byte[1024];
    // Start is called before the first frame update
    void Start()
    {
        #region 知识点一 异步方法和同步方法的区别
        //同步方法:
        //方法中逻辑执行完毕后，再继续执行后面的方法
        //异步方法:
        //方法中逻辑可能还没有执行完毕，就继续执行后面的内容

        //异步方法的本质
        //往往异步方法当中都会使用多线程执行某部分逻辑
        //因为我们不需要等待方法中逻辑执行完毕就可以继续执行下面的逻辑了
        //注意:unity中的协同程序中的某些异步方法，有的使用的是多线程有的使用的是迭代器分步执行
        //关于协同程序可以回顾Unity基础当中讲解协同程序原理的知识点
        #endregion

        #region 知识点二 举例说明异步方法原理
        //我们以一个异步倒计时方法举例
        //1.线程回调
        // CountDownAsync(20,()=>{ print("倒计时结束");});

        //2.async和await 会等待线程执行完毕继续执行后面的逻辑
        CountDownAsync2(20,()=>{ print("倒计时结束");});
        
        //相对第一种方式 可以让函数分步执行
        #endregion

        #region 知识点三 Socket TCP通信中的异步方法(Begin开头方法)
        //回调函数参数IAsyncResult
        //Asyncstate 调用异步方法时传入的参数 需要转换
        //AsyncWaitHandle 用于同步等待

        Socket socketTcp = new Socket(AddressFamily.InterNetwork,SocketType.Stream,ProtocolType.Tcp);
        //服务器相关
        //BeginAccept
        socketTcp.BeginAccept(AcceptCallBack,socketTcp);
        //EndAccept

        //客户端相关
        //BeginConnect
        IPEndPoint ipPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"),8080);
        socketTcp.BeginConnect(ipPoint,
        (result) => {
            try
            {
                socketTcp.EndConnect(result);
            }
            catch(SocketException e)
            {
                Debug.Log(e.SocketErrorCode);   
            }
        }
        ,socketTcp);
        //EndConnect

        //客户端服务器通用
        //接收消息 
        //BeginReceive
        //EndReceive
        socketTcp.BeginReceive(resultBytes,0/*写到resultBytes的开始位置*/,resultBytes.Length,SocketFlags.None,ReceiveCallBack,socketTcp);

        //发送消息
        //BeginSend
        //EndSend
        byte[] bytes = Encoding.UTF8.GetBytes("hello");
        socketTcp.BeginSend(bytes,0,bytes.Length,SocketFlags.None,
        (result) => {
            try
            {
                socketTcp.EndSend(result);
                Debug.Log("发送成功");
            }
            catch(SocketException e)
            {
                Debug.Log(e.SocketErrorCode);   
            }
        },socketTcp);
        #endregion
    }

    private void AcceptCallBack(IAsyncResult result)
    {
        try
        {
            //获取传入的参数
            Socket s = result.AsyncState as Socket;    //获取socketTcp  传入的第二个参数
            //通过调用EndAccept方法可以得到连入的客户端Socket
            Socket clientSocket = s.EndAccept(result);

            //连入后需要再次调用BeginAccept方法  继续监听  不是递归   客户端有多个连入
            s.BeginAccept(AcceptCallBack,s);
        }
        catch(SocketException e)
        {
            Debug.Log(e.SocketErrorCode);   //具体的连接错误
        }
    }

    private void ReceiveCallBack(IAsyncResult result)
    {
        try
            {
                Socket s = result.AsyncState as Socket;               //AsyncState指的还是最后传入的object  
                //返回值是接收到了多少个字节
                int num = s.EndReceive(result);
                //进行消息处理
                Encoding.UTF8.GetString(resultBytes,0,num);

                s.BeginReceive(resultBytes,0,resultBytes.Length,SocketFlags.None,ReceiveCallBack,s);   //s存储为object 使用时再拆箱
            }
            catch(SocketException e)
            {
                Debug.Log(e.SocketErrorCode);   
            }
    }
    // Update is called once per frame
    void Update()
    {
        
    }

    public void CountDownAsync(int second,UnityAction callBack)
    {
        Thread t = new Thread(() =>{
            while(true)
            {
                Debug.Log(second);
                Thread.Sleep(1000);
                second--;
                if(second<=0)
                {
                    break;
                }
            }
            callBack.Invoke();
        });

        t.Start();
        
        print("开始倒计时");
    }

    public async void CountDownAsync2(int second,UnityAction callBack)
    {
        // while(true)
        // {
        //     Debug.Log(second);
        //     await Task.Delay(1000);
        //     second--;
        //     if(second<=0)
        //     {
        //         break;
        //     }
        // }
        // callBack.Invoke();

        print("开始倒计时");

        await Task.Run(()=>{             //等待await执行结束才执行后面的代码
            while(true)
            {
                Debug.Log(second);
                Thread.Sleep(1000);
                second--;
                if(second<=0)
                {
                    break;
                }
            }
            callBack.Invoke();
        });

        print("倒计时结束");
    }
}
