using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;

public class Try : MonoBehaviour
{
    private Socket socket;
    // Start is called before the first frame update
    void Start()
    {
        socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        try
        {
            socket.Connect(new IPEndPoint(IPAddress.Parse("127.0.0.1"), 8080));
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

        //接受数据  开多线程  要再while中检测
        ThreadPool.QueueUserWorkItem(SendData);
        ThreadPool.QueueUserWorkItem(ReceiveData); //开启线程池  线程池会自动分配线程
    }
    void ReceiveData(System.Object data)  //接受数据
    {
            while (true)
            {
                byte[] bytes = new byte[1024];
                int receiveNum = socket.Receive(bytes);
                print("接收到服务端的数据:" + System.Text.Encoding.UTF8.GetString(bytes, 0, receiveNum)); //将字节数组转换为字符串
            }
    }

    void SendData(System.Object data)  //发送数据
    {
        while (true)
        {
            socket.Send(Encoding.UTF8.GetBytes(data as string));
        }
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
