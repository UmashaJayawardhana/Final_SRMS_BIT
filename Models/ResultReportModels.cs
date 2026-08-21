using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MvcApplication1.Models
{
    public class ResultReportModels
    {

        public int No { get; set; }
        public int ExamSerial { get; set; }
        public string ExamName { get; set; }
        public int TermSerial { get; set; }
        public string TermName { get; set; }
        public string Status { get; set; }
        public int Year { get; set; }
        public int BatchSerial { get; set; }
        public int ClassSerial { get; set; }
        public string BatchName { get; set; }
        public string ClassName { get; set; }
        public int SubjectSerial { get; set; }
        public string SubjectName { get; set; }
        public DateTime DateTime { get; set; }

        public string TeacherName { get; set; }

        public int StudentSerial { get; set; }
        public string FullName { get; set; }
        public string Attedance { get; set; }
        public decimal? Result { get; set; }
        public string RegNo { get; set; }

        public decimal? Sinhala { get; set; }
        public decimal? Mathematics { get; set; }
        public decimal? Total { get; set; }
        public int Rank { get; set; }


        public string AttedanceBuddhism { get; set; }
        public string AttedanceTamil { get; set; }
        public string AttedanceEnglish { get; set; }
        public string AttedanceEStudies { get; set; }
        public string AttedanceMathematics { get; set; }
        public string AttedanceSinhala { get; set; }


        public decimal? T1Assignment1 { get; set; }
        public decimal? T1Assignment2 { get; set; }
        public decimal? T1Final { get; set; }
        public decimal? T2Assignment1 { get; set; }
        public decimal? T2Assignment2 { get; set; }
        public decimal? T2Final { get; set; }
        public decimal? T3Assignment1 { get; set; }
        public decimal? T3Assignment2 { get; set; }
        public decimal? T3Final { get; set; }

        public string FullNameBuddhism { get; set; }
        public string FullNameTamil { get; set; }
        public string FullNameEnglish { get; set; }
        public string FullNameEStudies { get; set; }
        public string FullNameMathematics { get; set; }
        public string FullNameSinhala { get; set; }

        public string FullNameLowestBuddhism { get; set; }
        public string FullNameLowestTamil { get; set; }
        public string FullNameLowestEnglish { get; set; }
        public string FullNameLowestEStudies { get; set; }
        public string FullNameLowestMathematics { get; set; }
        public string FullNameLowestSinhala { get; set; }

        public decimal? LowestSinhala { get; set; }
        public decimal? LowestMathematics { get; set; }
        public decimal? LowestEStudies { get; set; }
        public decimal? LowestEnglish { get; set; }
        public decimal? LowestTamil { get; set; }
        public decimal? LowestBuddhism { get; set; }

        public decimal? HighestSinhala { get; set; }
        public decimal? HighestMathematics { get; set; }
        public decimal? HighestEStudies { get; set; }
        public decimal? HighestEnglish { get; set; }
        public decimal? HighestTamil { get; set; }
        public decimal? HighestBuddhism { get; set; }

        public decimal? AverageSinhala { get; set; }
        public decimal? AverageMathematics { get; set; }
        public decimal? AverageEStudies { get; set; }
        public decimal? AverageEnglish { get; set; }
        public decimal? AverageTamil { get; set; }
        public decimal? AverageBuddhism { get; set; }

        public decimal? As1Sinhala { get; set; }
        public decimal? As1Mathematics { get; set; }
        public decimal? As1EStudies { get; set; }
        public decimal? As1English { get; set; }
        public decimal? As1Tamil { get; set; }
        public decimal? As1Buddhism { get; set; }

        public decimal? As2Sinhala { get; set; }
        public decimal? As2Mathematics { get; set; }
        public decimal? As2EStudies { get; set; }
        public decimal? As2English { get; set; }
        public decimal? As2Tamil { get; set; }
        public decimal? As2Buddhism { get; set; }
    }
}