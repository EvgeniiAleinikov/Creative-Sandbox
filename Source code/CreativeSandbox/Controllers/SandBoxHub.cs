using CreativeSandbox.Models;
using Microsoft.AspNet.SignalR;
using System.Data.Entity;
using System.Diagnostics;
using System.Threading.Tasks;

namespace CreativeSandbox
{
    public class SandBoxHub : Hub
    {
        RoomContext db = new RoomContext();

        public void AddRoom()
        {
            var newRoom = new Room() { Name = "Work room", UsersIn = 0 };
            db.Rooms.Add(newRoom);
            db.SaveChanges();
            SendNewRoom(newRoom);
        }

        public void SendNewRoom(Room room)
        {
            Clients.All.receiveRoom(room.Id, room.Name, room.UsersIn);
        }

        public void ChangeRoomName(int id, string name)
        {
            db.Rooms.Find(id).Name = name;
            db.SaveChanges();
            Clients.Others.changeRoomName(id, name);
        }

        public void DeleteRoom(int id)
        {
            Room room = db.Rooms.Find(id);

            if (room.UsersIn == 0)
            {
                db.Rooms.Remove(room);
                db.SaveChanges();
                
                Clients.All.deleteRoom(id);
            }
            else
                Clients.Caller.erorDeleteRoom(id);
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}