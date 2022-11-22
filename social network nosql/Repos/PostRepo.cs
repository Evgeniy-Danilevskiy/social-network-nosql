using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;

namespace sns_nrdb.Data.Repositories
{
    public class PostsRepository
    {
        private readonly IMongoCollection<Post> posts;

        public PostsRepository(IMongoDatabase db)
        {
            posts = db.GetCollection<Post>("posts");
        }

        public IEnumerable<Post> GetAllUserPosts()
        {
            return posts.Find(Builders<Post>.Filter.Empty).ToList();
        }
        public IEnumerable<Post> GetLatest()
        {
            return posts.Find(Builders<Post>.Filter.Empty).SortByDescending(p => p.CreationDate).Limit(5).ToList();
        }

        public Post GetPostById(string postId)
        {
            return posts.Find(Builders<Post>.Filter.Eq(p => p.idmdb, new ObjectId(postId))).FirstOrDefault();

        }

        public void React(int userId, string postId)
        {
            var post = this.GetPostById(postId);

            if (post.Likes.Contains(userId))
            {
                post.Likes.Remove(userId);
                posts.UpdateOne(Builders<Post>.Filter.Eq(p => p.idmdb, new ObjectId(postId)), Builders<Post>.Update.Pull(p => p.Likes, userId));
            }
            else
            {
                post.Likes.Add(userId);
                posts.UpdateOne(Builders<Post>.Filter.Eq(p => p.idmdb, new ObjectId(postId)), Builders<Post>.Update.Push(p => p.Likes, userId));
            }

        }
        public IEnumerable<Post> GetUserPosts(int userId)
        {
            return posts.Find(Builders<Post>.Filter.Eq(p => p.UserId, userId)).SortByDescending(p => p.CreationDate).ToList();
        }

        public void Create(int userId, string body)
        {
            posts.InsertOne(new Post
            {
                CreationDate = DateTime.Now,
                PostBody = body,
                UserId = userId,
                Comments = new List<Comment>(),
                Likes = new List<int>()
            });
        }

        public void CreateComment(int userId, string postId, string body)
        {
            posts.UpdateOne(Builders<Post>.Filter.Eq(p => p.idmdb, new ObjectId(postId)), Builders<Post>.Update.Push(p => p.Comments, new Comment
            {
                CommentBody = body,
                UserId = userId
            }));
        }
    }
}