using System.Threading.Tasks;

namespace Az.Serverless.Bre.Func01.Repositories.Interfaces
{
    public interface IRulesStoreRepository
    {
        /// <summary>
        /// Get the rule config as a json string
        /// </summary>
        /// <param name="configFileName">Name with which the rules config file is stored in the rules store</param>
        /// <returns>Rules config file as a json string</returns>
        public Task<object> GetConfigAsync(string configFileName);
    }
}
