using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AppBanThuoc.Singalr
{
    [HubName("sendpost")]
   
    public class Post : Hub
    {
        
        public void sendposts(string message,string name ,string image , string id)
        {
            Clients.All.sendpost( message,  name,  image ,id);
        }

        

    }
}