using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GreenDonut;

namespace HotChocolate.Directives.Http
{
    internal class HttpGetDataLoader
        : DataLoaderBase<string, string>
    {
        private static readonly DataLoaderOptions<string> _options =
            new DataLoaderOptions<string>
            {
                Batching = false
            };
        private readonly IHttpClient _client;

        public HttpGetDataLoader(IHttpClient client)
            : base(_options)
        {
            _client = client ??
                throw new ArgumentNullException(nameof(client));
        }

        protected override Task<IReadOnlyList<Result<string>>> Fetch(
            IReadOnlyList<string> keys)
        {
            throw new NotImplementedException();
        }
    }
}
