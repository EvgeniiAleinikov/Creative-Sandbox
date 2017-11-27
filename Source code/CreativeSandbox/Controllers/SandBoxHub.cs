using CreativeSandbox.Models;
using Microsoft.AspNet.SignalR;

namespace CreativeSandbox.Controllers
{
    public class SandBoxHub : Hub
    {
        private readonly RoomContext _db = new RoomContext();

        public void AddRoom()
        {
            var newRoom = new Room() { Name = "Work room", UsersIn = 0 };
            _db.Rooms.Add(newRoom);
            _db.SaveChanges();
            SendNewRoom(newRoom);
        }

        public void SendNewRoom(Room room)
        {
            Clients.All.receiveRoom(room.Id, room.Name, room.UsersIn);
        }

        public void ChangeRoomName(int id, string name)
        {
            _db.Rooms.Find(id).Name = name;
            _db.SaveChanges();
            Clients.Others.changeRoomName(id, name);
        }

        public void DeleteRoom(int id)
        {
            Room room = _db.Rooms.Find(id);

            if (room.UsersIn == 0)
            {
                _db.Rooms.Remove(room);
                _db.SaveChanges();
                
                Clients.All.deleteRoom(id);
            }
            else
                Clients.Caller.erorDeleteRoom(id);
        }

        protected override void Dispose(bool disposing)
        {
            _db.Dispose();
            base.Dispose(disposing);
        }
    }
}