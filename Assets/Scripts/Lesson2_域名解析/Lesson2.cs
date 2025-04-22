using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using UnityEngine;

public class Lesson2 : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        #region 知识点一 什么是域名解析
        //域名解析也叫域名指向、服务器设置、域名配置以及反向IP登记等等
        //说得简单点就是将好记的域名解析成IP
        //IP地址是网络上标识站点的数字地址，但是IP地址相对来说记忆困难
        //所以为了方便记忆，采用域，代替IP地址标识站点地址。
        //比如 我们要登录一个网页 ww.baidu.com, 这个就是域名 我们可以通过记忆域名来记忆一个远端服务器的地址，而不是记录一个复杂的IP地址

        //域名解析就是域名到IP地址的转换过程。域名的解析工作由DNS服务器完成
        //我们在进行通信时有时会有需求通过域名获取IP
        //所以这节课我们就来学习C#提供的域名解析相关的类
        
        //域名系统(英文:Domain Name system，缩写:DNS)是互联网的一项服务
        //它作为将域名和IP地址相互映射的一个分布式数据库，能够使人更方便地访问互联网
        //是因特网上解决网上机器命名的一种系统，因为IP地址记忆不方便，就采用了域名系统来管理名字和IP的对应关系
        #endregion

        #region 知识点二 IPHostEntry类 
        // 命名空间:System.Net
        // 类名:IPHostEntry
        // 主要作用:域名解析后的返回值 可以通过该对象获取IP地址、主机名等等信息
        // 该类不会自己声明，都是作为某些方法的返回值返回信息，我们主要通过该类对象获取返回的信息
  
        // 获取关联IP          成员变量:AddressList   一个域名可能会对应多个IP地址
        // 获取主机别名列表     成员变量:Aliases         一个域名可能对应多个别名
        // 获取DNS名称         成员变量:HostName      该域名对应的DNS名称
        #endregion
    
        #region 知识点三 Dns类
        // 命名空间:System.Net
        // 类名:Dns
        // 主要作用:Dns是一个静态类  提供了很多静态方法，可以使用它来根据域名获取IP地址

        //常用方法
        //1.获取本地系统的主机名
        print(Dns.GetHostName());

        //2.获取指定域名的IP信息
        //根据域名获取
        //同步获取
        //注意:由于获取远程主机信息是需要进行网路通信，所以可能会阻塞主线程
        // IPHostEntry entry =  Dns.GetHostEntry("www.baidu.com");
        // for(int i = 0; i < entry.AddressList.Length; i++)
        // {
        //     print("IP地址:" + entry.AddressList[i]);
        // }
        // for(int i = 0; i < entry.Aliases.Length; i++)
        // {
        //     print("别名:" + entry.Aliases[i]);
        // }
        // print("DNS名称:" + entry.HostName);

        //异步获取
        GetHostEntry();
        #endregion
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private async void GetHostEntry()
    {
        Task<IPHostEntry> task = Dns.GetHostEntryAsync("www.baidu.com");
        await task;

        for(int i = 0; i < task.Result.AddressList.Length; i++)
        {
            print("IP地址:" + task.Result.AddressList[i]);
        }
        for(int i = 0; i < task.Result.Aliases.Length; i++)
        {
            print("别名:" + task.Result.Aliases[i]);
        }
        print("DNS名称:" + task.Result.HostName);
    }
}
