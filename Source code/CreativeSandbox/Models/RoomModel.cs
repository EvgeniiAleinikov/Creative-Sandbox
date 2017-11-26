using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace CreativeSandbox.Models
{
    public class Room
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Content { get; set; }
        public int UsersIn { get; set; }
    }

    public class RoomContext : DbContext
    {
        public RoomContext() : base("DefaultConnection")
        { }
        public DbSet<Room> Rooms { get; set; }

    }
}