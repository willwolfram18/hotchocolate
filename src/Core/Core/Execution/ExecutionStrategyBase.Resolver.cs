using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using HotChocolate.Resolvers;

namespace HotChocolate.Execution
{
    internal abstract partial class ExecutionStrategyBase
    {
        protected static async Task<object> ExecuteResolverAsync(
           ResolverTask resolverTask,
           IErrorHandler errorHandler,
           CancellationToken cancellationToken)
        {
            Activity activity = resolverTask.Diagnostics.BeginResolveField(
                resolverTask);
            object result = null;

            try
            {
                result = await ExecuteMiddlewareAsync(
                    resolverTask, errorHandler)
                    .ConfigureAwait(false);

                if (result is IEnumerable<IError> errors)
                {
                    resolverTask.Diagnostics.ResolverError(
                        resolverTask, errors);
                }
                else if (result is IError error)
                {
                    resolverTask.Diagnostics.ResolverError(
                        resolverTask,
                        new IError[] { error });
                }
            }
            finally
            {
                resolverTask.Diagnostics.EndResolveField(
                    activity,
                    resolverTask,
                    result);
            }

            return result;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static async Task<object> ExecuteMiddlewareAsync(
            ResolverTask resolverTask,
            IErrorHandler errorHandler)
        {
            object result = null;

            try
            {
                result = await ExecuteFieldMiddlewareAsync(resolverTask)
                    .ConfigureAwait(false);

                if (result is IError error)
                {
                    return errorHandler.Handle(error);
                }
                else if (result is IEnumerable<IError> errors)
                {
                    return errorHandler.Handle(errors);
                }
                else
                {
                    return result;
                }
            }
            catch (QueryException ex)
            {
                return errorHandler.Handle(ex.Errors);
            }
            catch (Exception ex)
            {
                return errorHandler.Handle(ex, builder => builder
                    .SetPath(resolverTask.Path)
                    .AddLocation(resolverTask.FieldSelection.Selection));
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static async Task<object> ExecuteFieldMiddlewareAsync(
            ResolverTask resolverTask)
        {
            await resolverTask.FieldDelegate.Invoke(resolverTask)
                .ConfigureAwait(false);

            return resolverTask.ResolverResult;
        }
    }
}
