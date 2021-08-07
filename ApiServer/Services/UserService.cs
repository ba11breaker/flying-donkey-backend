using ApiServer.Models;
using MongoDB.Driver;
using System.Collections.Generic;
using System.Linq;

namespace ApiServer.Services
{
    public class UserService
    {
        private readonly IMongoCollection<User> _users;
        private readonly string UsersCollectionName = "Users";

        public UserService(IDatabaseSettings settings)
        {
            var client = new MongoClient(settings.ConnectionString);
            var database = client.GetDatabase(settings.DatabaseName);

            _users = database.GetCollection<User>(UsersCollectionName);
        }

        public List<User> GetUsers() => _users.Find(GetUsers => true).ToList();

        public User Get(string id) => _users.Find<User>(user => user.Id == id).FirstOrDefault();

        public User Create(User user)
        {
            _users.InsertOne(user);
            return user;
        }
        public void Update(string id, User userIn) => _users.ReplaceOne(user => user.Id == id, userIn);
        public void Remove(User userIn) => _users.DeleteOne(user => user.Id == userIn.Id);
        public void Remove(string id) => _users.DeleteOne(users => users.Id == id);
    }
}