using System;
using System.Collections.Generic;
using PokeIn;
using PokeIn.Comet;

namespace ChatSample
{
    //Our custom parameter class
    [Serializable]
    public class ChatMessage
    {
        public string Username, Message;

        //You should define an Empty Consturctor to allow PokeIn to de-serialize
        public ChatMessage()
        {
            Username = "";
            Message="";
        }

        public ChatMessage(string userName, string message)
        {
            Username = userName;
            Message = message;
        }
    }

    //Chat Class
    public class ChatApp:IDisposable
    {
        public static Dictionary<string, string> Users = new Dictionary<string,string>();//clientId, username
        public static Dictionary<string, string> Names = new Dictionary<string,string>();//username, clientId
        string _clientId;
        string _username;
        public ChatApp(string clientId)
        {
            _clientId = clientId;
            _username = "";
        }
        public void Dispose() 
        {
            lock(Names)
            {
                if(Names.ContainsKey(_username))
                    if(Names[_username]==_clientId)
                    {
                        Names.Remove(_username);
                        Users.Remove(_clientId);
                        Send(new ChatMessage(_username, "Disconnected"));
                    }
            } 
        } 
        public void SetName(string userName)
        {
            if (_username != "")
            {
                CometWorker.SendToClient(_clientId, "alert('You already have a username!');btnChat.disabled = '';");
                return;
            }

            lock (Names)
            {
                if(Names.ContainsKey(userName))
                {
                    Names[userName] = _clientId;
                }
                else
                {
                    Names.Add(userName, _clientId);
                    Users.Add(_clientId, userName);
                }
            }
            _username = userName;

            //Create JSON Method
            string json = JSON.Method("UsernameSet", userName);//UsernameSet('UserName');
            CometWorker.SendToClient(_clientId, json); 

        }

        //When PokeIn sees ChatMessage custom class as a parameter, it automaticly defines ChatMessage JS class on client side.
        public void Send(ChatMessage message) 
        {
            //Create JSON method from custom class
            string json = JSON.Method("ChatMessageFrom", message); //ChatMessageFrom( {Username:'username', Message:'message' } );
            CometWorker.SendToAll(json);
        }
    }
}
