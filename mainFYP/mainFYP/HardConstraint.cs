using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mainFYP
{
    class HardConstraint
    {
        int totalH2Violation;
        List<Tuple<int, int, int>> periodH2Violation; //(d index, p index, #H2 violation in that d & p)

        int totalH3Violation;
        List<Tuple<int, int, int>> periodH3Violation;

        int totalH9Violation;
        List<Tuple<int, int, int>> periodH9Violation;

        int totalHardViolation;
        List<Tuple<int, int, int>> periodHardViolation;

        int total_S1_Penalty;

        public HardConstraint(SolutionManager solMgr, Variable varCol)
        {
            totalH2Violation = CalculateTotalH2Violation(solMgr);
            periodH2Violation = CalculatePeriodH2Violation(solMgr);

            periodH3Violation = new List<Tuple<int, int, int>>();
            totalH3Violation = CalculateTotalH3Violation(solMgr);

            totalH9Violation = CalculateTotalH9Violation(solMgr);
            periodH9Violation = CalculatePeriodH9Violation(solMgr);
            
            totalHardViolation = CalculateTotalViolation();
            periodHardViolation = CalculatePeriodTotalHardViolation();

        }

        public HardConstraint(HardConstraint copyClass)
        {
            this.totalH2Violation = copyClass.totalH2Violation;
            this.periodH2Violation = new List<Tuple<int, int, int>>();
            foreach (var c in copyClass.periodH2Violation)
                this.periodH2Violation.Add(c);

            this.totalH3Violation = copyClass.totalH3Violation;
            this.periodH3Violation = new List<Tuple<int, int, int>>();
            foreach (var c in copyClass.periodH3Violation)
                this.periodH3Violation.Add(c);

            this.totalH9Violation = copyClass.totalH9Violation;
            this.periodH9Violation = new List<Tuple<int, int, int>>();
            foreach (var c in copyClass.periodH9Violation)
                this.periodH9Violation.Add(c);

            this.totalHardViolation = copyClass.totalHardViolation;
            this.periodHardViolation = new List<Tuple<int, int, int>>();
            foreach (var c in copyClass.periodHardViolation)
                this.periodHardViolation.Add(c);
        }

        public int H2Violation
        {
            get { return totalH2Violation; }
        }
        public List<Tuple<int, int, int>> PeriodH2Violation
        {
            get { return periodH2Violation; }
        }
        public int H3Violation
        {
            get { return totalH3Violation; }
        }
        public List<Tuple<int, int, int>> PeriodH3Violation
        {
            get { return periodH3Violation; }
        }
        public int H9Violation
        {
            get { return totalH9Violation; }
        }
        public List<Tuple<int, int, int>> PeriodH9Violation
        {
            get { return periodH9Violation; }
        }
        public int TotalHardViolation
        {
            get { return totalHardViolation; }
        }
        public List<Tuple<int, int, int>> PeriodHardViolation
        {
            get { return periodHardViolation; }
        }
        public int CalculateTotalH2Violation(SolutionManager solMgr)
        {
            int counter = 0;
            foreach (var d in solMgr.ListD)
            {
                foreach (var p in d.PeriodList)
                {
                    foreach (var s_1 in p.PcsList) //get a particular student batch in a day D, period P
                    {
                        foreach (var v in p.VenueList)
                        {
                            if (v.Capacity == v.RemainingCapacity) //venue V is not assigned for any exams
                                continue;
                            else foreach (var e in v.Programme)
                                {
                                    foreach (var s in e.Pcs)
                                    {
                                        if (s == s_1) // if the selected student is in the exam subject E
                                            counter++;
                                    }
                                }
                        }
                    }
                    counter -= p.PcsList.Count();
                    //p.PcsList indicates the student batches in each period. By right, every s should only appear once in each period. Hence,
                    //ideally if every s only appear once in all the e's in each p, the counter will be 0 after deduction. Should a s appear more
                    //than once, then counter will be > 0, indicating there is violation.
                }
            }
            return counter;
        }
        public List<Tuple<int, int, int>> CalculatePeriodH2Violation(SolutionManager solMgr)
        {
            List<Tuple<int, int, int>> violationInEachPeriod = new List<Tuple<int, int, int>>();

            int counter;
            foreach (var d in solMgr.ListD)
            {
                foreach (var p in d.PeriodList)
                {
                    counter = 0;
                    foreach (var s_1 in p.PcsList) //get a particular student batch in a day D, period P
                    {
                        foreach (var v in p.VenueList)
                        {
                            if (v.Capacity == v.RemainingCapacity) //venue V is not assigned for any exams
                                continue;
                            else foreach (var e in v.Programme)
                                {
                                    foreach (var s in e.Pcs)
                                    {
                                        if (s == s_1) // if the selected student is in the exam subject E
                                            counter++;
                                    }
                                }
                        }
                    }
                    counter -= p.PcsList.Count();
                    violationInEachPeriod.Add(new Tuple<int, int, int>(d.Index, p.Index, counter));
                }
            }
            return violationInEachPeriod;
        }
        public void UpdatePeriodH2Violation(int d, int p, SolutionManager solMgr)
        {
            var currentIndex = periodH2Violation.FindIndex(x => x.Item1 == d && x.Item2 == p);
            var currentTuple = periodH2Violation.Find(x => x.Item1 == d && x.Item2 == p);
            int originalCount = currentTuple.Item3;

            int counter = 0;
            Period selectedPeriod = solMgr.ListD[d - 1].PeriodList[p - 1];

            foreach (var s_1 in selectedPeriod.PcsList) //get a particular student batch in a day D, period P
            {
                foreach (var v in selectedPeriod.VenueList)
                {
                    if (v.Capacity == v.RemainingCapacity) //venue V is not assigned for any exams
                        continue;
                    else foreach (var e in v.Programme)
                        {
                            foreach (var s in e.Pcs)
                            {
                                if (s == s_1) // if the selected student is in the exam subject E
                                    counter++;
                            }
                        }
                }
            }
            counter -= selectedPeriod.PcsList.Count();

            periodH2Violation[currentIndex] = new Tuple<int, int, int>(d, p, counter);
            totalH2Violation += (counter - originalCount);
            totalHardViolation += (counter - originalCount);
        }
        public int CalculateTotalH3Violation(SolutionManager solMgr)
        {
            int sum = 0;
            int periodCounter;
            foreach (var d in solMgr.ListD)
            {
                foreach (var p in d.PeriodList)
                {
                    periodCounter = 0;
                    foreach (var v in p.VenueList)
                    {
                        if (v.RemainingCapacity < 0)
                        {
                            sum++;
                            periodCounter++;
                        }
                    }
                    this.PeriodH3Violation.Add(new Tuple<int, int, int>(d.Index, p.Index, periodCounter));
                }
            }
            return sum;
        }
        public List<Tuple<int, int, int>> CalculatePeriodH3Violation(SolutionManager solMgr)
        {
            List<Tuple<int, int, int>> violationInEachPeriod = new List<Tuple<int, int, int>>();

            int counter;
            foreach (var d in solMgr.ListD)
            {
                foreach (var p in d.PeriodList)
                {
                    counter = 0;
                    foreach (var v in p.VenueList)
                    {
                        if (v.RemainingCapacity < 0)
                            counter++;
                    }
                    violationInEachPeriod.Add(new Tuple<int, int, int>(d.Index, p.Index, counter));
                }
            }
            return violationInEachPeriod;
        }
        public void UpdatePeriodH3Violation(int d, int p, SolutionManager solMgr)
        {
            int currentIndex = periodH3Violation.FindIndex(x => x.Item1 == d && x.Item2 == p);
            var currentTuple = periodH3Violation.Find(x => x.Item1 == d && x.Item2 == p);
            int originalCount = currentTuple.Item3;

            int counter = 0;
            Period selectedPeriod = solMgr.ListD[d - 1].PeriodList[p - 1];

            foreach (var v in selectedPeriod.VenueList)
            {
                if (v.RemainingCapacity < 0)
                    counter++;
            }

            periodH3Violation[currentIndex] = new Tuple<int, int, int>(d, p, counter);
            totalH3Violation += (counter - originalCount);
            totalHardViolation += (counter - originalCount);
        }

        public int CalculateTotalH9Violation(SolutionManager solMgr)
        {
            int counter = 0;
            foreach (var d in solMgr.ListD)
            {
                foreach (var p in d.PeriodList)
                {
                    foreach (var v in p.VenueList)
                    {
                        if (v.Programme.Count() > 15)
                            counter++;
                    }
                }
            }
            return counter;
        }
        public List<Tuple<int, int, int>> CalculatePeriodH9Violation(SolutionManager solMgr)
        {
            List<Tuple<int, int, int>> violationInEachPeriod = new List<Tuple<int, int, int>>();

            int counter;
            foreach (var d in solMgr.ListD)
            {
                foreach (var p in d.PeriodList)
                {
                    counter = 0;
                    foreach (var v in p.VenueList)
                    {
                        if (v.Programme.Count() > 15)
                            counter++;
                    }
                    violationInEachPeriod.Add(new Tuple<int, int, int>(d.Index, p.Index, counter));
                }
            }
            return violationInEachPeriod;
        }
        public void UpdatePeriodH9Violation(int d, int p, SolutionManager solMgr)
        {
            int currentIndex = periodH9Violation.FindIndex(x => x.Item1 == d && x.Item2 == p);
            var currentTuple = periodH9Violation.Find(x => x.Item1 == d && x.Item2 == p);
            int originalCount = currentTuple.Item3;

            int counter = 0;
            Period selectedPeriod = solMgr.ListD[d - 1].PeriodList[p - 1];

            foreach (var v in selectedPeriod.VenueList)
            {
                if (v.Programme.Count() > 15)
                    counter++;
            }

            periodH9Violation[currentIndex] = new Tuple<int, int, int>(d, p, counter);
            totalH9Violation += (counter - originalCount);
            totalHardViolation += (counter - originalCount);
        }

        public int CalculateTotalViolation()
        {
            return totalH2Violation + totalH3Violation + totalH9Violation; 
        }
        public List<Tuple<int, int, int>> CalculatePeriodTotalHardViolation()
        {
            List<Tuple<int, int, int>> value = new List<Tuple<int, int, int>>();

            for (int i = 0; i < periodH2Violation.Count(); i++)
            {
                value.Add(new Tuple<int, int, int>(periodH2Violation[i].Item1, periodH2Violation[i].Item2,
                    periodH2Violation[i].Item3 + periodH3Violation[i].Item3 + periodH9Violation[i].Item3));
            }
            return value;
        }

        public void UpdateConstraints(int d_1, int p_1, int d_2, int p_2, SolutionManager solMgr)
        {
            UpdatePeriodH2Violation(d_1, p_1, solMgr);
            UpdatePeriodH2Violation(d_2, p_2, solMgr);
            UpdatePeriodH3Violation(d_1, p_1, solMgr);
            UpdatePeriodH3Violation(d_2, p_2, solMgr);
            UpdatePeriodH9Violation(d_1, p_1, solMgr);
            UpdatePeriodH9Violation(d_2, p_2, solMgr);
        }
    }
}
