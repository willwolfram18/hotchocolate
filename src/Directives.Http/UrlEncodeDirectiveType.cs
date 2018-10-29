using HotChocolate.Execution;
using HotChocolate.Resolvers;
using HotChocolate.Types;

namespace HotChocolate.Directives.Http
{
    /// <summary>
    /// A directive that runs the <c>URL</c> encode on the previous result
    /// described by the RFC 3986 standard.
    /// </summary>
    public sealed class UrlEncodeDirectiveType
        : DirectiveType<UrlEncodeDirective>
    {
        private static readonly UrlEncoder _encoder = new UrlEncoder();

        /// <inheritdoc/>
        protected override void Configure(
            IDirectiveTypeDescriptor<UrlEncodeDirective> descriptor)
        {
            descriptor.Name(DirectiveNames.UrlEncode);
            descriptor.Location(DirectiveLocation.FieldDefinition);
            descriptor.Middleware(next => context =>
            {
                UrlEncode(context);

                return next.Invoke(context);
            });
        }

        private void UrlEncode(IDirectiveContext context)
        {
            if (context.Result != null)
            {
                if (context.Result is string result)
                {
                    context.Result = _encoder.Encode(result);
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
