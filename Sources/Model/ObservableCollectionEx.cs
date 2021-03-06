using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace CMScoutIntrinsic {

    class ObservableCollectionEx<T> : ObservableCollection<T> {

        public void Reset(IEnumerable<T> newItems) {
            this.Items.Clear();

            foreach(T newItem in newItems) {
                this.Items.Add(newItem);
            }

            this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }

    }

}
