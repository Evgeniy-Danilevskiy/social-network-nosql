using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sns_nrdb.ConsoleApp
{
    public class Login
    {
        private readonly DbConnection _dbConnection;
        private readonly Neo4jConnection _neo4jConnection;

        public Login(DbConnection dbConnection, Neo4jConnection neo4jConnection)
        {
            _dbConnection = dbConnection;
            _neo4jConnection = neo4jConnection;
        }

        public User CurrentUser { get; private set; }

        public bool IsAuthenticated()
        {
            return CurrentUser != null;
        }

        public bool SignIn(string email, string password)
        {
            if (CurrentUser != null)
            {
                throw new Exception("User already signed in, sign out first!");
            }

            var userMongo = _dbConnection.Users.GetByCredentials(email, password);
            var userNeo4j = _neo4jConnection.Users.GetUser(email, password);

            if (userMongo != null)
            {
                CurrentUser = userMongo;

                return true;
            }

            return false;
        }

        public void SignOut()
        {
            CurrentUser = null;
        }
    }
}
