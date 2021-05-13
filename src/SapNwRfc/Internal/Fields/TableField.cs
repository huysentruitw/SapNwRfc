using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using SapNwRfc.Internal.Interop;

namespace SapNwRfc.Internal.Fields
{
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global", Justification = "Reflection use")]
    internal sealed class TableField<TItem> : Field<TItem[]>
    {
        public TableField(string name, TItem[] value)
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

            foreach (TItem row in Value)
            {
                IntPtr lineHandle = interop.AppendNewRow(tableHandle, out errorInfo);
                errorInfo.ThrowOnError();
                InputMapper.Apply(interop, lineHandle, row);
            }
        }

        public static TableField<T> Extract<T>(RfcInterop interop, IntPtr dataHandle, string name)
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

            T[] rows = Array.Empty<T>();

            if (rowCount > 0)
            {
                rows = new T[rowCount];

                resultCode = interop.MoveToFirstRow(
                    tableHandle: tableHandle,
                    errorInfo: out errorInfo);

                resultCode.ThrowOnError(errorInfo);

                for (int i = 0; i < rowCount; i++)
                {
                    IntPtr rowHandle = interop.GetCurrentRow(
                        tableHandle: tableHandle,
                        errorInfo: out errorInfo);

                    errorInfo.ThrowOnError();

                    rows[i] = OutputMapper.Extract<T>(interop, rowHandle);

                    resultCode = interop.MoveToNextRow(
                        tableHandle: tableHandle,
                        errorInfo: out errorInfo);

                    if (resultCode == RfcResultCode.RFC_TABLE_MOVE_EOF)
                        return new TableField<T>(name, rows.Take(i + 1).ToArray());

                    resultCode.ThrowOnError(errorInfo);
                }
            }

            return new TableField<T>(name, rows);
        }

        [ExcludeFromCodeCoverage]
        public override string ToString()
            => string.Join(Environment.NewLine, Value.Select((row, index) => $"[{index}] {row}"));
    }
}
