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
    {
        private IExecutionContext _executionContext;
        private IDictionary<string, object> _result;
        private Action _propagateNonNullViolation;
        private Dictionary<string, ArgumentValue> _arguments;
        private bool _isResultResolved;
        private object _resolvedResult;

        public ResolverTask Branch(
            FieldSelection fieldSelection,
            Path path,
            IImmutableStack<object> source,
            object resolverResult,
            IDictionary<string, object> result,
            Action propagateNonNullViolation)
        {
            ResolverTask branch = ObjectPools.ResolverTasks.Rent();
            branch.Initialize(
                this,
                fieldSelection,
                path,
                source,
                resolverResult,
                result,
                propagateNonNullViolation);
            return branch;
        }

        public object Parent { get; private set; }

        public bool IsRootTask { get; private set; }

        public IImmutableStack<object> Source { get; private set; }

        public ObjectType ObjectType { get; private set; }

        public FieldSelection FieldSelection { get; private set; }

        public IType FieldType { get; private set; }

        public Path Path { get; private set; }

        public Task<object> Task { get; set; }

        public object ResolverResult { get; set; }

        public FieldDelegate FieldDelegate { get; private set; }

        public IImmutableDictionary<string, object> ScopedContextData
        {
            get;
            set;
        }

        public QueryExecutionDiagnostics Diagnostics
        {
            get
            {
                return _executionContext.Diagnostics;
            }
        }

        public void PropagateNonNullViolation()
        {
            if (_propagateNonNullViolation != null)
            {
                _propagateNonNullViolation.Invoke();
            }
            _result[FieldSelection.ResponseName] = null;
        }

        public void SetResult(object value)
        {
            _result[FieldSelection.ResponseName] = value;
        }

        public void ReportError(IError error)
        {
            if (error == null)
            {
                throw new ArgumentNullException(nameof(error));
            }

            _executionContext.AddError(error);
        }

        public IError CreateError(string message)
        {
            if (string.IsNullOrEmpty(message))
            {
                throw new ArgumentException(
                    CoreResources.ResolverTask_ErrorMessageIsNull,
                    nameof(message));
            }

            return QueryError.CreateFieldError(
                message,
                Path,
                FieldSelection.Selection);
        }
    }
}
