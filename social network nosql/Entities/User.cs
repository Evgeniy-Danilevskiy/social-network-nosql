using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sns_nrdb
{
    public class User
    {
        [BsonId]
        public ObjectId idmdb { get; set; }

        [BsonElement("id")]
        public int UserId { get; set; }

        [BsonElement("name")]
        public string Name { get; set; }

        [BsonElement("surname")]
        public string Surname { get; set; }

        [BsonElement("age")]
        public int Age { get; set; }

        [BsonElement("email")]
        public string Email { get; set; }

        [BsonElement("password")]
        public string Password { get; set; }

        [BsonElement("interests")]
        public List<string> Interests { get; set; }

        [BsonElement("friends")]
        public List<int> Friends { get; set; }

        [BsonElement("subscriptions")]
        public List<int> Subscriptions { get; set; }
    }
}
