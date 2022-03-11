
using SignalRLibrary;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SignalRServ
{
    public interface IHubMethods
    {
        Task Send(Message message);

        Task UpdateRooms();

        Task SendAudio(string name, byte[] message);
    }
}
