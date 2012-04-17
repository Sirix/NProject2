using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NProject.Web.Helpers
{
    //public class MultipleKeyDictionary<TKey, TEntry> : IEnumerable<KeyValuePair<TKey, List<TEntry>>>
    //{
    //    private List<KeyValuePair<TKey, List<TEntry>>> data = new List<KeyValuePair<TKey, List<TEntry>>>();

    //    public void Add(TKey key, TEntry entry)
    //    {
    //        var dataItem = data.FirstOrDefault(i => i.Key.Equals(key));
    //        //we have no value
    //        if (dataItem.Equals(default(KeyValuePair<TKey, List<TEntry>>)))
    //        {
    //            List<TEntry> items = new List<TEntry>() {entry};
    //            data.Add(new KeyValuePair<TKey, List<TEntry>>(key, items));
    //        }
    //        else
    //        {
    //            dataItem.Value.Add(entry);
    //        }
    //    }

    //    IEnumerator<KeyValuePair<TKey, List<TEntry>>> IEnumerable<KeyValuePair<TKey, List<TEntry>>>.GetEnumerator()
    //    {
    //        throw new NotImplementedException();
    //    }
    //}
}