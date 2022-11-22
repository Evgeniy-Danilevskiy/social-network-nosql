using Neo4jClient;
using sns_nrdb.Entities;
using System;
using System.Collections.Generic;

namespace sns_nrdb.ConsoleApp
{
    public class Menu
    {
        private readonly Login _login;
        private readonly DbConnection _dbConnection;
        private readonly Neo4jConnection _neo4jConnection;

        public Menu(Login login, DbConnection dbConnection, Neo4jConnection neo4jConnection)
        {
            _login = login;
            _dbConnection = dbConnection;
            _neo4jConnection = neo4jConnection;
        }

        public void ShowMenu()
        {
            if (_login.IsAuthenticated())
            {
                ShowAuthentaciatedUserMenu();
            }
            else
            {
                ShowAnonymousUserMenu();
            }
        }

        private void ShowAuthentaciatedUserMenu()
        {
            int input;

            do
            {
                Console.WriteLine("\n1. Show My Posts");
                Console.WriteLine("2. Show Latest Posts");
                Console.WriteLine("3. Create Post");
                Console.WriteLine("4. Create Comment");
                Console.WriteLine("5. Delete Account");
                Console.WriteLine("6. Show Users");
                Console.WriteLine("7. Subscribe to a User");
                Console.WriteLine("8. Show Subscriptions");
                Console.WriteLine("9. Show Stream");
                Console.WriteLine("10. Like/Dislike Post");
                Console.WriteLine("11. Show Friends");
                Console.WriteLine("12. Unsubscribe");
                Console.WriteLine("13. Check if user is a friend");
                Console.WriteLine("14. Show shortest path length");
                Console.WriteLine("15. Check you are subscribed");

                Console.WriteLine("0. Sign Out");
                Console.Write("Enter a number: ");
                input = int.Parse(Console.ReadLine());

                switch (input)
                {
                    case 1:
                        ShowAuthenticatedUserPost();
                        break;
                    case 2:
                        ShowLatestPosts();
                        break;
                    case 3:
                        CreatePost();
                        break;
                    case 4:
                        CreateComment();
                        break;
                    case 5:
                        DeleteUser();
                        break;
                    case 6:
                        ShowUsers();
                        break;
                    case 7:
                        SubscribeToAUser();
                        break;
                    case 8:
                        ShowFollows();
                        break;
                    case 9:
                        ShowPosts();
                        break;
                    case 10:
                        LikeOrUnlikePost();
                        break;
                    case 11:
                        ShowFriends();
                        break;
                    case 12:
                        Unsubsribe();
                        break;
                    case 13:
                        ShowIfFriends();
                        break;
                    case 14:
                        ShowPathLength();
                        break;
                    case 15:
                        ShowIfFollows();
                        break;
                    case 0:
                        SignOut();
                        break;
                }
            } while (input != 0);
        }

        private void DeleteUser()
        {
            Console.Write("Enter userId: ");
            int userId = int.Parse(Console.ReadLine());

            _dbConnection.Users.Delete(_login.CurrentUser.UserId);
            _neo4jConnection.Users.DeleteUser(_login.CurrentUser.UserId);
            Console.WriteLine($"User {_login.CurrentUser.Email} Deleted");
            SignOut();
        }
        private void CreateUser()
        {
            var userMongo = new User();
            var userNeo4j = new UserNeo4j();
            int input = -1;
            Console.Write("Name: ");
            var name = Console.ReadLine();
            Console.Write("Surname: ");
            var surname = Console.ReadLine();
            Console.Write("Email: ");
            var email = Console.ReadLine();
            Console.Write("Age: ");
            var age = int.Parse(Console.ReadLine());
            Console.Write("Password: ");
            var password = Console.ReadLine();
            Console.WriteLine("Interests: ");
            List<string> interests = new List<string>();
            do
            {
                Console.WriteLine("1.Add interest");
                Console.WriteLine("0.Exit");
                Console.Write(">> ");
                input = int.Parse(Console.ReadLine());
                switch (input)
                {
                    case 1:
                        Console.Write("Interest: ");
                        var interest = Console.ReadLine();
                        interests.Add(interest);
                        break;
                    case 0:
                        break;
                }
            } while (input != 0);
            List<int> subs = new List<int>();
            List<int> friends = new List<int>();
            userMongo = new User
            {
                Name = name,
                Surname = surname,
                Email = email,
                Age = age,
                Password = password,
                Interests = interests,
                Subscriptions = subs,
                Friends = friends
            };
            userNeo4j = new UserNeo4j
            {
                UserId = userMongo.UserId,
                Name = name,
                Surname = surname,
                Email = email,
                Age = age,
                Password = password
            };

            _dbConnection.Users.Add(userMongo);
            _neo4jConnection.Users.Add(userNeo4j);

            Console.WriteLine($"User {userMongo.Name} {userMongo.Surname} Created");
        }
        private void SubscribeToAUser()
        {
            Console.WriteLine("User Id: ");
            var userId = int.Parse(Console.ReadLine());
           _dbConnection.Users.Subscribe(_login.CurrentUser.UserId, userId);
           _neo4jConnection.Users.CreateRelationshipUserFollows(_login.CurrentUser.UserId, userId);
            Console.WriteLine("Subscribed successfully");

        }
        private void Unsubsribe()
        {
            Console.WriteLine("User Id: ");
            var userId = int.Parse(Console.ReadLine());
           _dbConnection.Users.Unsubscribe(_login.CurrentUser.UserId, userId);
           _neo4jConnection.Users.DeleteRelationshipUserFollower(_login.CurrentUser.UserId, userId);
            Console.WriteLine("Unsubscribed successfully");
        }
        private void LikeOrUnlikePost()
        {
            Console.WriteLine("Post Id: ");
            var postId = Console.ReadLine();
           _dbConnection.Posts.React(_login.CurrentUser.UserId, postId);
        }
        private void ShowPosts()
        {
            var posts =_dbConnection.Posts.GetAllUserPosts();
            foreach (var post in posts)
            {
                Console.WriteLine("\n" + post.IdString + "\n" + post.CreationDate.ToShortDateString() + "\n" + post.PostBody);
                Console.WriteLine("\n" + "Comments:");
                if (post.Comments != null)
                    foreach (var comment in post.Comments)
                    {
                        Console.WriteLine("\t" + comment.UserId + ": " + comment.CommentBody);
                    }
            }
        }
        private void ShowLatestPosts()
        {
            var latestPosts =_dbConnection.Posts.GetLatest();
            foreach (var post in latestPosts)
            {
                Console.WriteLine("\n" + post.IdString + "\n" + post.CreationDate.ToShortDateString() + "\n" + post.PostBody);
                Console.WriteLine("\n" + "Comments:");
                if (post.Comments != null)
                    foreach (var comment in post.Comments)
                    {
                        Console.WriteLine("\t" + comment.UserId + ": " + comment.CommentBody);
                    }
                Console.WriteLine("\n" + "Likes:");
                if (post.Comments != null)
                    foreach (var comment in post.Comments)
                    {
                        Console.WriteLine("\t" + comment.UserId + ": " + comment.CommentBody);
                    }
            }
        }
        private void ShowAuthenticatedUserPost()
        {
            var authenticatedUserPots =_dbConnection.Posts.GetUserPosts(_login.CurrentUser.UserId);
            foreach (var post in authenticatedUserPots)
            {
                Console.WriteLine("\n" + post.CreationDate.ToShortDateString() + "\n" + post.PostBody);
            }
        }
        private void CreateComment()
        {
            Console.WriteLine("Post Id: ");
            var postId = Console.ReadLine();

            Console.WriteLine("Body of the comment: ");
            var body = Console.ReadLine();
           _dbConnection.Posts.CreateComment(_login.CurrentUser.UserId, postId, body);
            Console.WriteLine("Comment created.");

        }
        private void CreatePost()
        {
            Console.WriteLine("Body of the post: ");
            var body = Console.ReadLine();
           _dbConnection.Posts.Create(_login.CurrentUser.UserId, body);
            Console.WriteLine("Post created.");
        }
        private void ShowUsers()
        {
            var users =_dbConnection.Users.GetAll();
            foreach (var user in users)
            {
                Console.WriteLine(user.Name + " " + user.Surname);
            }
        }
        private void ShowFollows()
        {
            var authenticatedUserFollowers =_dbConnection.Users.GetUserFollows(_login.CurrentUser.UserId);
            foreach (var follower in authenticatedUserFollowers)
            {
                Console.WriteLine(follower.Name + " " + follower.Surname);
            }
        }
        private void ShowFriends()
        {
            var authenticatedUserFollowers =_dbConnection.Users.GetUserFriends(_login.CurrentUser.UserId);
            foreach (var follower in authenticatedUserFollowers)
            {
                Console.WriteLine(follower.Name + " " + follower.Surname);
            }
        }

        private void ShowIfFollows()
        {
            Console.WriteLine("User Id: ");
            var userId = int.Parse(Console.ReadLine());
            var ifFollows = _neo4jConnection.Users.IfFollows(_login.CurrentUser.UserId, userId);
            Console.WriteLine("Follows: {0}", ifFollows);
        }

        private void ShowIfFriends()
        {
            Console.WriteLine("User Id: ");
            var userId = int.Parse(Console.ReadLine());
            var areFriends = _neo4jConnection.Users.AreFriends(_login.CurrentUser.UserId, userId);
            Console.WriteLine("Are Friends: {0}", areFriends);
        }

        private void ShowPathLength()
        {
            Console.WriteLine("User Id: ");
            var userId = int.Parse(Console.ReadLine());
            var sp = _neo4jConnection.Users.ShortestPathToSearthedUser(_login.CurrentUser.UserId, userId);
            if (sp == 0)
            {
                Console.WriteLine("There Is No Path To This User");
            }
            else
            {
                Console.WriteLine("Shortest Path: {0}", sp);
            }

        }
        private void ShowAnonymousUserMenu()
        {
            int input;

            do
            {
                Console.WriteLine("1. Sign In");
                Console.WriteLine("2. Create Account");
                Console.WriteLine("0. Exit");

                Console.Write(">> ");
                input = int.Parse(Console.ReadLine());

                switch (input)
                {
                    case 1:
                        SignIn();
                        break;
                    case 2:
                        CreateUser();
                        break;
                }

            } while (input != 0);
        }
        private void SignIn()
        {
            Console.WriteLine("Email: ");
            var email = Console.ReadLine();

            Console.WriteLine("Password: ");
            var password = Console.ReadLine();
            try
            {
                _login.SignIn(email, password);
            }
            catch (Exception)
            {
                Console.WriteLine("There was an error during authentication, check your credentials and try again");
                ShowAnonymousUserMenu();
            }

            if (_login.IsAuthenticated())
            {
                Console.WriteLine("Sign in successful");
                Console.WriteLine("Hello, " + _login.CurrentUser.Name);
                ShowAuthentaciatedUserMenu();
            }
            else
            {
                Console.WriteLine("There was an error during authentication, check your credentials and try again");
                ShowAnonymousUserMenu();
            }
        }


        private void SignOut()
        {
            _login.SignOut();
            ShowAnonymousUserMenu();
        }


    }
}