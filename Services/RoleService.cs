using apiauthz.Models;
using MongoDB.Driver;
using System.Collections.Generic;
using System.Linq;
using System;

namespace apiauthz.Services
{
    public class RoleService
    {
        private readonly IMongoCollection<Role> _roles;

        public RoleService(IResourceAccessDatabaseSettings settings)
        {
            var client = new MongoClient(settings.ConnectionString);
            var database = client.GetDatabase(settings.DatabaseName);

            _roles = database.GetCollection<Role>(settings.RolesCollectionName);
        }

        public List<Role> Get() =>
            _roles.Find(role => true).ToList();

        public Role Get(Guid id) =>
            _roles.Find<Role>(role => role.Id == id).FirstOrDefault();

        public Role Create(Role role)
        {
            _roles.InsertOne(role);
            return role;
        }

        public void Update(Guid id, Role roleIn) =>
            _roles.ReplaceOne(role => role.Id == id, roleIn);

        public void Remove(Role roleIn) =>
            _roles.DeleteOne(role => role.Id == roleIn.Id);

        public void Remove(Guid id) =>
            _roles.DeleteOne(role => role.Id == id);
     }
 }
