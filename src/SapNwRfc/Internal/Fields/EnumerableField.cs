using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using SapNwRfc.Internal.Interop;

namespace SapNwRfc.Internal.Fields
{
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global", Justification = "Reflection use")]
    internal sealed class EnumerableField<TItem> : Field<IEnumerable<TItem>>
    {
        public EnumerableField(string name, IEnumerable<TItem> value)
            : base(name, value)
        {
        }

        public override void Apply(RfcInterop interop, IntPtr dataHandle)
        {
            if (Value == null)
                return;

            RfcResultCode resultCode = interop.GetTable(
                dataHandle: dataHandle,
                name: Name,
                out IntPtr tableHandle,
                out RfcErrorInfo errorInfo);

            resultCode.ThrowOnError(errorInfo);

            // Clear the table content before adding more - these tables are re-used between calls
            interop.DeleteAllRows(tableHandle, out errorInfo);

            foreach (TItem row in Value)
            {
                IntPtr lineHandle = interop.AppendNewRow(tableHandle, out errorInfo);
                errorInfo.ThrowOnError();
                InputMapper.Apply(interop, lineHandle, row);
            }
        }

        public static EnumerableField<T> Extract<T>(RfcInterop interop, IntPtr dataHandle, string name)
        {
            RfcResultCode resultCode = interop.GetTable(
                dataHandle: dataHandle,
                name: name,
                tableHandle: out IntPtr tableHandle,
                errorInfo: out RfcErrorInfo errorInfo);

            resultCode.ThrowOnError(errorInfo);

            resultCode = interop.GetRowCount(
                tableHandle: tableHandle,
                rowCount: out uint rowCount,
                errorInfo: out errorInfo);

            resultCode.ThrowOnError(errorInfo);

            if (rowCount == 0)
                return new EnumerableField<T>(name, Enumerable.Empty<T>());

            IEnumerable<T> rows = YieldTableRows<T>(interop, tableHandle);

            return new EnumerableField<T>(name, rows);
        }

        public static IEnumerable<T> YieldTableRows<T>(RfcInterop interop, IntPtr tableHandle)
        {
            RfcResultCode moveFirstResultCode = interop.MoveToFirstRow(
                tableHandle: tableHandle,
                errorInfo: out RfcErrorInfo moveFirstErrorInfo);

            if (moveFirstResultCode == RfcResultCode.RFC_TABLE_MOVE_BOF)
                yield break;

            moveFirstResultCode.ThrowOnError(moveFirstErrorInfo);

            while (true)
            {
                IntPtr rowHandle = interop.GetCurrentRow(
                    tableHandle: tableHandle,
                    errorInfo: out RfcErrorInfo errorInfo);

                errorInfo.ThrowOnError();

                yield return OutputMapper.Extract<T>(interop, rowHandle);

                RfcResultCode moveNextResultCode = interop.MoveToNextRow(
                    tableHandle: tableHandle,
                    errorInfo: out RfcErrorInfo moveNextErrorInfo);

                if (moveNextResultCode == RfcResultCode.RFC_TABLE_MOVE_EOF)
                    yield break;

                moveNextResultCode.ThrowOnError(moveNextErrorInfo);
            }
        }

        [ExcludeFromCodeCoverage]
        public override string ToString()
            => string.Join(Environment.NewLine, Value.Select((row, index) => $"[{index}] {row}"));
    }
}
