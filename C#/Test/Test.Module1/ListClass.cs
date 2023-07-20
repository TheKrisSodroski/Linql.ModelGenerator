using System;
using System.Collections.Generic;
using System.Text;

namespace Test.Module1
{
    public class ListClass 
    {
        public List<int> IntList = new List<int>();

        public int[] IntArray { get; set; }

        public ICollection<int> IntCollection { get; set; }

        public IEnumerable<int> IntEnumerable { get; set; }

        public Dictionary<int, int> IntDictionary { get;set; }
    }
}
