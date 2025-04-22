using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Test : MonoBehaviour
{
    public Button button;
    public Button button1;
    public Button button2;
    public Button button3;

    public TMP_InputField inputField;
    
    // Start is called before the first frame update
    void Start()
    {
        
        if (NetMgr.Instance == null)
        {
            GameObject obj = new GameObject("NetMgr");
            obj.AddComponent<NetMgr>();
        }
        NetMgr.Instance.ConnectServer("127.0.0.1", 8080);
        // NetMgr.Instance.Send("我是客户端");
        // StartCoroutine(print());
        button.onClick.AddListener(() =>
        {
            string msg = inputField.text;
            if(msg != string.Empty)
            {
                // NetMgr.Instance.Send(msg);
                PlayerMsg playmsg = new PlayerMsg();
                playmsg.playerID = 1;
                playmsg.PlayerData = new PlayerData();
                playmsg.PlayerData.name = "我的客户端";
                playmsg.PlayerData.atk = 100;
                playmsg.PlayerData.def = 100;
                NetMgr.Instance.Send(playmsg); //发送消息
            }
            inputField.text = string.Empty;
        });

        //黏包
        button1.onClick.AddListener(() =>
        {
            print("黏包");
            PlayerMsg playmsg = new PlayerMsg();
            playmsg.playerID = 1;
            playmsg.PlayerData = new PlayerData();
            playmsg.PlayerData.name = "我的客户端";
            playmsg.PlayerData.atk = 100;
            playmsg.PlayerData.def = 100;

            PlayerMsg playmsg2 = new PlayerMsg();
            playmsg2.playerID = 2;
            playmsg2.PlayerData = new PlayerData();
            playmsg2.PlayerData.name = "我的客户端2";
            playmsg2.PlayerData.atk = 200;
            playmsg2.PlayerData.def = 500;

            byte[] bytes = new byte[playmsg.GetBytesNum() + playmsg2.GetBytesNum()];
            playmsg.Writing().CopyTo(bytes, 0);
            playmsg2.Writing().CopyTo(bytes, playmsg.GetBytesNum());
            NetMgr.Instance.SendTest(bytes);
        });

        //分包
        button2.onClick.AddListener(async() =>
        {
            print("分包");
            PlayerMsg playmsg = new PlayerMsg();
            playmsg.playerID = 1;
            playmsg.PlayerData = new PlayerData();
            playmsg.PlayerData.name = "我的客户端";
            playmsg.PlayerData.atk = 100;
            playmsg.PlayerData.def = 100;
            
            byte[] bytes = playmsg.Writing();
            byte[] bytes1 = new byte[10];
            byte[] bytes2 = new byte[bytes.Length - 10];
            Array.Copy(bytes, 0, bytes1, 0, 10);
            Array.Copy(bytes, 10, bytes2, 0, bytes.Length - 10);
            NetMgr.Instance.SendTest(bytes1);
            await Task.Delay(1000);
            NetMgr.Instance.SendTest(bytes2);

        });

        //黏包分包
        button3.onClick.AddListener(async() =>
        {
            // print("分包加黏包");
            // PlayerMsg playmsg = new PlayerMsg();
            // playmsg.playerID = 1;
            // playmsg.PlayerData = new PlayerData();
            // playmsg.PlayerData.name = "我的客户端";
            // playmsg.PlayerData.atk = 100;
            // playmsg.PlayerData.def = 100;

            // PlayerMsg playmsg2 = new PlayerMsg();
            // playmsg2.playerID = 2;
            // playmsg2.PlayerData = new PlayerData();
            // playmsg2.PlayerData.name = "我的客户端2";
            // playmsg2.PlayerData.atk = 200;
            // playmsg2.PlayerData.def = 500;

            // byte[] bytes = new byte[playmsg.GetBytesNum() + playmsg2.GetBytesNum()];
            // playmsg.Writing().CopyTo(bytes, 0);
            // playmsg2.Writing().CopyTo(bytes, playmsg.GetBytesNum());
            // byte[] bytes1 = new byte[playmsg.GetBytesNum()+10];
            // byte[] bytes2 = new byte[bytes.Length -10 - playmsg.GetBytesNum()];
            // Array.Copy(bytes, 0, bytes1, 0, playmsg.GetBytesNum()+10);
            // Array.Copy(bytes, playmsg.GetBytesNum()+10, bytes2, 0, bytes.Length -10 - playmsg.GetBytesNum());
            // NetMgr.Instance.SendTest(bytes1);
            // await Task.Delay(1000);
            // NetMgr.Instance.SendTest(bytes2);
            QuitMsg quitmsg = new QuitMsg();
            byte[] bytes = quitmsg.Writing();
            NetMgr.Instance.SendTest(bytes);

        });
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator print()
    {
        for(int i = 0; i < 10; i++)
        {
            // NetMgr.Instance.Send("我是客户端" + i);
            yield return new WaitForSeconds(1f);
        }
    }

    void OnDestroy()
    {
      
    }
}
