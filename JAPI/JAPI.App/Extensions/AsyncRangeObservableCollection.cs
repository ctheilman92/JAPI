using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace JAPI.App
{
    /// <summary>
    /// Introduces creating thread raised events when updating collection in separate thread. 
    /// also adds AddRange functionality
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class AsyncRangeObservableCollection<T> : ObservableCollection<T>
    {
        private SynchronizationContext syncContext = SynchronizationContext.Current;
        private bool _suppressNotification = false;

        //create asyncOperation to post events to thread which initialized teh colleciton (UI thread)
        public AsyncRangeObservableCollection() { }

        public AsyncRangeObservableCollection(IEnumerable<T> list) : base(list)
        {
        }

        protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            if (!_suppressNotification)
            {
                if (SynchronizationContext.Current == syncContext)
                {
                    // Execute the CollectionChanged event on the current thread
                    RaiseCollectionChanged(e);
                }
                else
                {
                    // Raises the CollectionChanged event on the creator thread
                    syncContext.Send(RaiseCollectionChanged, e);
                }
            }
        }

        private void RaiseCollectionChanged(object e)
            => base.OnCollectionChanged((NotifyCollectionChangedEventArgs)e);

        public void AddRange(IEnumerable<T> list)
        {
            if (list == null)
                throw new ArgumentNullException("list");

            _suppressNotification = true;
            foreach (T item in list)
            {
                Add(item);
            }
            _suppressNotification = false;
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }

        // Post the PropertyChanged event on the creator thread
        protected override void OnPropertyChanged(PropertyChangedEventArgs e)
            => syncContext.Send(RaisePropertyChanged, e);

        // We are in the creator thread, call the base implementation directly
        private void RaisePropertyChanged(object e)
            => base.OnPropertyChanged((PropertyChangedEventArgs)e);
    }

    /// <summary>
    /// this is an extension class of the async range observable collection - Since observable collection only updates binding source when insert or delete - this will update anytime an item changes 
    /// this is necessary for the data grid since we are polling objects - updating items in the bound collection
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class AsyncPropertyTrackingObservableCollection<T> : AsyncRangeObservableCollection<T> 
    {
        protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                RegisterPropertyChanged(e.NewItems);
            }
            else if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                UnregisterPropertyChanged(e.OldItems);
            }
            else if (e.Action == NotifyCollectionChangedAction.Replace)
            {
                UnregisterPropertyChanged(e.OldItems);
                RegisterPropertyChanged(e.NewItems);
            }
            base.OnCollectionChanged(e);
        }

        protected override void ClearItems()
            => base.ClearItems();
        
        private void RegisterPropertyChanged(IList items)
        {
            foreach (INotifyPropertyChanged item in items)
            {
                if (item != null)
                    item.PropertyChanged += new PropertyChangedEventHandler(item_PropertyChanged);
            }
        }

        private void UnregisterPropertyChanged(IList items)
        {
            foreach (INotifyPropertyChanged item in items)
            {
                if (item != null)
                    item.PropertyChanged -= new PropertyChangedEventHandler(item_PropertyChanged);
            }
        }

        private void item_PropertyChanged(object sender, PropertyChangedEventArgs e)
            => base.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
    }
}
