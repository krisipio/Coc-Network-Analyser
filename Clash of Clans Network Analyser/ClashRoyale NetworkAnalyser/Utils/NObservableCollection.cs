using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace ClashRoyale_NetworkAnalyser.Utils
{
    public class NObservableCollection<T> : ObservableCollection<T>
    {
        protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            try
            {
                base.OnCollectionChanged(e);
            }
            catch (Exception)
            {
                
            }
            
        }
    }
}
