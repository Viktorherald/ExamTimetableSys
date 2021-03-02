using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using System.IO;
using System.Threading;

namespace mainFYP
{
    static class MainProgram
    {
        static Random rnd = new Random();
        static Variable varCollection = new Variable();
        static SolutionManager solMgr = new SolutionManager(varCollection);
        static HardConstraint hardConstraintMgr;
        static SoftConstraint softConstraintMgr;

        static Variable tempVarCollection;
        static SolutionManager tempSolMgr;
        static HardConstraint tempHardConstraint;
        static SoftConstraint tempSoftConstraint;

        static StreamWriter plotWriter = new StreamWriter("C:\\Users\\ViktorHerald\\Desktop\\FYP\\smallFYP\\Data Output\\Final Report purpose\\A97474_T4.2055_L1_G100(7).txt");
        static StreamWriter softPlotWrter = new StreamWriter("C:\\Users\\ViktorHerald\\Desktop\\FYP\\smallFYP\\plotDataSoftA90.txt");

        static int counter, counter2;
        static void Main(string[] args)
        {
            InitializationStep();

            hardConstraintMgr = new HardConstraint(solMgr, varCollection);
            softConstraintMgr = new SoftConstraint(solMgr);
            Console.WriteLine("\nTotal violation on H2: {0}", hardConstraintMgr.H2Violation);
            Console.WriteLine("Total violation on H3: {0}", hardConstraintMgr.H3Violation);
            Console.WriteLine("Total violation on H9: {0}", hardConstraintMgr.H9Violation);
            Console.WriteLine("Total Penalty: {0}", softConstraintMgr.TotalS2Penalty);

            foreach (var s in solMgr.ListStudentBatch)
            {
                Console.Write("{0}: ", s.indexNo);
                foreach (var d in s.InvolvedDays)
                    Console.Write("{0} ", d);

                Console.WriteLine();
            }          

            Console.ReadLine();

            int k = 1;
            counter = 1; counter2 = 1;
            double currentTemp, temp1, alpha1;

            temp1 = 29.24;
            currentTemp = temp1;
            alpha1 = 0.956022;

            int origTotalConstraint, newTotalConstraint, diffConstraint;

            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            Console.WriteLine("\nAlgorithm starts here.");

            //while (hardConstraintMgr.TotalHardViolation > 0)
            //{
            //    for (int i = 0; i < 1; i++) //length of plateau
            //    {
            //        Period selectedPeriod;
            //        Tuple<int, int, int> d1_p1;
            //        List<Tuple<int, int, int, int>> selectedExams;

            //        d1_p1 = SearchPeriodWithHardViolation(rnd, hardConstraintMgr);
            //        selectedExams = GetRelevantTuple(d1_p1.Item1, d1_p1.Item2, solMgr, varCollection);
            //        selectedPeriod = solMgr.ListD[d1_p1.Item1 - 1].PeriodList[d1_p1.Item2 - 1];

            //        Tuple<int, int> d2_p2 = ChoosePeriodtoSwap(d1_p1, rnd);

            //        selectedExams = GetRelevantTuple(d1_p1.Item1, d1_p1.Item2, solMgr, varCollection);
            //        for (int j = 0; j < selectedExams.Count(); j++)
            //        {
            //            SettingUpNeighborSolution(hardConstraintMgr, softConstraintMgr);

            //            origTotalConstraint = hardConstraintMgr.TotalHardViolation;

            //            PerformSwap(selectedExams, d2_p2.Item1 - 1, d2_p2.Item2 - 1, j);
            //            hardConstraintMgr.UpdateConstraints(d1_p1.Item1, d1_p1.Item2, d2_p2.Item1, d2_p2.Item2, solMgr);

            //            newTotalConstraint = hardConstraintMgr.TotalHardViolation;

            //            diffConstraint = newTotalConstraint - origTotalConstraint;
            //            if (diffConstraint > 0)
            //            {
            //                RevertChanges();
            //            }
            //            else
            //            {
            //                j--;
            //            }
            //            RecordPhaseOneData(hardConstraintMgr.TotalHardViolation);
            //            selectedExams = GetRelevantTuple(d1_p1.Item1, d1_p1.Item2, solMgr, varCollection);

            //            if (hardConstraintMgr.TotalHardViolation == 0)
            //                break;
            //        }
            //        if (hardConstraintMgr.TotalHardViolation == 0)
            //            break;
            //    }
            //    k += 1;
            //    temp1 = temp1 * alpha1;
            //    Console.WriteLine("Plateau: {0}  Total Violated: {1}", k, hardConstraintMgr.TotalHardViolation);
            //}
            //plotWriter.Close();
            //PrintTimetable(solMgr);
            //Console.WriteLine("\nTotal violation on H2: {0}", hardConstraintMgr.H2Violation);
            //Console.WriteLine("Total violation on H3: {0}", hardConstraintMgr.H3Violation);
            //Console.WriteLine("Total violation on H9: {0}", hardConstraintMgr.H9Violation);

            //stopwatch.Stop();
            //Console.WriteLine("Time Elapsed: {0}", stopwatch.Elapsed);

            while (hardConstraintMgr.TotalHardViolation > 0)
            {
                for (int i = 0; i < 1; i++) //length of plateau
                {
                    Period selectedPeriod;
                    Tuple<int, int, int> d1_p1;
                    List<Tuple<int, int, int, int>> selectedExams;

                    d1_p1 = SearchPeriodWithHardViolation(rnd, hardConstraintMgr);
                    selectedExams = GetRelevantTuple(d1_p1.Item1, d1_p1.Item2, solMgr, varCollection);
                    selectedPeriod = solMgr.listD[d1_p1.Item1 - 1].PeriodList[d1_p1.Item2 - 1];

                    Tuple<int, int> d2_p2 = ChoosePeriodtoSwap(d1_p1, rnd);
                    selectedExams = GetRelevantTuple(d1_p1.Item1, d1_p1.Item2, solMgr, varCollection);
                    for (int j = 0; j < selectedExams.Count(); j++)
                    {
                        SettingUpNeighborSolution(hardConstraintMgr, softConstraintMgr);

                        origTotalConstraint = hardConstraintMgr.TotalHardViolation;

                        PerformSwap(selectedExams, d2_p2.Item1 - 1, d2_p2.Item2 - 1, j);
                        hardConstraintMgr.UpdateConstraints(d1_p1.Item1, d1_p1.Item2, d2_p2.Item1, d2_p2.Item2, solMgr);

                        newTotalConstraint = hardConstraintMgr.TotalHardViolation;

                        diffConstraint = newTotalConstraint - origTotalConstraint;
                        if (diffConstraint > 0)
                        {
                            double r = rnd.NextDouble();
                            if (r >= Math.Exp(-(diffConstraint / temp1)))
                            {
                                RevertChanges();
                            }
                        }
                        else
                        {
                            j--;
                        }
                        RecordPhaseOneData(hardConstraintMgr.TotalHardViolation);
                        selectedExams = GetRelevantTuple(d1_p1.Item1, d1_p1.Item2, solMgr, varCollection);

                        if (hardConstraintMgr.TotalHardViolation == 0)
                            break;
                    }
                    if (hardConstraintMgr.TotalHardViolation == 0)
                        break;
                }
                k += 1;
                //if (k < 50)
                //{
                //    temp1 = temp1 * 0.9862327;
                //}
                //else
                //{
                //    temp1 = temp1 * alpha1;
                //}
                temp1 = temp1 * alpha1;
                Console.WriteLine("Plateau: {0}     Temp: {1}   Total Violated: {2}", k, temp1, hardConstraintMgr.TotalHardViolation);
            }
            plotWriter.Close();
            PrintTimetable(solMgr);
            Console.WriteLine("\nTotal violation on H2: {0}", hardConstraintMgr.H2Violation);
            Console.WriteLine("Total violation on H3: {0}", hardConstraintMgr.H3Violation);
            Console.WriteLine("Total violation on H9: {0}", hardConstraintMgr.H9Violation);

            stopwatch.Stop();
            Console.WriteLine("Time Elapsed: {0}", stopwatch.Elapsed);
            //foreach (var s in solMgr.ListStudentBatch)
            //{
            //    Console.Write("{0}: ", s.indexNo);
            //    foreach (var d in s.InvolvedDays)
            //        Console.Write("{0} ", d);

            //    Console.WriteLine();
            //}
            //foreach (var d in solMgr.ListD)
            //{
            //    foreach (var p in d.PeriodList)
            //    {
            //        foreach (var v in p.VenueList)
            //        {
            //            Console.WriteLine("{0}: {1}", v.Description, v.RemainingCapacity);
            //        }
            //    }
            //}
            Console.ReadLine();

            softConstraintMgr.UpdateSoftConstraint(solMgr);
            Console.WriteLine("Total S2 penalty: {0}", softConstraintMgr.TotalS2Penalty);

            k = 1;
            counter = 1;
            double temp2, alpha2;

            temp2 = 29.24;
            currentTemp = temp2;
            alpha2 = 0.956022; 

            int origSoftConstraint, newSoftConstraint, softConstraintDiff;

            Stopwatch stopwatch2 = new Stopwatch();
            stopwatch2.Start();

            while (counter2 < 150000)
            {
                for (int i = 0; i < 1; i++)
                {
                    List<int> periodArray = new List<int>();
                    for (int j = 0; j < 27; j++)
                        periodArray.Add(j);

                    Shuffle(rnd, periodArray);
                    int p1, p2;
                    while (periodArray.Count() >= 25)
                    {
                        p1 = periodArray[0];
                        periodArray.Remove(p1);

                        for (int x = 0; x < 6; x++)
                        {
                            p2 = periodArray[x];
                            periodArray.Remove(p1);

                            Period selectedPeriod = solMgr.listD[p1 / 3].PeriodList[p1 % 3];
                            int counter = selectedPeriod.ExamCount;
                            for (int j = 0; j < 10; j++)
                            {
                                List<Tuple<int, int, int, int>> selectedExams;
                                SettingUpNeighborSolution(hardConstraintMgr, softConstraintMgr);

                                origTotalConstraint = hardConstraintMgr.TotalHardViolation;
                                origSoftConstraint = softConstraintMgr.TotalS2Penalty;

                                selectedExams = GetRelevantTuple((p1 / 3) + 1, (p1 % 3) + 1, solMgr, varCollection);
                                PerformSwap(selectedExams, (p2 / 3), (p2 % 3), j);
                                hardConstraintMgr.UpdateConstraints((p1 / 3) + 1, (p1 % 3) + 1, (p2 / 3) + 1, (p2 % 3) + 1, solMgr);
                                softConstraintMgr.UpdateSoftConstraint(solMgr);

                                newTotalConstraint = hardConstraintMgr.TotalHardViolation;
                                newSoftConstraint = softConstraintMgr.TotalS2Penalty;


                                diffConstraint = newTotalConstraint - origTotalConstraint;
                                softConstraintDiff = newSoftConstraint - origSoftConstraint;

                                if (diffConstraint > 0)
                                    RevertChanges();
                                else if (softConstraintDiff > 0)
                                {
                                    double r = rnd.NextDouble();
                                    if (r >= Math.Exp(-(softConstraintDiff / temp2)))
                                    {
                                        RevertChanges();
                                    }
                                }
                                RecordPhaseTwoData(hardConstraintMgr.TotalHardViolation, softConstraintMgr.TotalS2Penalty);
                            }
                        }
                    }
                }
                k += 1;
                temp2 = temp2 * alpha2;
                Console.WriteLine("Plateau: {0}  Temp: {1}   Total Violated: {2}   Penalty: {3}", k, temp2, hardConstraintMgr.TotalHardViolation,
                                                                                                    softConstraintMgr.TotalS2Penalty);

            }
            softPlotWrter.Close();
            Console.WriteLine("\nTotal violation on H2: {0}", hardConstraintMgr.H2Violation);
            Console.WriteLine("Total violation on H3: {0}", hardConstraintMgr.H3Violation);
            Console.WriteLine("Total violation on H9: {0}", hardConstraintMgr.H9Violation);
            Console.WriteLine("Total Penalty: {0}", softConstraintMgr.TotalS2Penalty);

            stopwatch.Stop();
            Console.WriteLine("Time Elapsed: {0}", stopwatch.Elapsed);
            Console.ReadLine();
        }

        public static void InitializationStep()
        {
            InitializeRandomTimetable(solMgr, rnd);
            PrintTimetable(solMgr);
        }
        public static void PrintTimetable(SolutionManager solMgr)
        {
            foreach (var day in solMgr.ListD)
            {
                foreach (var period in day.PeriodList)
                {
                    Console.WriteLine("Day {0}, Period {1}", day.Index, period.Session);
                    Console.WriteLine("===========================");
                    foreach (var venue in period.VenueList)
                    {
                        if (venue.RemainingCapacity < venue.Capacity)
                        {
                            foreach (var programme in venue.Programme)
                            {
                                Console.Write("{0}: ({1}, {2})", venue.Description, programme.ProgrammeCode, programme.TotalEnrollment);
                                foreach (var s in programme.Pcs)
                                {
                                    Console.Write(s + ", ");
                                }
                                Console.Write("\n");
                            }
                        }
                        if (venue.RemainingCapacity < 0)
                            Console.WriteLine("^^^^^^^^^");
                    }
                }
            }
        }
        public static void InitializeRandomTimetable(SolutionManager solMgr, Random rnd)
        {
            foreach (var x in solMgr.ListProg)
            {
                solMgr.CreateTuple(rnd.Next(0, 9), x, rnd.Next(0, 3), rnd.Next(0, 68));
            }
        }
        public static List<Tuple<int, int, int, int>> GetRelevantTuple(int d, int p, SolutionManager solMgr, Variable varCollection)
        {
            return varCollection.X_depv.FindAll(x => x.Item1 == d && x.Item3 == p);
        }

        public static Tuple<int, int, int> SearchPeriodWithHardViolation(Random rnd, HardConstraint constraintMgr)
        {
            while (true)
            {
                int randomInt = rnd.Next(0, 27);
                if (constraintMgr.PeriodH2Violation[randomInt].Item3 != 0)
                    return constraintMgr.PeriodH2Violation[randomInt];

                else if (constraintMgr.PeriodH3Violation[randomInt].Item3 != 0)
                    return constraintMgr.PeriodH3Violation[randomInt];

                else if (constraintMgr.PeriodH9Violation[randomInt].Item3 != 0)
                    return constraintMgr.PeriodH9Violation[randomInt];
            }
        }
        public static void Shuffle(List<Tuple<int, int, int, int>> selectedExams, Random rng)
        {
            int n = selectedExams.Count;
            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);
                var temp = selectedExams[k];
                selectedExams[k] = selectedExams[n];
                selectedExams[n] = temp;
            }
        }
        public static Tuple<int, int> ChoosePeriodtoSwap(Tuple<int, int, int> tuple, Random rnd)
        {
            int d, p;
            d = rnd.Next(1, 10); p = rnd.Next(1, 4);
            while (d == tuple.Item1 && p == tuple.Item2)
            {
                d = rnd.Next(1, 10); p = rnd.Next(1, 4);
            }
            return new Tuple<int, int>(d, p);
        }

        public static Tuple<int, int, int, int> FindVariable(int e, SolutionManager solMgr, Variable variablesCollection)
        {
            var listProg = solMgr.ListProg;

            var x_depv_tuple = variablesCollection.X_depv.Find(y => y.Item2 == e);
            int d = x_depv_tuple.Item1, p = x_depv_tuple.Item3, v = x_depv_tuple.Item4;

            return Tuple.Create(d, e, p, v);
        }

        public static void PerformSwap(List<Tuple<int, int, int, int>> selectedEs, int d_2, int p_2, int r)
        {
            int d, e, p, v;

            d = selectedEs[r].Item1 - 1;
            e = selectedEs[r].Item2;
            p = selectedEs[r].Item3 - 1;
            v = selectedEs[r].Item4 - 1;

            solMgr.SwapTuple(d, e, p, v, d_2, p_2, rnd);
        }

        public static void SettingUpNeighborSolution(HardConstraint hardConstraint, SoftConstraint softConstraint)
        {
            tempVarCollection = new Variable(varCollection);
            tempSolMgr = new SolutionManager(solMgr, tempVarCollection);
            tempHardConstraint = new HardConstraint(hardConstraint);
            tempSoftConstraint = new SoftConstraint(softConstraint);
        }
        public static void RevertChanges()
        {
            varCollection = tempVarCollection;
            solMgr = tempSolMgr;
            hardConstraintMgr = tempHardConstraint;
            softConstraintMgr = tempSoftConstraint;
        }

        public static void RecordPhaseOneData(int currentTotalConstraint)
        {
            plotWriter.WriteLine("{0}, {1}", counter, currentTotalConstraint);
            counter++;
        }
        public static void RecordPhaseTwoData(int currentHardConstraint, int currentSoftConstraint)
        {
            softPlotWrter.WriteLine("{0}, {1}, {2}", counter2, currentHardConstraint, currentSoftConstraint);
            counter2++;
        }
        public static void Shuffle(this Random rng, List<int> array)
        {
            int n = array.Count();
            while (n > 1)
            {
                int k = rng.Next(n--);
                int temp = array[n];
                array[n] = array[k];
                array[k] = temp;
            }
        }
    }
}
