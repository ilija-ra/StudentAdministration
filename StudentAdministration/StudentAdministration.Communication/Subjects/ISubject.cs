using Microsoft.ServiceFabric.Services.Remoting;
using StudentAdministration.Communication.Subjects.Models;

namespace StudentAdministration.Communication.Subjects
{
    public interface ISubject : IService
    {
        Task<SubjectEnrollResponseModel> Enroll(SubjectEnrollRequestModel? model);

        Task<SubjectGetAllEnrolledResponseModel> GetAllEnrolled(string? studentId);

        Task<SubjectDropOutResponseModel> DropOut(string? subjectId, string? studentId);

        Task<SubjectGetStudentsBySubjectResponseModel> GetStudentsBySubject(string? subjectId);

        Task<SubjectSetGradesResponseModel> SetGrades(SubjectSetGradesRequestModel? model);

        Task<SubjectGetAllResponseModel> GetAll();

        Task<SubjectConfirmSubjectsResponseModel> ConfirmSubjects();
    }
}
