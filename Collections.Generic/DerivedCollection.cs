// ////////////////////////////////////////////////////////////////////////////
// COPYRIGHT (c) 2012 Schweitzer Engineering Laboratories, Pullman, WA
// ////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Linq;

namespace SEL.Collections.Generic
{
    /// <summary>
    /// Derive an ICollection&lt;T&gt from other properties/collections.
    /// </summary>
    public class DerivedCollection<T> : Derived
    {
        /// <summary>
        /// Construct a DerivedCollection that derives the value of target, using function.
        /// The dependencies (i.e., the inputs to function) must be provided as dependencies.
        /// 
        /// Note that this class limits change propogation by only changing the output 
        /// collection when necessary (i.e., if the function returns the same list of 
        /// items as already in the collection, the collection remains unchanged).
        /// </summary>
        /// <param name="target">The collection to derive.</param>
        /// <param name="function">Called to determine the items that should be in the target
        /// collection.  Called whenever any dependency changes.</param>
        /// <param name="dependencies">List of dependencies.</param>
        public DerivedCollection(ICollection<T> target, Func<IEnumerable<T>> function, params object[] dependencies)
            : base(dependencies)
        {
            _target = target;
            _function = function;
            Recompute();
        }

        /// <summary>
        /// This protected constructor is used when the _function will be set by a subclass.
        /// </summary>
        protected DerivedCollection(ICollection<T> target, params object[] dependencies)
            : base(dependencies)
        {
            _target = target;
        }

        private readonly ICollection<T> _target;
        protected Func<IEnumerable<T>> _function;

        public override void Recompute()
        {
            var newListMembers = _function();
            _target.UpdateItems(newListMembers);
        }
    }

    /// <summary>
    /// The CollectionConverter creates a one-to-one mapping from a collection of Tinput
    /// items to a collection of Ttarget items.
    /// </summary>
    public class CollectionConverter<Tinput, Ttarget> : Derived
    {
        /// <summary>
        /// Construct a CollectionConverter that updates the target collection based on
        /// the input collection, using the converterFunction to create new target 
        /// items for each input items.
        /// </summary>
        /// <param name="input">The collection that is to be converted.</param>
        /// <param name="target">The generated collection, which will have one member
        /// for each input item.</param>
        /// <param name="converterFunction">The function to call that will 
        /// generate a target item given an input item.</param>
        public CollectionConverter(ICollection<Ttarget> target, Func<Tinput, Ttarget> converterFunction,
                                   ICollection<Tinput> input)
            : base(input)
        {
            _target = target;
            _input = input;
            _converterFunction = converterFunction;
            Recompute();
        }

        private readonly ICollection<Ttarget> _target;
        private readonly ICollection<Tinput> _input;
        private readonly Func<Tinput, Ttarget> _converterFunction;

        public override void Recompute()
        {
            var newListMembers = _input.Select(e => _converterFunction(e));
            _target.UpdateItems(newListMembers);
        }
    }

    /// <summary>
    /// Derive a set from two input sets given the binary set function that computes the result.
    /// </summary>
    public class BinarySetFunction<T> : DerivedCollection<T>
    {
        /// <summary>
        /// Construct a BinarySetFunction given the left and right operand sets, the target, and
        /// the function to perform.
        /// </summary>
        public BinarySetFunction(ICollection<T> target, Func<ICollection<T>, ICollection<T>,
                                                            IEnumerable<T>> binarySetFunction,
                                 IObservableCollection<T> left, IObservableCollection<T> right) :
                                     base(target, left, right)
        {
            _left = left;
            _right = right;
            _function = () => binarySetFunction(_left, _right);
            Recompute();
        }

        private readonly IObservableCollection<T> _left;
        private readonly IObservableCollection<T> _right;
    }

    public class BinarySetSubtract<T> : BinarySetFunction<T>
    {
        public BinarySetSubtract(ICollection<T> target, IObservableCollection<T> left,
                                 IObservableCollection<T> right) :
                                     base(target, (l, r) => l.Where(e => !r.Contains(e)), left, right)
        {
        }
    }
}