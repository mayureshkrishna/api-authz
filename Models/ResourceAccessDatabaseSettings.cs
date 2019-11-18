using System;
namespace apiauthz.Models
{
    public class ResourceAccessDatabaseSettings : IResourceAccessDatabaseSettings
    {
        public string PoliciesCollectionName { get; set; }
        public string RolesCollectionName { get; set; }
        public string AuthorizationsCollectionName { get; set; }
        public string ConnectionString { get; set; }
        public string DatabaseName { get; set; }
    }

    public interface IResourceAccessDatabaseSettings
    {
        string PoliciesCollectionName { get; set; }
        string RolesCollectionName { get; set; }
        string AuthorizationsCollectionName { get; set; }
        string ConnectionString { get; set; }
        string DatabaseName { get; set; }
    }
}
