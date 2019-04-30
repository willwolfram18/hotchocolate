using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using HotChocolate.Execution.Instrumentation;
using HotChocolate.Language;
using HotChocolate.Properties;
using HotChocolate.Resolvers;
using HotChocolate.Types;
using Microsoft.Extensions.DependencyInjection;

namespace HotChocolate.Execution
{
    internal sealed partial class ResolverTask
        : IMiddlewareContext
    {
        #region IResolverContext

        ISchema IResolverContext.Schema => _executionContext.Schema;

        ObjectType IResolverContext.ObjectType => ObjectType;

        ObjectField IResolverContext.Field => FieldSelection.Field;

        DocumentNode IResolverContext.QueryDocument =>
            _executionContext.Operation.Document;

        DocumentNode IResolverContext.Document =>
            _executionContext.Operation.Document;

        OperationDefinitionNode IResolverContext.Operation =>
            _executionContext.Operation.Definition;

        FieldNode IResolverContext.FieldSelection =>
            FieldSelection.Selection;

        IImmutableStack<object> IResolverContext.Source => Source;

        Path IResolverContext.Path => Path;

        IImmutableDictionary<string, object> IResolverContext.ScopedContextData
        {
            get => ScopedContextData;
            set => ScopedContextData = value;
        }

        CancellationToken IResolverContext.RequestAborted =>
            _executionContext.RequestAborted;

        CancellationToken IResolverContext.CancellationToken =>
            _executionContext.RequestAborted;

        IDictionary<string, object> IHasContextData.ContextData =>
            _executionContext.ContextData;

        object IMiddlewareContext.Result
        {
            get => ResolverResult;
            set
            {
                if (value is IResolverResult r)
                {
                    if (r.IsError)
                    {
                        ResolverResult = QueryError.CreateFieldError(
                            r.ErrorMessage,
                            Path,
                            FieldSelection.Selection);
                    }
                    else
                    {
                        ResolverResult = r.Value;
                    }
                }
                else
                {
                    ResolverResult = value;
                }
                IsResultModified = true;

            }
        }

        public bool IsResultModified { get; private set; }

        T IResolverContext.Parent<T>()
        {
            return (T)Parent;
        }

        T IResolverContext.Argument<T>(NameString name) => Argument<T>(name);

        private T Argument<T>(NameString name)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentNullException(nameof(name));
            }

            if (_arguments.TryGetValue(name, out ArgumentValue argumentValue))
            {
                return CoerceArgumentValue<T>(name, argumentValue);
            }

            return default;
        }

        private T CoerceArgumentValue<T>(
            string name,
            ArgumentValue argumentValue)
        {
            if (argumentValue.Value is T value)
            {
                return value;
            }

            if (argumentValue.Value == null)
            {
                return default;
            }

            if (TryConvertValue(argumentValue, out value))
            {
                return value;
            }

            IError error = ErrorBuilder.New()
                .SetMessage(string.Format(
                    CultureInfo.InvariantCulture,
                    CoreResources.ResolverContext_ArgumentConversion,
                    name,
                    argumentValue.Type.ClrType.FullName,
                    typeof(T).FullName))
                .SetPath(Path)
                .AddLocation(FieldSelection.Selection)
                .Build();

            throw new QueryException(error);
        }

        private bool TryConvertValue<T>(
            ArgumentValue argumentValue,
            out T value)
        {
            if (_executionContext.TypeConversion.TryConvert(
                argumentValue.Type.ClrType, typeof(T),
                argumentValue.Value, out object converted))
            {
                value = (T)converted;
                return true;
            }

            value = default;
            return false;
        }

        T IResolverContext.Service<T>()
        {
            if (typeof(T) == typeof(IServiceProvider))
            {
                return (T)_executionContext.Services;
            }
            return (T)_executionContext.Services.GetRequiredService(typeof(T));
        }

        object IResolverContext.Service(Type service)
        {
            return _executionContext.Services.GetRequiredService(service);
        }

        T IResolverContext.CustomProperty<T>(string key)
        {
            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }

            if (_executionContext.ContextData.TryGetValue(
                key, out object value))
            {
                if (value is null)
                {
                    return default;
                }

                if (value is T v)
                {
                    return v;
                }
            }

            throw new ArgumentException(
                CoreResources.ResolverContext_CustomPropertyNotExists);
        }

        T IResolverContext.Resolver<T>()
        {
            return _executionContext.Activator.GetOrCreateResolver<T>();
        }

        void IResolverContext.ReportError(string errorMessage) =>
            ReportError(ErrorBuilder.New()
                .SetMessage(errorMessage)
                .SetPath(Path)
                .AddLocation(FieldSelection.Selection)
                .Build());
        void IResolverContext.ReportError(IError error) =>
            ReportError(_executionContext.ErrorHandler.Handle(error));

        IReadOnlyCollection<FieldSelection> IResolverContext.CollectFields(
            ObjectType typeContext) =>
            _executionContext.FieldHelper.CollectFields(
                typeContext, FieldSelection.Selection.SelectionSet);

        IReadOnlyCollection<FieldSelection> IResolverContext.CollectFields(
            ObjectType typeContext, SelectionSetNode selectionSet) =>
            _executionContext.FieldHelper.CollectFields(
                typeContext, FieldSelection.Selection.SelectionSet);

        async Task<T> IMiddlewareContext.ResolveAsync<T>()
        {
            if (!_isResultResolved)
            {
                if (FieldSelection.Field.Resolver == null)
                {
                    _resolvedResult = null;
                }
                else
                {
                    _resolvedResult = await FieldSelection.Field.Resolver
                        .Invoke(this).ConfigureAwait(false);
                }
                _isResultResolved = true;
            }

            return (T)_resolvedResult;
        }

        #endregion
    }
}
