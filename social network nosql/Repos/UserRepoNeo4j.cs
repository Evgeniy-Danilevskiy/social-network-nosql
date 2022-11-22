using Neo4jClient;
using sns_nrdb.Entities;
using System.Linq;

namespace sns_nrdb
{
    public class UserRepoNeo4j
    {
        private readonly BoltGraphClient _client;

        public UserRepoNeo4j(BoltGraphClient client)
        {
            _client = client;
        }

        public UserNeo4j GetUser(string email, string password)
        {
            var user = _client.Cypher
                .Match("(u:UserNeo4j {email: $ue})")
                .WithParam("ue", email)
                .Where("u.password= $password")
                .WithParam("password", password)
                .Return(u => u.As<UserNeo4j>())
                .ResultsAsync.Result;

            return user.ElementAt(0);
        }

        public void Add(UserNeo4j newUser)
        {
            _client.Cypher
                .Create("(u:UserNeo4j $newUser)")
                .WithParam("newUser", newUser)
                .ExecuteWithoutResultsAsync().Wait();

        }
        public void DeleteUser(int userId)
        {
            _client.Cypher
                .Match("(u:UserNeo4j {id: $deleteUser})")
                .WithParam("deleteUser", userId)
                .DetachDelete("u")
                .ExecuteWithoutResultsAsync().Wait();

        }
        public void CreateRelationshipUserFollows(int userId, int followsId)
        {
            _client.Cypher
                .Match("(u:UserNeo4j{id:$uid})", "(f:UserNeo4j{id: $fid})")
                .WithParam("uid", userId)
                .WithParam("fid", followsId)
                .Create("(u)-[:Subscription]->(f)")
                .ExecuteWithoutResultsAsync().Wait();
        }


        public void DeleteRelationshipUserFollower(int userId, int followsId)
        {
            _client.Cypher
                .Match("(u:UserNeo4j{id: $uid})-[r:Subscription]-(f:UserNeo4j{id: $fid})")
                .WithParam("uid", userId)
                .WithParam("fid", followsId)
                .Delete("r")
                .ExecuteWithoutResultsAsync().Wait();
        }

        public bool AreFriends(int userId, int searchedUserId)
        {
            var areFriends = _client.Cypher
                .Match("(u:UserNeo4j {id: $uid})-[r:Subscription]-(s:UserNeo4j {id: $sid})")
                .WithParam("uid", userId)
                .WithParam("sid", searchedUserId)
                .Return((u, s) => new
                {
                    Follow = s.As<UserNeo4j>()
                })
                .ResultsAsync.Result
                .Count() == 2;

            return areFriends;
        }

        public bool IfFollows(int userId, int searchedUserId)
        {
            var areFriends = _client.Cypher
                .Match("(u:UserNeo4j {id: $uid})-[r:Subscription]->(s: UserNeo4j {id: $sid})")
                .WithParam("uid", userId)
                .WithParam("sid", searchedUserId)
                .Return((u, s) => new
                {
                    Follow = s.As<UserNeo4j>()
                })
                .ResultsAsync.Result
                .Count() == 1;

            return areFriends;
        }

        public long ShortestPathToSearthedUser(int userId, int searchedUserId)
        {
            var shortestPath = _client.Cypher
                .Match("sp = shortestPath((:UserNeo4j {id: $uid})-[*]-(:UserNeo4j {id: $sid}))")
                .WithParam("uid", userId)
                .WithParam("sid", searchedUserId)
                .Return(sp => sp.Length())
                .ResultsAsync.Result;
            return shortestPath.First();
        }

    }
}
