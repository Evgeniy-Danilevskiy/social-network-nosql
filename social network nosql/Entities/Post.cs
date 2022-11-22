using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sns_nrdb
{
    public class Post
    {
        [BsonId]
        public ObjectId idmdb { get; set; }
        public string IdString { get { return idmdb.ToString(); } }
        
        [BsonElement("userId")]
        public int UserId { get; set; }

        [BsonElement("body")]
        public string PostBody { get; set; }

        [BsonElement("date")]
        public DateTime CreationDate { get; set; }

        [BsonElement("comments")]
        public List<Comment> Comments { get; set; }

        [BsonElement("likes")]
        public List<int> Likes { get; set; }

        //public override string ToString()
        //{
        //    return $"\n{UserName} ({CreationDate.ToString("G")})\t{Likes.Count} likes\t {Comments.Count} comments" +
        //        $"\n\n{PostBody}\n\n";
        //}
    }
}
