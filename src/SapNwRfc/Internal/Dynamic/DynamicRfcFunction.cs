using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using SapNwRfc.Internal.Interop;

namespace SapNwRfc.Internal.Dynamic
{
    internal sealed class DynamicRfcFunction : DynamicObject, IReadOnlyDictionary<string, object>
    {
        private readonly RfcInterop _interop;
        private readonly IntPtr _functionHandle;
        private readonly ISapFunctionMetadata _functionMetadata;

        internal DynamicRfcFunction(RfcInterop interop, IntPtr functionHandle, ISapFunctionMetadata functionMetadata)
        {
            _interop = interop;
            _functionHandle = functionHandle;
            _functionMetadata = functionMetadata;
        }

        private bool TryGetByName(string name, Type returnType, out object result)
        {
            if (_functionMetadata.Parameters.TryGetValue(name, out ISapParameterMetadata parameter))
            {
                if (DynamicRfc.TryGetRfcValue(_interop, _functionHandle, parameter.Name, parameter.Type, parameter.GetTypeMetadata, returnType, out result))
                {
                    return true;
                }
            }

            result = null;
            return false;
        }

        private bool TryGetByIndex(int index, Type returnType, out object result)
        {
            if (index >= 0 && index < _functionMetadata.Parameters.Count)
            {
                ISapParameterMetadata parameter = _functionMetadata.Parameters[index];

                if (DynamicRfc.TryGetRfcValue(_interop, _functionHandle, parameter.Name, parameter.Type, parameter.GetTypeMetadata, returnType, out result))
                {
                    return true;
                }
            }

            result = null;
            return false;
        }

        public override IEnumerable<string> GetDynamicMemberNames()
        {
            return Keys;
        }

        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            return TryGetByName(binder.Name, binder.ReturnType, out result);
        }

        public override bool TryGetIndex(GetIndexBinder binder, object[] indexes, out object result)
        {
            if (indexes.Length != 1)
                throw new ArgumentOutOfRangeException(nameof(indexes));

            var index = indexes[0];

            if (index is string name)
                return TryGetByName(name, binder.ReturnType, out result);

            if (index is int i)
                return TryGetByIndex(i, binder.ReturnType, out result);

            if (index is uint u)
                return TryGetByIndex((int)u, binder.ReturnType, out result);

            throw new ArgumentException(nameof(indexes));
        }

        public override bool TryInvokeMember(InvokeMemberBinder binder, object[] args, out object result)
        {
            if (binder.Name == "GetMetadata" && binder.CallInfo.ArgumentCount == 0)
            {
                result = _functionMetadata;
                return true;
            }

            return base.TryInvokeMember(binder, args, out result);
        }

        public IEnumerable<string> Keys
        {
            get
            {
                foreach (var parameter in _functionMetadata.Parameters)
                {
                    yield return parameter.Name;
                }
            }
        }

        public IEnumerable<object> Values
        {
            get
            {
                foreach (var parameter in _functionMetadata.Parameters)
                {
                    if (TryGetByName(parameter.Name, typeof(object), out object result))
                    {
                        yield return result;
                    }
                }
            }
        }

        public int Count => _functionMetadata.Parameters.Count;

        public object this[string key]
        {
            get
            {
                if (TryGetByName(key, typeof(object), out var result))
                {
                    return result;
                }

                throw new KeyNotFoundException();
            }
        }

        public bool ContainsKey(string key)
        {
            return _functionMetadata.Parameters.TryGetValue(key, out _);
        }

        public bool TryGetValue(string key, out object value)
        {
            return TryGetByName(key, typeof(object), out value);
        }

        public IEnumerator<KeyValuePair<string, object>> GetEnumerator()
        {
            foreach (var parameter in _functionMetadata.Parameters)
            {
                if (TryGetByName(parameter.Name, typeof(object), out object result))
                {
                    yield return new KeyValuePair<string, object>(parameter.Name, result);
                }
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
