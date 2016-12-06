// ////////////////////////////////////////////////////////////////////////////
// COPYRIGHT (c) 2012 Schweitzer Engineering Laboratories, Pullman, WA
// ////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;

namespace SEL.Silverlight.RegionPartitioner
{
    /// <summary>
    /// MEF Requires a single common interface
    /// </summary>
    public interface IRegionContent : IComparable<IRegionContent>
    {
        FrameworkElement Create();

        string Name { get; }
        int Ordinal { get; }
    }

    public class RegionContentOrdinalComparer : IComparer<IRegionContent>
    {
        public int Compare(IRegionContent x, IRegionContent y)
        {
            return x.Ordinal.CompareTo(y.Ordinal);
        }
    }

    /// <summary>
    /// Base class of UI component modules.  
    /// The unused type parameter T is needed to reduce the scope of the static members to each derived type.
    /// </summary>
// ReSharper disable UnusedTypeParameter
    public abstract class RegionContent<T> : IRegionContent
// ReSharper restore UnusedTypeParameter
    {
        public FrameworkElement Create()
        {
            Initialize();
            Debug.WriteLine(string.Format("Creating {0}", Name));
            return CreateI();
        }

        private void Initialize()
        {
            if (_initialized)
            {
                return;
            }

            _initialized = true;

            Debug.WriteLine(string.Format("Initializing {0}", Name));
            InitI();
        }

        protected abstract void InitI();
        protected abstract FrameworkElement CreateI();

        public abstract string Name { get; }

        public abstract int Ordinal { get; }

        private static bool _initialized;

        public int CompareTo(IRegionContent other)
        {
            return String.CompareOrdinal(Name, other.Name);
        }
    }
}