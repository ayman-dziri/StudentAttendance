//using StudentAttendance.src.StudentAttendance.Domain.Entities;


//namespace StudentAttendance.src.StudentAttendance.Infrastructure.Repositories.Mocks
//{
//    public class FakeAbsenceRepository : IAbsenceRepository
//    {
//        private readonly List<Absence> _data = new();

//        public Task<Absence?> GetByStudentAndSessionAsync(string studentId, string sessionId)
//            => Task.FromResult(_data.FirstOrDefault(x => x.StudentId == studentId && x.SessionId == sessionId));

//        public Task<List<Absence>> GetByStudentIdAsync(string studentId)
//            => Task.FromResult(_data.Where(x => x.StudentId == studentId).ToList());

//        public Task CreateAsync(Absence absence)
//        {
//            absence.Id = string.IsNullOrWhiteSpace(absence.Id) ? Guid.NewGuid().ToString() : absence.Id;
//            _data.Add(absence);
//            return Task.CompletedTask;
//        }

//        public Task UpdateAsync(Absence absence)
//        {
//            var idx = _data.FindIndex(x => x.Id == absence.Id);
//            if (idx >= 0) _data[idx] = absence;
//            return Task.CompletedTask;
//        }
//    }
//}
