using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GD_CommonLibrary.Method
{
    public class LimitedSizeObservableCollection<T>:ObservableCollection<T>
    {
        public LimitedSizeObservableCollection() : base()
        {
            Capacity = int.MaxValue;
        }

        public LimitedSizeObservableCollection(int capacity):base()
        {
            Capacity = capacity;
        }
        public LimitedSizeObservableCollection(int capacity, IEnumerable<T> collection) : base(collection)
        {
            Capacity = capacity;
        }
        public LimitedSizeObservableCollection(int capacity, List<T> list) : base(list) 
        {
            Capacity = capacity;
        }


        public int Capacity { get; set; }

        protected override void InsertItem(int index, T item)
        {
            base.InsertItem(index, item);
            if (this.Items.Count > Capacity)
                base.RemoveAt(0);
        }
    }
}
