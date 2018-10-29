using HotChocolate.Execution;
using HotChocolate.Resolvers;
using HotChocolate.Types;

namespace HotChocolate.Directives.Http
{
    /// <summary>
    /// A directive that runs the <c>URL</c> decode on the previous result
    /// described by the RFC 3986 standard.
    /// </summary>
    public sealed class UrlDecodeDirectiveType
        : DirectiveType<UrlDecodeDirective>
    {
        private static readonly UrlDecoder _decoder = new UrlDecoder();

        /// <inheritdoc/>
        protected override void Configure(
            IDirectiveTypeDescriptor<UrlDecodeDirective> descriptor)
        {
            descriptor.Name(DirectiveNames.UrlEncode);
            descriptor.Location(DirectiveLocation.FieldDefinition);
            descriptor.Middleware(next => context =>
            {
                UrlDecode(context);

                return next.Invoke(context);
            });
        }

        private void UrlDecode(IDirectiveContext context)
        {
            if (context.Result != null)
            {
                if (context.Result is string result)
                {
                    context.Result = _decoder.Decode(result);
                }
                else
                {
                    throw new QueryException(QueryErrors
                        .UrlEncodeExpectsStringResult);
                }
            }
        }
    }
}
