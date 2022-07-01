using System;
using System.Collections.Generic;

namespace TTRCalc
{
    class Program
    {
        private enum FacilityType { Sellbot, Cashbot, Lawbot, Bossbot };

        // Point names associated w/ facilities
        private static Dictionary<FacilityType, string> PointTypes = new Dictionary<FacilityType, string>()
        {
            {
                FacilityType.Sellbot,
                "Merits"
            },
            {
                FacilityType.Cashbot,
                "Cogbucks"
            },
            {
                FacilityType.Lawbot,
                "Jury Notices"
            },
            {
                FacilityType.Bossbot,
                "Stock Options"
            }
        };

        // Point values associated w/ facilities
        private static Dictionary<FacilityType, (string, uint, double)[]> FacilityValues = new Dictionary<FacilityType, (string, uint, double)[]>()
        {
            {
                FacilityType.Sellbot,
                new (string, uint, double)[] {
                    ("Long", 776, 3.0),
                    //("Medium", 584, 2.0), Commenting this out for now because who does medium factory runs lol
                    ("Short", 480, 1.0)
                }
            },
            {
                FacilityType.Cashbot,
                new (string, uint, double)[] {
                    ("Bullion Mint", 1202, 2.0),
                    ("Dollar Mint", 679, 1.5),
                    ("Coin Mint", 356, 1.0)
                }
            },
            {
                FacilityType.Lawbot,
                new (string, uint, double)[] {
                    ("D Office", 1842, 2.0),
                    ("C Office", 1370, 1.66),
                    ("B Office", 944, 1.33),
                    ("A Office", 564, 1.0)
                }
            },
            {
                FacilityType.Bossbot,
                new (string, uint, double)[] {
                    ("Back 9", 5120, 3.0),
                    ("Middle 6", 3020, 2.0),
                    ("Front 3", 1120, 1.0)
                }
            }
        };
        static void Main(string[] args)
        {
            Console.Title = "Sysroot's TTR Suit Point Calculator";
            while (true)
            {
                FacilityType SelectedFacility;
                string PointType;
                uint NumValue;
                do
                {
                    Console.Clear();
                    Console.WriteLine("Select facility type:\n");
                    Console.WriteLine("1) Sellbot\n2) Cashbot\n3) Lawbot\n4) Bossbot\n");
                } while (!uint.TryParse(Console.ReadKey().KeyChar.ToString(), out NumValue) && NumValue < 1 && NumValue > 4);
                SelectedFacility = (FacilityType)(NumValue - 1);
                PointType = PointTypes[SelectedFacility];
                uint PointsNeeded;
                do
                {
                    Console.Clear();
                    Console.WriteLine($"How many {PointType} do you need?\n");
                } while (!uint.TryParse(Console.ReadLine(), out PointsNeeded));
                uint CurrentPoints;
                do
                {
                    Console.Clear();
                    Console.WriteLine($"How many {PointType} do you have?\n");
                } while (!uint.TryParse(Console.ReadLine(), out CurrentPoints) || CurrentPoints > PointsNeeded);
                string Result = CalculateAndPrint(SelectedFacility, PointsNeeded - CurrentPoints);
                Console.Clear();
                Console.WriteLine(Result);
                if (SelectedFacility == FacilityType.Cashbot)
                    Console.WriteLine("NOTE: For Cashbot, the calculator assumes a worst-case scenario. You may not need to do as much as the calculator recommends.\n");
                Console.WriteLine("Press any key to continue.");
                Console.ReadKey();
            }
        }

        // Function for calculating and printing the suggested route
        static string CalculateAndPrint(FacilityType Facility, uint PointsNeeded)
        {
            (string, uint, double)[] PointSources = FacilityValues[Facility];
            
            uint[] SourceCounts = CalculateMostEfficientRoute(PointSources, PointsNeeded);

            string OutputString = "You should do:\n\n";

            for (int i = 0; i < SourceCounts.Length; ++i)
            {
                if (SourceCounts[i] == 0)
                    continue;
                OutputString += $"{SourceCounts[i]} {PointSources[i].Item1}{(SourceCounts[i] > 1 ? "s" : "")}\n";
            }

            return OutputString;
        }

        // The actual recursive function for calculating the route
        static uint[] CalculateMostEfficientRoute((string, uint, double)[] PointSources, uint PointsNeeded)
        {
            uint[] SourceCounts = new uint[PointSources.Length];
            uint PointsRemaining = PointsNeeded;
            while (PointsRemaining > 0)
            {
                double LowestTime = 0;
                uint BestCount = 0;
                uint BestSourceIndex = 0;
                for (uint i = 0; i < PointSources.Length; ++i)
                {
                    var PointSource = PointSources[i];
                    uint Points = PointSource.Item2;
                    double Time = PointSource.Item3;
                    double FractionNeeded = ((double)PointsRemaining / (double)Points);
                    uint RoundedFraction = (uint)Math.Floor(FractionNeeded);
                    uint CountNeeded = Math.Max(RoundedFraction, 1);
                    double TimeNeeded = Time * CountNeeded;
                    if (Points * CountNeeded < PointsRemaining)
                    {
                        uint[] TestCounts = CalculateMostEfficientRoute(PointSources, PointsRemaining - (Points * CountNeeded));
                        for (int j = 0; j < TestCounts.Length; ++j)
                            TimeNeeded += TestCounts[j] * PointSources[j].Item3;
                    } 
                    if (TimeNeeded < LowestTime || LowestTime == 0)
                    {
                        LowestTime = TimeNeeded;
                        BestCount = CountNeeded;
                        BestSourceIndex = i;
                    }
                };
                SourceCounts[BestSourceIndex] += BestCount;
                uint PointsEarned = PointSources[BestSourceIndex].Item2 * BestCount;
                if (PointsEarned > PointsRemaining)
                    break;
                else
                    PointsRemaining -= PointsEarned;
            };
            return SourceCounts;
        }
    }
}
