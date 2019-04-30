using System;
using System.Collections.Generic;
using System.Collections.Immutable;
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
        : IShared
    {
        public void Initialize(
            IExecutionContext executionContext,
            FieldSelection fieldSelection,
            IImmutableStack<object> source,
            IDictionary<string, object> result)
        {
            _executionContext = executionContext;
            Source = source;
            ObjectType = fieldSelection.Field.DeclaringType;
            FieldSelection = fieldSelection;
            FieldType = fieldSelection.Field.Type;
            Path = Path.New(fieldSelection.ResponseName);
            _result = result;
            ScopedContextData = ImmutableDictionary<string, object>.Empty;
            Parent = executionContext.Operation.RootValue;

            FieldDelegate = executionContext.FieldHelper
                .CreateMiddleware(fieldSelection);
            _arguments = fieldSelection.CoerceArgumentValues(
                executionContext.Variables, Path);
            IsRootTask = true;
        }

        private void Initialize(
            ResolverTask parent,
            FieldSelection fieldSelection,
            Path path,
            IImmutableStack<object> source,
            object parentResult,
            IDictionary<string, object> result,
            Action propagateNonNullViolation)
        {
            _executionContext = parent._executionContext;
            Source = source;
            ObjectType = fieldSelection.Field.DeclaringType;
            FieldSelection = fieldSelection;
            FieldType = fieldSelection.Field.Type;
            Path = path;
            _result = result;
            ScopedContextData = parent.ScopedContextData;
            Parent = parentResult;

            FieldDelegate = parent._executionContext.FieldHelper
                .CreateMiddleware(fieldSelection);
            _arguments = fieldSelection.CoerceArgumentValues(
                parent._executionContext.Variables, Path);
            IsRootTask = false;

            bool isNonNullType = FieldSelection.Field.Type.IsNonNullType();
            string responseName = FieldSelection.ResponseName;
            Action parentPropagateNonNullViolation =
                parent.PropagateNonNullViolation;

            _propagateNonNullViolation = () =>
            {
                if (isNonNullType)
                {
                    if (_propagateNonNullViolation != null)
                    {
                        propagateNonNullViolation.Invoke();
                    }
                    else if (parentPropagateNonNullViolation != null)
                    {
                        parentPropagateNonNullViolation.Invoke();
                    }
                }
                result[FieldSelection.ResponseName] = null;
            };
        }

        public void Clean()
        {
            _executionContext = null;
            Source = null;
            Parent = null;
            ObjectType = null;
            FieldSelection = null;
            FieldType = null;
            Path = null;
            _result = null;
            ScopedContextData = null;
            _propagateNonNullViolation = null;
            FieldDelegate = null;
            Task = null;
            ResolverResult = null;
            _arguments = null;
            IsRootTask = false;
            _isResultResolved = false;
            _resolvedResult = null;
        }
    }
}
