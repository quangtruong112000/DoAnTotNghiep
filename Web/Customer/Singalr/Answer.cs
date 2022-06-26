using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AppBanThuoc.Singalr
{
    [HubName("sendanswer")]

    public class Answer : Hub
    {
        public void sendanswer(string message, string name, string image, string id)
        {
            Clients.All.sendanswer(message, name, image , id);
        }
    }
}