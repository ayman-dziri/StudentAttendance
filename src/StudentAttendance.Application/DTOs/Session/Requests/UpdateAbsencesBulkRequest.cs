using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using StudentAttendance.src.StudentAttendance.Domain.Enums;

namespace StudentAttendance.src.StudentAttendance.Application.DTOs.Session.Requests
{
    public class UpdateAbsencesBulkItem
    {
        public string StudentId { get; set; } = null!;
        public StatusPresence Status { get; set; }
    }
}