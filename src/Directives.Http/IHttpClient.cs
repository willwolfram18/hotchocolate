using System.Threading.Tasks;

namespace HotChocolate.Directives.Http
{
    internal interface IHttpClient
    {
        Task<string> Get(string url);
    }
}
