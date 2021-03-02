using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mainFYP
{
    class Day
    {
        int index;
        string description;
        List<Period> periodList;

        public int Index
        {
            get { return index; }
        }
        public List<Period> PeriodList
        {
            get { return periodList; } 
            set { periodList = value; }
        }
        public string Description
        {
            get { return description; }
        }

        public Day()
        {
            this.index = 0;
            this.periodList = new List<Period>();
        }
        public Day(int index)
        {
            this.index = index;
            this.periodList = new List<Period>();
        }
        public Day(int index, string description)
        {
            this.index = index;
            this.description = description;
            this.periodList = new List<Period>();
        }
        public Day(int index, List<Period> periodList)
        {
            this.index = index;
            this.periodList = periodList;
        }     
    }
}
