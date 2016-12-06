using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;

namespace Gongchengshi
{
    /// <summary>
    /// Property interface.
    /// This interface only exists to differentiate properties from collections when they
    /// are put in a list of dependencies.
    /// </summary>
    public interface IProperty : INotifyPropertyChanged
    {}

    /// <summary>
    /// A generic databindable Property.  This class exists for the following reasons:
    ///   * Eliminate string binding of property names.
    ///   * Support derived properties.
    /// 
    /// Usage:
    ///     Property&lt;int&gt; MyIntProperty = new Property&lt;int&gt;();
    ///     ...
    ///     MyIntProperty.Value = 5;
    /// </summary>
    public class Property<T> : IProperty
    {
        private static readonly bool _isClass = typeof(T).IsClass;

        public Property()
        {
            // This class does not need to be IDisposable because it does not register for events
            // on outside objects.  This is registering for its own event, and will get garbage
            // collected appropriately (because this object add the event will be an island).
            PropertyChanged += (s, e) => Changed();
        }

        public Property(T value)
            : this()
        {
            _Value = value;
        }

        public T Value
        {
            get { return _Value; }
            set
            {
                if (_isClass) // Avoids boxing when using ReferenceEquals or comparing with null
                {
                    if (ReferenceEquals(_Value, value))
                    {
                        return;
                    }

                    if (value == null || _Value == null)
                    {
                        _Value = value;
                        RaisePropertyChanged();
                        return;
                    }
                }

                if (!value.Equals(_Value))
                {
                    _Value = value;
                    RaisePropertyChanged();
                }
            }
        }
        T _Value;

        public void RaisePropertyChanged()
        {
            PropertyChanged(this, new PropertyChangedEventArgs(PropertyConstants.ValueName));
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public event Action Changed = delegate { };

        public override string ToString()
        {
            return _Value.ToString();
        }
    }

    /// <summary>
    /// Pulled into own class because these are not type T specific.
    /// </summary>
    public static class PropertyConstants
    {
        public const string ValueName = "Value";
    }

    /// <summary>
    /// An abstract base class for deriving properties or lists off other properties/lists.
    /// Subclass and implement Recompute() to create a derived object.
    /// </summary>    
    public abstract class Derived : IDisposable
    {
        protected Derived(params object[] dependencies)
        {
            foreach (var dependency in dependencies)
            {
                if (dependency is IProperty)
                {
                    var castedDependency = (IProperty) dependency;
                    castedDependency.PropertyChanged += Derived_PropertyChanged;
                    _disposables.Add(() => castedDependency.PropertyChanged -= Derived_PropertyChanged);
                }
                else if (dependency is INotifyCollectionChanged)
                {
                    var castedDependency = (INotifyCollectionChanged) dependency;
                    castedDependency.CollectionChanged += Derived_CollectionChanged;
                    _disposables.Add(() => castedDependency.CollectionChanged -= Derived_CollectionChanged);
                }
                else
                {
                    // In the future, it is possible to support INotifyPropertyChanged dependencies
                    // (i.e., non-IProperty objects).
                    // This feature was not yet needed, so support was not added.
                    throw new ArgumentException("Can't depend on something that doesn't notify of changes.");
                }
            }
        }
        
        private void Derived_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            Recompute();
        }

        private void Derived_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == PropertyConstants.ValueName)
                Recompute();            
        }

        public abstract void Recompute();

        public void Dispose()
        {
            _disposables.ForEach(d => d());
        }

        protected List<Action> _disposables = new List<Action>();
    }

    /// <summary>
    /// Derive a Property&lt;T&gt; from other properties/collections.
    /// </summary>
    public class DerivedProperty<T> : Derived
    {
        /// <summary>
        /// Construct a DerivedProperty that derives the value of property, using function.
        /// The dependencies (i.e., the inputs to function) must be provided as dependencies.
        /// </summary>
        /// <param name="property">The property to derive.</param>
        /// <param name="function">Called to determine the new value of property whenever any 
        /// dependency changes.</param>
        /// <param name="dependencies">List of dependencies.</param>
        public DerivedProperty(Property<T> property, Func<T> function, params object[] dependencies)
            : base(dependencies)
        {
            _property = property;
            _function = function;
            Recompute();
        }

        public override void Recompute()
        {
            _property.Value = _function();
        }

        private readonly Func<T> _function;
        private readonly Property<T> _property;
    }

    public class OnPropertyChanged : Derived
    {
        private readonly Action _action;

        public OnPropertyChanged(Action action, params object[] dependencies)
            : base(dependencies)
        {
            _action = action;
        }

        public override void Recompute()
        {
            _action();
        }
    }
}
