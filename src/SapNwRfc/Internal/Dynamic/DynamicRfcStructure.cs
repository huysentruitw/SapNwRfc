using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using SapNwRfc.Internal.Interop;

namespace SapNwRfc.Internal.Dynamic
{
    internal sealed class DynamicRfcStructure : DynamicObject, IReadOnlyDictionary<string, object>
    {
        private readonly RfcInterop _interop;
        private readonly IntPtr _structureHandle;
        private readonly ISapTypeMetadata _typeMetadata;

        internal DynamicRfcStructure(RfcInterop interop, IntPtr structureHandle, ISapTypeMetadata typeMetadata)
        {
            _interop = interop;
            _structureHandle = structureHandle;
            _typeMetadata = typeMetadata;
        }

        private bool TryGetByName(string name, Type returnType, out object result)
        {
            if (_typeMetadata.Fields.TryGetValue(name, out ISapFieldMetadata field))
            {
                if (DynamicRfc.TryGetRfcValue(_interop, _structureHandle, field.Name, field.Type, field.GetTypeMetadata, returnType, out result))
                {
                    return true;
                }
            }

            result = null;
            return false;
        }

        private bool TryGetByIndex(int index, Type returnType, out object result)
        {
            if (index >= 0 && index < _typeMetadata.Fields.Count)
            {
                ISapFieldMetadata field = _typeMetadata.Fields[index];

                if (DynamicRfc.TryGetRfcValue(_interop, _structureHandle, field.Name, field.Type, field.GetTypeMetadata, returnType, out result))
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
                result = _typeMetadata;
                return true;
            }

            return base.TryInvokeMember(binder, args, out result);
        }

        public IEnumerable<string> Keys
        {
            get
            {
                foreach (var field in _typeMetadata.Fields)
                {
                    yield return field.Name;
                }
            }
        }

        public IEnumerable<object> Values
        {
            get
            {
                foreach (var field in _typeMetadata.Fields)
                {
                    if (TryGetByName(field.Name, typeof(object), out object result))
                    {
                        yield return result;
                    }
                }
            }
        }

        public int Count => _typeMetadata.Fields.Count;

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
            return _typeMetadata.Fields.TryGetValue(key, out _);
        }

        public bool TryGetValue(string key, out object value)
        {
            return TryGetByName(key, typeof(object), out value);
        }

        public IEnumerator<KeyValuePair<string, object>> GetEnumerator()
        {
            foreach (var field in _typeMetadata.Fields)
            {
                if (TryGetByName(field.Name, typeof(object), out object result))
                {
                    yield return new KeyValuePair<string, object>(field.Name, result);
                }
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
