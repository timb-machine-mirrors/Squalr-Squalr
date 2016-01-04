﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace Anathema
{
    /// <summary>
    /// Class used by SnapshotRegion as a wrapper for indexing into the raw collection of data
    /// </summary>
    public class SnapshotElement
    {
        public readonly IntPtr BaseAddress;     // Address of this Element
        public readonly Byte[] PreviousValue;   // Raw previous and values
        public readonly Byte[] CurrentValue;    // Raw current values

        private SnapshotRegion Parent;
        private Int32 Index;

        private Boolean _Valid;
        public Boolean Valid { get { return _Valid; } set { _Valid = value; Parent[Index] = this; } }
        public Type ElementType { get; set; }   // Type for interpreting the stored values

        protected SnapshotElement() { }
        public SnapshotElement(IntPtr BaseAddress, SnapshotRegion Parent, Int32 Index, Type ElementType, Boolean Valid, Byte[] CurrentValue, Byte[] PreviousValue)
        {
            this.BaseAddress = BaseAddress;
            this.Parent = Parent;
            this.Index = Index; 
            this.ElementType = ElementType;
            this.Valid = Valid;
            this.CurrentValue = CurrentValue;
            this.PreviousValue = PreviousValue;

            // Mark invalid automatically if the value initialized to null -- this is likely due to reading passed the value buffer
            // For example trying to read a Int32 at byte 1021 of a 1024 byte region
            if (CurrentValue == null)
                this.Valid = false;
        }

        private dynamic GetValue(Byte[] Array)
        {
            dynamic Value = 0;
            var @switch = new Dictionary<Type, Action> {
                    { typeof(Byte), () => Value = Array[0] },
                    { typeof(SByte), () => Value = (SByte)Array[0] },
                    { typeof(Int16), () => Value = BitConverter.ToInt16(Array, 0) },
                    { typeof(Int32), () => Value = BitConverter.ToInt32(Array, 0) },
                    { typeof(Int64), () => Value = BitConverter.ToInt64(Array, 0) },
                    { typeof(UInt16), () => Value = BitConverter.ToUInt16(Array, 0) },
                    { typeof(UInt32), () => Value = BitConverter.ToUInt32(Array, 0) },
                    { typeof(UInt64), () => Value = BitConverter.ToUInt64(Array, 0) },
                    { typeof(Single), () => Value = BitConverter.ToSingle(Array, 0) },
                    { typeof(Double), () => Value = BitConverter.ToDouble(Array, 0) }
                };

            if (@switch.ContainsKey(ElementType))
                @switch[ElementType]();

            return Value;
        }

        public Boolean Changed()
        {
            return !CurrentValue.SequenceEqual(PreviousValue);
        }

        public Boolean Unchanged()
        {
            return CurrentValue.SequenceEqual(PreviousValue);
        }

        public Boolean Increased()
        {
            return (GetValue(CurrentValue) > GetValue(PreviousValue));
        }

        public Boolean Decreased()
        {
            return (GetValue(CurrentValue) < GetValue(PreviousValue));
        }

        public Boolean EqualToValue(dynamic Value)
        {
            return (GetValue(CurrentValue) == Value);
        }

        public Boolean NotEqualToValue(dynamic Value)
        {
            return (GetValue(CurrentValue) != Value);
        }

        public Boolean GreaterThanValue(dynamic Value)
        {
            return (GetValue(CurrentValue) > Value);
        }

        public Boolean LessThanValue(dynamic Value)
        {
            return (GetValue(CurrentValue) < Value);
        }

        public Boolean IncreasedByValue(dynamic Value)
        {
            return (GetValue(CurrentValue) == GetValue(PreviousValue) + Value);
        }

        public Boolean DecreasedByValue(dynamic Value)
        {
            return (GetValue(CurrentValue) == GetValue(PreviousValue) - Value);
        }
    }

    public class SnapshotElement<T> : SnapshotElement where T : struct
    {
        // Variables required for committing changes back to the region from which this element comes
        private SnapshotRegion<T> Parent;
        private Int32 Index;

        private T? _MemoryLabel;
        public T? MemoryLabel { get { return _MemoryLabel; } set { _MemoryLabel = value; Parent[Index] = this; } }

        public SnapshotElement(IntPtr BaseAddress, SnapshotRegion<T> Parent, Int32 Index, Type ElementType, Boolean Valid, Byte[] CurrentValue, Byte[] PreviousValue, T? Label)
            : base(BaseAddress, Parent, Index, ElementType, Valid, CurrentValue, PreviousValue)
        {
            this.Parent = Parent;
            this.Index = Index;
            this._MemoryLabel = Label;
        }
    }
}