using Microsoft.ServiceFabric.Services.Remoting;
using StudentAdministration.Communication.Subjects.Models;

namespace StudentAdministration.Communication.Subjects
{
    public interface ISubject : IService
    {
        Task<SubjectEnrollResponseModel> Enroll(SubjectEnrollRequestModel? model);

        Task<SubjectGetAllEnrolledResponseModel> GetAllEnrolled(string? studentId, bool? dropOut);

        Task<SubjectDropOutResponseModel> DropOut(SubjectDropOutRequestModel model);

        Task<SubjectGetStudentsBySubjectResponseModel> GetStudentsBySubject(string? subjectId);

        Task<SubjectGetSubjectsByProfessorResponseModel> GetSubjectsByProfessor(string? professorId);

        Task<SubjectSetGradeResponseModel> SetGrade(SubjectSetGradesRequestModel? model);

        Task<SubjectGetAllResponseModel> GetAll();

        Task<SubjectConfirmSubjectsResponseModel> ConfirmSubjects(string? studentId);

        Task ClearDictionaries();
    }
}
