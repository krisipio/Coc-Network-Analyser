using System;

namespace ClashRoyale_NetworkAnalyser.Utils
{
    using System.Collections.ObjectModel;
    using System.Collections.Specialized;
    using System.Windows.Threading;

    public class MTObservableCollection<T> : NObservableCollection<T>
    {
        public override event NotifyCollectionChangedEventHandler CollectionChanged;

        protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            NotifyCollectionChangedEventHandler CollectionChanged = this.CollectionChanged;
            try
            {
                if (CollectionChanged != null)
                    foreach (NotifyCollectionChangedEventHandler nh in CollectionChanged.GetInvocationList())
                    {
                        DispatcherObject dispObj = nh.Target as DispatcherObject;
                        if (dispObj != null)
                        {
                            Dispatcher dispatcher = dispObj.Dispatcher;
                            if (dispatcher != null && !dispatcher.CheckAccess())
                            {
                                dispatcher.BeginInvoke(
                                    (Action)(() => nh.Invoke(this,
                                        new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset))),
                                    DispatcherPriority.DataBind);
                                continue;
                            }
                        }
                        nh.Invoke(this, e);
                    }
            }
            catch (Exception)
            {
                
            }
            
        }

        public void Update()
        {
            NotifyCollectionChangedEventHandler CollectionChanged = this.CollectionChanged;
            try
            {
                if (CollectionChanged != null)
                    foreach (NotifyCollectionChangedEventHandler nh in CollectionChanged.GetInvocationList())
                    {
                        DispatcherObject dispObj = nh.Target as DispatcherObject;
                        if (dispObj != null)
                        {
                            Dispatcher dispatcher = dispObj.Dispatcher;
                            if (dispatcher != null && !dispatcher.CheckAccess())
                            {
                                dispatcher.BeginInvoke(
                                    (Action) (() => nh.Invoke(this,
                                        new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset))),
                                    DispatcherPriority.DataBind);
                                continue;
                            }
                        }
                        nh.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
                    }
            }
            catch (Exception)
            {

            }
        }
    }
}