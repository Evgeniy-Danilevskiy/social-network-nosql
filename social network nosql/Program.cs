

using Microsoft.Extensions.Configuration;
using Microsoft.VisualBasic.ApplicationServices;
using Neo4jClient;
using sns_nrdb.ConsoleApp;
using sns_nrdb.Data.Repositories;
using System;
using System.Net.PeerToPeer;
using System.Runtime.Remoting.Contexts;
using static ServiceStack.Diagnostics.Events;
using System.Security.Policy;

namespace sns_nrdb
{
    public class Program
    {
        private static IConfiguration cfg = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
        static string ConnectionString
        {
            get
            {
                return cfg.GetConnectionString("SNS");
            }
        }

        //neo4j connection info
        static string Url
        {
            get
            {
                return cfg.GetSection("neo4j")["url"];
            }
        }

        static string Username
        {
            get
            {
                return cfg.GetSection("neo4j")["username"];
            }
        }

        static string Password
        {
            get
            {
                return cfg.GetSection("neo4j")["password"];
            }
        }
        static void Main(string[] args)
        {

            var dbConnect = new DbConnection(ConnectionString);
            var neo4jConnect = new Neo4jConnection(Url, Username, Password);

            var login = new Login(dbConnect, neo4jConnect);
            var menu = new Menu(login, dbConnect, neo4jConnect);


            menu.ShowMenu();
        }
    }
}
