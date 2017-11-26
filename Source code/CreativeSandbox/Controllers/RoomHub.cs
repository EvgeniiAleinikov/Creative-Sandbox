using CreativeSandbox.Models;
using Microsoft.AspNet.SignalR;
using System;

namespace CreativeSandbox
{
    public class RoomHub : Hub
    {
        RoomContext db = new RoomContext();

        public void ConnectToRoom(int id)
        {
            Room room = db.Rooms.Find(id);
            room.UsersIn++;
            db.SaveChanges();

            Clients.Others.updateUsersInRoom(id, room.UsersIn);

            Groups.Add(Context.ConnectionId, id.ToString());

            SendCanvas(room.Content);
        }

        public void LeaveRoom(int id)
        {
            Room room = db.Rooms.Find(id);
            room.UsersIn--;
            db.SaveChanges();

            Clients.Others.updateUsersInRoom(room.Id, room.UsersIn);
        }

        public void Send(Data data, string id)
        {
            Clients.OthersInGroup(id).addLine(data);
        }

        private void SendCanvas(string canvasUrl)
        {
            Clients.Caller.loadImage(canvasUrl);
        }

        public void AddImage(string id, string imageUrl, string imageId)
        {
            Clients.OthersInGroup(id).addImage(imageUrl, imageId);
        }

        public void DragImage(string id, string imageId, string top, string left)
        {
            Clients.OthersInGroup(id).dragImage(imageId, top, left);
        }

        public void ResizeImage(string id, string imageId, string width, string height)
        {
            Clients.OthersInGroup(id).resizeImage(imageId, width, height);
        }

        public void RotateImage(string id, string imageId, string transform)
        {
            Clients.OthersInGroup(id).rotateImage(imageId, transform);
        }

        public void DeleteImage(string id, string imageId)
        {
            Clients.OthersInGroup(id).deleteImage(imageId);
        }

        public void ClearAll(string id)
        {
            db.Rooms.Find(Int32.Parse(id)).Content = null;
            db.SaveChanges();
            Clients.OthersInGroup(id).clearAll();
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}