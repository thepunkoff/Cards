using System;

namespace Cards.Domain.Models
{
    public class User
    {
        public User(Guid id, string username)
        {
            Id = id;
            Username = username;
        }
        
        public Guid Id { get; set; }
        public string Username { get; set; }
    }
}