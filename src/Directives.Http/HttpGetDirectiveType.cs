using System.Threading.Tasks;
using HotChocolate.Resolvers;
using HotChocolate.Types;

namespace HotChocolate.Directives.Http
{
    public sealed class HttpGetDirectiveType
        : DirectiveType<HttpGetDirective>
    {
        private static readonly UrlParser _parser = new UrlParser();

        protected override void Configure(
            IDirectiveTypeDescriptor<HttpGetDirective> descriptor)
        {
            descriptor.Name(DirectiveNames.HttpGet);
            descriptor.Location(DirectiveLocation.FieldDefinition);
            descriptor.Argument(ArgumentNames.Url)
                .Type<NonNullType<StringType>>();
            descriptor.Middleware(next => context =>
            {
                return Execute(context).ContinueWith(
                    previous => next.Invoke(context),
                    TaskContinuationOptions.RunContinuationsAsynchronously);
            });
        }

        private async Task Execute(IDirectiveContext context)
        {
            // 1. get all dynamic parts from the url
            // 2. use the values from the dynamic parts as key

            string rawUrl = context.Argument<string>(ArgumentNames.Url);
            string url = _parser.Parse(rawUrl, context);
            string key = "XXX";
            HttpGetDataLoader dataLaoder = context
                .DataLoader<HttpGetDataLoader>();

            context.Result = await dataLaoder.LoadAsync(key)
                .ConfigureAwait(false);
        }
    }
}
