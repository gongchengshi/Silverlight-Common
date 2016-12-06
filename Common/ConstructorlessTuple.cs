// ////////////////////////////////////////////////////////////////////////////
// COPYRIGHT (c) 2012 Schweitzer Engineering Laboratories, Pullman, WA
// ////////////////////////////////////////////////////////////////////////////

namespace SEL
{
    /// <summary>
    /// These classes are only needed for DataContract serialization in Silverlight
    /// The DataContract serializer in Silverlight requires a default constructor
    /// The regular Tuple has no default constructor
    /// </summary>
    public struct ConstructorlessTuple<T1, T2>
    {
        public T1 Item1;
        public T2 Item2;
    }

    public struct ConstructorlessTuple<T1, T2, T3>
    {
        public T1 Item1;
        public T2 Item2;
        public T3 Item3;
    }

    public struct ConstructorlessTuple<T1, T2, T3, T4>
    {
        public T1 Item1;
        public T2 Item2;
        public T3 Item3;
        public T4 Item4;
    }

    public struct ConstructorlessTuple<T1, T2, T3, T4, T5>
    {
        public T1 Item1;
        public T2 Item2;
        public T3 Item3;
        public T4 Item4;
        public T5 Item5;
    }
}