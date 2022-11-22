using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sns_nrdb
{
    public class Comment
    {
        [BsonElement("body")]
        public string CommentBody { get; set; }
        
        [BsonElement("userId")]
        public int UserId { get; set; }

        //public override string ToString()
        //{
        //    return $"{UserName} ({CreationDate.ToString("G")}):\n{CommentBody}";
        //}
    }
}
