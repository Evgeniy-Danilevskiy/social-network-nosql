using Neo4jClient;
using sns_nrdb.Data.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sns_nrdb
{
    public class Neo4jConnection
    {

        private readonly BoltGraphClient client;

        public Neo4jConnection(string uri, string username, string password)
        {
            client = new BoltGraphClient(uri, username, password);
            client.ConnectAsync().Wait();
            Users = new UserRepoNeo4j(client);
        }

        public UserRepoNeo4j Users { get; }
    }
}
