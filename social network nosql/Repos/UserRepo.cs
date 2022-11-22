using MongoDB.Driver;
using System.Collections.Generic;
using System.Linq;

namespace sns_nrdb.Data.Repositories
{
    public class UsersRepository
    {

        private readonly IMongoCollection<User> users;

        public UsersRepository(IMongoDatabase db)
        {
            users = db.GetCollection<User>("users");
        }

        public void Add(User newUser)
        {
            var user = users.Find(Builders<User>.Filter.Empty).SortByDescending(u => u.UserId).Limit(1).FirstOrDefault();
            if (user == null)
            {
                user.UserId = 1;
            }
            var newId = user.UserId + 1;
            newUser.UserId = newId;
            users.InsertOne(newUser);
        }

        public void Delete(int userId)
        {
            users.DeleteOne(p => p.UserId == userId);
        }

        public List<User> GetAll()
        {
            return users.Find(Builders<User>.Filter.Empty).ToList();
        }

        public IEnumerable<User> Get(string name, string surname)
        {
            var filter = Builders<User>.Filter.Empty;

            if (!string.IsNullOrEmpty(surname))
            {
                filter &= Builders<User>.Filter.Eq(u => u.Surname, surname);
            }

            if (!string.IsNullOrEmpty(name))
            {
                filter &= Builders<User>.Filter.Eq(u => u.Name, name);
            }

            return users.Find(filter).ToList();
        }

        public User GetByCredentials(string email, string password)
        {
            return users.Find(u => u.Email == email && u.Password == password).FirstOrDefault();
        }

        public List<User> GetUserFollows(int userId)
        {
            var user = users.Find(Builders<User>.Filter.Eq(u => u.UserId, userId)).FirstOrDefault();
            var subs = users.Find(Builders<User>.Filter.In(u => u.UserId, user.Subscriptions));

            return subs.ToList();
        }

        public void Subscribe(int userId, int potentialFriendId)
        {
            var user = users.Find(Builders<User>.Filter.Eq(u => u.UserId, userId)).FirstOrDefault();
            var potentialFriend = users.Find(Builders<User>.Filter.Eq(u => u.UserId, potentialFriendId)).FirstOrDefault();
            if (!user.Subscriptions.Contains(potentialFriendId))
            {
                users.UpdateOne(Builders<User>.Filter.Eq(u => u.UserId, userId), Builders<User>.Update.Push(u => u.Subscriptions, potentialFriendId));
            }

            if (!user.Friends.Contains(potentialFriendId) && potentialFriend.Subscriptions.Contains(userId))
            {
                users.UpdateOne(Builders<User>.Filter.Eq(u => u.UserId, potentialFriendId), Builders<User>.Update.Push(u => u.Friends, userId));
                users.UpdateOne(Builders<User>.Filter.Eq(u => u.UserId, userId), Builders<User>.Update.Push(u => u.Friends, potentialFriendId));
            }

        }

        public void Unsubscribe(int userId, int anotherUId)
        {
            var user = users.Find(Builders<User>.Filter.Eq(u => u.UserId, userId)).FirstOrDefault();
            var anotherUser = users.Find(Builders<User>.Filter.Eq(u => u.UserId, anotherUId)).FirstOrDefault();
            if (user.Subscriptions.Contains(anotherUId))
            {
                users.UpdateOne(Builders<User>.Filter.Eq(u => u.UserId, userId), Builders<User>.Update.Pull(u => u.Subscriptions, anotherUId));
            }

            if (user.Friends.Contains(anotherUId))
            {
                users.UpdateOne(Builders<User>.Filter.Eq(u => u.UserId, userId), Builders<User>.Update.Pull(u => u.Friends, anotherUId));
                users.UpdateOne(Builders<User>.Filter.Eq(u => u.UserId, anotherUId), Builders<User>.Update.Pull(u => u.Friends, userId));
            }
        }
        public IEnumerable<User> GetUserFriends(int userId)
        {
            var user = users.Find(Builders<User>.Filter.Eq(u => u.UserId, userId)).FirstOrDefault();
            var follows = users.Find(Builders<User>.Filter.In(u => u.UserId, user.Friends));

            return follows.ToList();
        }

    }
}