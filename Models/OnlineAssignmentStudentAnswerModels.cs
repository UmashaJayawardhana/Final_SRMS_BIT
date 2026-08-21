using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MvcApplication1.Models
{
    public class OnlineAssignmentStudentAnswerModels
    {
        public int AnswerSerial { get; set; }
        public int ExamScheduleAssignmentSerial { get; set; }
        public int StudentSerial { get; set; }
        public int QuestionSerial { get; set; }
        public int SelectedOption { get; set; }
    }
}
