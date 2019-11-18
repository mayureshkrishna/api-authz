using apiauthz.Models;
using MongoDB.Driver;
using System.Collections.Generic;
using System.Linq;
using System;

namespace apiauthz.Services
{
    public class AuthorizationService
    {
        private readonly IMongoCollection<Authorization> _authorizations;
        private readonly IMongoCollection<Role> _roles;
        private readonly IMongoCollection<Policy> _policies;

        public AuthorizationService(IResourceAccessDatabaseSettings settings)
        {
            var client = new MongoClient(settings.ConnectionString);
            var database = client.GetDatabase(settings.DatabaseName);

            _authorizations = database.GetCollection<Authorization>(settings.AuthorizationsCollectionName);
            _roles = database.GetCollection<Role>(settings.RolesCollectionName);
            _policies = database.GetCollection<Policy>(settings.PoliciesCollectionName);

        }

        public List<Authorization> Get() =>
            _authorizations.Find(authorization => true).ToList();


        public List<Authorization> GetByResourceId(String resourceId)
        {
            var condition = Builders<Authorization>.Filter.Eq(p => p.ResourceId, resourceId);
            var results = _authorizations.Find(condition).ToList().AsQueryable();
            return (System.Collections.Generic.List<apiauthz.Models.Authorization>)results;
        }


        public Authorization Get(Guid id) =>
            _authorizations.Find<Authorization>(authorization => authorization.Id == id).FirstOrDefault();


        public Authorization Validate(String resourceId, String userId, String action, Scope scope)
        {
            bool validUser = false;
            //var condition = Builders<Authorization>.Filter.Eq(p => p.ResourceId, resourceId);
            var authorization = _authorizations.Find<Authorization>(results => results.ResourceId == resourceId).FirstOrDefault();
            Authorization authResult = null;

            // foreach (var authorization in results)
            //{
            if (authorization !=null)
            {

            
                var role = _roles.Find<Role>(role => role.Id == authorization.RoleId).FirstOrDefault();
                if (role != null)
                {
                    validUser = role.Identities.Users.Contains(userId);
                    if (validUser == false)
                    {
                        //TODO: Get AD User groups for User Id in the input
                        List<String> userGroups = new List<string>
                            {
                                "fvf vdfv",
                                "vfdr erter",
                                "AZUREModifyGroup"
                            };

                        if (userGroups != null)
                        {

                            validUser = userGroups.Any(group => role.Identities.Groups.Contains(group));
                        }

                    }
                    if (validUser == true)
                    {

                        var policy = _policies.Find<Policy>(policy => policy.Id == authorization.PolicyId).FirstOrDefault();
                        if (policy != null)
                        {
                            validUser = policy.Actions.Contains(action);
                            if (validUser == true)
                            {
                                /*
                                if (authorization.ScopeMap.Count > 0 && policy.Scopes.Count > 0)
                                {
                                    var reducedScope = authorization.ScopeMap.Where(s => policy.Scopes.Contains(s.Key))
                                    .ToDictionary(dict => dict.Key, dict => dict.Value);

                                    if (reducedScope != null)
                                    {
                                        validUser = reducedScope.All(sc => scope.Contains(sc));
                                    }
                                }
                                if (validUser == true)
                                {
                                    authResult = authorization;
                                    // break;
                                }
                                */
                                authResult = authorization;
                            }
                        }
                }
            }

            }
            //}

            return authResult;

        }
        
        
        public Authorization Create(Authorization authorization)
        {
            _authorizations.InsertOne(authorization);
            return authorization;
            
        }

        public void Update(Guid id, Authorization authorizationIn) =>
            _authorizations.ReplaceOne(authorization => authorization.Id == id, authorizationIn);

        public void Remove(Authorization authorizationIn) =>
            _authorizations.DeleteOne(authorization => authorization.Id == authorizationIn.Id);

        public void Remove(Guid id) =>
            _authorizations.DeleteOne(authorization => authorization.Id == id);
     }
 }
