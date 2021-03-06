using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;

namespace CMScoutIntrinsic {

    class ListCollectionViewSource<T> {
        public ListCollectionViewSource(IList<T> source, Predicate<T> filter, Comparison<T> comparison) {
            Source     = source;
            Filter     = filter;
            Comparison = comparison;
            View       = new ObservableCollectionEx<T>();

            Refresh();

            if(Source is INotifyCollectionChanged) {
                ((INotifyCollectionChanged)Source).CollectionChanged += OnSourceCollectionChanged;
            }
        }


        public IList<T>                  Source     { get; }
        public Predicate<T>              Filter     { get; }
        public Comparison<T>             Comparison { get; }
        public ObservableCollectionEx<T> View       { get; }



        public void Refresh() {
            List<T> newView = Source.Where(item => Filter == null || Filter(item)).ToList();

            if(Comparison != null) {
                newView.Sort(Comparison);
            }

            View.Reset(newView);
        }

        public void Resort() {
            List<T> newView = new List<T>(View);

            newView.Sort(Comparison);

            View.Reset(newView);
        }

        private void OnSourceCollectionChanged(Object sender, NotifyCollectionChangedEventArgs args) {
            if(args.Action == NotifyCollectionChangedAction.Reset) {
                Refresh();
            }
            else if(args.Action == NotifyCollectionChangedAction.Remove) {
                if(args.OldItems != null) {
                    foreach(T item in args.OldItems) {
                        View.Remove(item);
                    }
                }
            }
            else if(args.Action == NotifyCollectionChangedAction.Add) {
                if(args.NewItems != null) {
                    foreach(T item in args.NewItems) {
                        if(Filter == null || Filter(item)) {
                            if(Comparison == null) {
                                View.Add(item);
                            }
                            else {
                                Int32 i = 0;

                                for(; i < View.Count; ++i) {
                                    if(Comparison(item, View[i]) < 0) {
                                        break;
                                    }
                                }

                                View.Insert(i, item);
                            }
                        }
                    }
                }
            }
            else if(args.Action == NotifyCollectionChangedAction.Move) {
                throw new NotImplementedException();
            }
            else if(args.Action == NotifyCollectionChangedAction.Replace) {
                throw new NotImplementedException();
            }
            else {
                throw new Exception(String.Format("ListCollectionViewSource.OnSourceCollectionChanged: unknown action {0}", args.Action));
            }
        }
    }

}
