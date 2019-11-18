using apiauthz.Models;
using MongoDB.Driver;
using System.Collections.Generic;
using System.Linq;
using System;

namespace apiauthz.Services
{
    public class PolicyService
    {
        private readonly IMongoCollection<Policy> _policies;

        public PolicyService(IResourceAccessDatabaseSettings settings)
        {
            var client = new MongoClient(settings.ConnectionString);
            var database = client.GetDatabase(settings.DatabaseName);

            _policies = database.GetCollection<Policy>(settings.PoliciesCollectionName);
        }

        public List<Policy> Get() =>
            _policies.Find(policy => true).ToList();

        public Policy Get(Guid id) =>
            _policies.Find<Policy>(policy => policy.Id == id).FirstOrDefault();

        public Policy Create(Policy policy)
        {
            _policies.InsertOne(policy);
            return policy;
        }

        public void Update(Guid id, Policy policyIn) =>
            _policies.ReplaceOne(policy => policy.Id == id, policyIn);

        public void Remove(Policy policyIn) =>
            _policies.DeleteOne(policy => policy.Id == policyIn.Id);

        public void Remove(Guid id) =>
            _policies.DeleteOne(policy => policy.Id == id);
     }
 }
