using Azure.Data.Tables;
using Microsoft.ServiceFabric.Data;
using Microsoft.ServiceFabric.Data.Collections;
using Microsoft.ServiceFabric.Services.Communication.Runtime;
using Microsoft.ServiceFabric.Services.Remoting.Runtime;
using Microsoft.ServiceFabric.Services.Runtime;
using StudentAdministration.Communication.Subjects;
using StudentAdministration.Communication.Subjects.Models;
using StudentAdministration.Subject.Entities;
using System.Fabric;
using System.Security.Cryptography;
using System.Text;

namespace StudentAdministration.Subject
{
    internal sealed class Subject : StatefulService, ISubject
    {
        private const string _gradeName = "Grades";
        private const string _subjectName = "Subjects";
        private const string _connectionString = "DefaultEndpointsProtocol=http;AccountName=devstoreaccount1;AccountKey=Eby8vdM02xNOcqFlqUwJPLlmEtlCDXJ1OUzFT50uSRZ6IFsuFq2UVErCz4I6tq/K1SZFPTOtr/KBHBeksoGMGw==;TableEndpoint=http://127.0.0.1:10002/devstoreaccount1;";
        private static TableServiceClient _serviceClient = new TableServiceClient(_connectionString);
        private static TableClient _subjectTableClient = _serviceClient.GetTableClient(_subjectName);
        private static TableClient _gradeTableClient = _serviceClient.GetTableClient(_gradeName);

        private readonly IReliableStateManager _stateManager;
        private IReliableDictionary<string, GradeEntity> _grades;

        public Subject(StatefulServiceContext context) : base(context)
        {
            _stateManager = this.StateManager;
        }

        #region ISubjectImplementation

        public async Task<SubjectEnrollResponseModel> Enroll(SubjectEnrollRequestModel? model)
        {
            try
            {
                using (ITransaction tx = _stateManager.CreateTransaction())
                {
                    var grade = new GradeEntity()
                    {
                        Id = null,
                        SubjectId = model?.SubjectId,
                        SubjectPartitionKey = model?.SubjectPartitionKey,
                        StudentId = model?.StudentId,
                        StudentPartitionKey = model?.StudentPartitionKey,
                        StudentFullName = model?.StudentFullName,
                        StudentIndex = model?.StudentIndex,
                        ProfessorFullName = model?.ProfessorFullName,
                        Grade = model?.Grade
                    };

                    ConditionalValue<GradeEntity> existingGrade = await _grades.TryGetValueAsync(tx, model?.SubjectId + model?.StudentId);

                    if (existingGrade.HasValue)
                    {
                        tx.Abort();
                        return null!;
                    }

                    await _grades.AddOrUpdateAsync(tx, model?.SubjectId + model?.StudentId, grade, (id, value) => grade);
                    await tx.CommitAsync();

                    return new SubjectEnrollResponseModel();
                }
            }
            catch
            {
                return null!;
            }
        }

        public async Task<SubjectDropOutResponseModel> DropOut(SubjectDropOutRequestModel? model)
        {
            try
            {
                using (ITransaction tx = _stateManager.CreateTransaction())
                {
                    ConditionalValue<GradeEntity> existingGrade = await _grades.TryGetValueAsync(tx, model?.SubjectId! + model?.StudentId);

                    if (!existingGrade.HasValue)
                    {
                        return null!;
                    }

                    await _grades.TryRemoveAsync(tx, model?.SubjectId! + model?.StudentId);
                    await tx.CommitAsync();

                    return new SubjectDropOutResponseModel();
                }
            }
            catch
            {
                return null!;
            }
        }

        // WARNING: This method need to be called before anything else
        // Because it populates _grades dictionary
        public async Task<SubjectGetAllEnrolledResponseModel> GetAllEnrolled(string? studentId)
        {
            try
            {
                if (await IsDictionaryEmptyAsync<GradeEntity>(_grades))
                {
                    return await UtilizeAzure(studentId);
                }
                else
                {
                    return await UtilizeGradeDictionary();
                }
            }
            catch
            {
                return null!;
            }
        }

        public async Task<SubjectGetAllEnrolledResponseModel> UtilizeAzure(string? studentId)
        {
            try
            {
                var items = new List<SubjectGetAllEnrolledItemModel>();

                using (ITransaction tx = _stateManager.CreateTransaction())
                {
                    var gradeEntities = _gradeTableClient.QueryAsync<GradeEntity>(x => x.StudentId!.Equals(studentId));
                    var subjectEntities = _subjectTableClient.QueryAsync<SubjectEntity>().ToBlockingEnumerable();

                    await foreach (var entity in gradeEntities)
                    {
                        var subject = subjectEntities.FirstOrDefault(x => x.Id == entity.SubjectId && x.PartitionKey == entity.SubjectPartitionKey);

                        items.Add(new SubjectGetAllEnrolledItemModel()
                        {
                            Id = entity.Id,
                            Subject = new SubjectItemModel()
                            {
                                Id = subject?.Id,
                                Title = subject?.Title,
                                Department = subject?.Department,
                                Grade = entity.Grade,
                                PartitionKey = subject?.PartitionKey
                            },
                            StudentId = entity.StudentId,
                            StudentPartitionKey = entity.StudentPartitionKey,
                            ProfessorFullName = entity.ProfessorFullName
                        });

                        await _grades.AddAsync(tx, entity?.SubjectId + entity?.StudentId, entity!);
                    }

                    await tx.CommitAsync();

                    return new SubjectGetAllEnrolledResponseModel() { Items = items };
                }
            }
            catch
            {
                return null!;
            }
        }

        public async Task<SubjectGetAllEnrolledResponseModel> UtilizeGradeDictionary()
        {
            try
            {
                var response = new SubjectGetAllEnrolledResponseModel();

                using (var tx = _stateManager.CreateTransaction())
                {
                    var enumerator = (await _grades.CreateEnumerableAsync(tx)).GetAsyncEnumerator();
                    var subjectEntities = _subjectTableClient.QueryAsync<SubjectEntity>().ToBlockingEnumerable();

                    while (await enumerator.MoveNextAsync(default))
                    {
                        var enumer = enumerator.Current.Value;
                        var subject = subjectEntities.FirstOrDefault(x => x.Id == enumer.SubjectId && x.PartitionKey == enumer.SubjectPartitionKey);

                        response.Items!.Add(new SubjectGetAllEnrolledItemModel()
                        {
                            //Id = enumer.Id,
                            //SubjectId = enumer.SubjectId,
                            //SubjectPartitionKey = enumer.SubjectPartitionKey,
                            //StudentId = enumer.StudentId,
                            //StudentPartitionKey = enumer.StudentPartitionKey,
                            //ProfessorFullName = enumer.ProfessorFullName,
                            //Grade = enumer.Grade

                            Id = enumer.Id,
                            Subject = new SubjectItemModel()
                            {
                                Id = subject?.Id,
                                Title = subject?.Title,
                                Department = subject?.Department,
                                Grade = enumer.Grade,
                                PartitionKey = subject?.PartitionKey
                            },
                            StudentId = enumer.StudentId,
                            StudentPartitionKey = enumer.StudentPartitionKey,
                            ProfessorFullName = enumer.ProfessorFullName
                        });
                    }
                }

                return response;
            }
            catch
            {
                return null!;
            }
        }

        public async Task<SubjectGetSubjectsByProfessorResponseModel> GetSubjectsByProfessor(string? professorId)
        {
            try
            {
                var items = new List<SubjectGetSubjectsByProfessorItemModel>();

                var entities = _subjectTableClient.QueryAsync<SubjectEntity>(x => x.ProfessorId!.Equals(professorId));

                await foreach (var entity in entities)
                {
                    items.Add(new SubjectGetSubjectsByProfessorItemModel()
                    {
                        SubjectId = entity.Id,
                        SubjectPartitionKey = entity.PartitionKey,
                        Title = entity.Title,
                        Department = entity.Department,
                        ProfessorId = entity.ProfessorId,
                        ProfessorFullName = entity.ProfessorFullName,
                        ProfessorPartitionKey = entity.ProfessorPartitionKey,
                    });
                }

                return new SubjectGetSubjectsByProfessorResponseModel() { Items = items };
            }
            catch
            {
                return null!;
            }
        }

        public async Task<SubjectGetStudentsBySubjectResponseModel> GetStudentsBySubject(string? subjectId)
        {
            try
            {
                var items = new List<SubjectGetStudentsBySubjectItemModel>();

                var entities = _gradeTableClient.QueryAsync<GradeEntity>(x => x.SubjectId!.Equals(subjectId));

                await foreach (var entity in entities)
                {
                    items.Add(new SubjectGetStudentsBySubjectItemModel()
                    {
                        Id = entity.Id,
                        SubjectId = entity.SubjectId,
                        SubjectPartitionKey = entity.SubjectPartitionKey,
                        StudentId = entity.StudentId,
                        StudentPartitionKey = entity.StudentPartitionKey,
                        StudentIndex = entity.StudentIndex,
                        StudentFullName = entity.StudentFullName,
                        Grade = entity.Grade
                    });
                }

                return new SubjectGetStudentsBySubjectResponseModel() { Items = items };
            }
            catch
            {
                return null!;
            }
        }

        public async Task<SubjectSetGradeResponseModel> SetGrade(SubjectSetGradesRequestModel? model)
        {
            try
            {
                var student = _gradeTableClient.QueryAsync<GradeEntity>(
                    x => x.SubjectId == model!.SubjectId &&
                    x.SubjectPartitionKey == model.SubjectPartitionKey &&
                    x.StudentId == model.StudentId &&
                    x.StudentPartitionKey == model.StudentPartitionKey)
                    .ToBlockingEnumerable().FirstOrDefault();

                if (student is null)
                {
                    return null!;
                }

                student.Grade = model?.Grade;

                await _gradeTableClient.UpsertEntityAsync(student);

                return new SubjectSetGradeResponseModel();

                //await foreach (var entity in entities)
                //{
                //    foreach (var studentGrade in model!.StudentGrades)
                //    {
                //        if (studentGrade.StudentPartitionKey!.Equals(entity.StudentPartitionKey)
                //            && studentGrade.StudentId!.Equals(entity.StudentId))
                //        {
                //            entity.Grade = studentGrade.Grade;

                //            await _gradeTableClient.UpsertEntityAsync(entity);

                //            break;
                //        }
                //    }
                //}

                //return new SubjectSetGradeResponseModel();
            }
            catch
            {
                return null!;
            }
        }

        public async Task<SubjectGetAllResponseModel> GetAll()
        {
            try
            {
                var items = new List<SubjectGetAllItemModel>();
                var entities = _subjectTableClient.QueryAsync<SubjectEntity>();

                await foreach (var entity in entities)
                {
                    items.Add(new SubjectGetAllItemModel()
                    {
                        Id = entity.Id,
                        Title = entity.Title,
                        Department = entity.Department,
                        PartitionKey = entity.PartitionKey,
                        Professor = new ProfessorItemModel()
                        {
                            Id = entity.ProfessorId,
                            FullName = entity.ProfessorFullName,
                            PartitionKey = entity.ProfessorPartitionKey
                        }
                    });
                }

                return new SubjectGetAllResponseModel() { Items = items };
            }
            catch
            {
                return null!;
            }
        }

        public async Task<SubjectConfirmSubjectsResponseModel> ConfirmSubjects()
        {
            try
            {
                var items = await GetItemsFromDictionaryAsync();

                foreach (var item in items)
                {
                    if (!ItemExistsInTableAsync(item))
                    {
                        item!.Id = Guid.NewGuid().ToString();
                        item.PartitionKey = AssignToPartition(item!.Id).ToString();
                        item.RowKey = item!.Id;

                        await _gradeTableClient.UpsertEntityAsync(item);
                    }
                }

                return new SubjectConfirmSubjectsResponseModel();
            }
            catch
            {
                return null!;
            }
        }

        #endregion

        #region PrivateHelpers

        private async Task InitializeTables()
        {
            await _gradeTableClient.CreateIfNotExistsAsync();
            await _subjectTableClient.CreateIfNotExistsAsync();
        }

        private async Task PopulateSubjects()
        {
            try
            {
                var subId1 = "be3e6225-ee2d-47cf-96c8-b8e427496637";

                await _subjectTableClient.UpsertEntityAsync(new TableEntity(AssignToPartition(subId1).ToString(), subId1)
                {
                    { "Id", subId1 },
                    { "Title", "Biology" },
                    { "Department", "Department of Biological Sciences" },
                    { "ProfessorId", "a524095a-0688-487b-8b73-cc28f084cfd9" },
                    { "ProfessorFullName", "Samantha Rodriguez" },
                    { "ProfessorPartitionKey", "1" },
                });

                var subId2 = "5dc384d0-32f0-4c16-a233-f63078cbf1f3";

                await _subjectTableClient.UpsertEntityAsync(new TableEntity(AssignToPartition(subId2).ToString(), subId2)
                {
                    { "Id", subId2 },
                    { "Title", "Economics" },
                    { "Department", "Department of Economics" },
                    { "ProfessorId", "a524095a-0688-487b-8b73-cc28f084cfd9" },
                    { "ProfessorFullName", "Samantha Rodriguez" },
                    { "ProfessorPartitionKey", "1" },
                });

                var subId3 = "406cc2ba-654c-4418-a155-f7048e917af4";

                await _subjectTableClient.UpsertEntityAsync(new TableEntity(AssignToPartition(subId3).ToString(), subId3)
                {
                    { "Id", subId3 },
                    { "Title", "History" },
                    { "Department", "Department of History" },
                    { "ProfessorId", "a524095a-0688-487b-8b73-cc28f084cfd9" },
                    { "ProfessorFullName", "Samantha Rodriguez" },
                    { "ProfessorPartitionKey", "1" },
                });

                var subId4 = "1ddaa48c-4e91-49b7-a175-5d9e70283568";

                await _subjectTableClient.UpsertEntityAsync(new TableEntity(AssignToPartition(subId4).ToString(), subId4)
                {
                    { "Id", subId4 },
                    { "Title", "Computer Science" },
                    { "Department", "Department of Computer Science and Engineering" },
                    { "ProfessorId", "0e9c175c-e9ac-4863-9747-7cf26ca4c8de" },
                    { "ProfessorFullName", "Benjamin Hayes" },
                    { "ProfessorPartitionKey", "1" },
                });

                var subId5 = "7a164230-d106-4a6f-af45-8eda7b385ab3";

                await _subjectTableClient.UpsertEntityAsync(new TableEntity(AssignToPartition(subId5).ToString(), subId5)
                {
                    { "Id", subId5 },
                    { "Title", "Literature" },
                    { "Department", "Department of English or Humanities" },
                    { "ProfessorId", "0e9c175c-e9ac-4863-9747-7cf26ca4c8de" },
                    { "ProfessorFullName", "Benjamin Hayes" },
                    { "ProfessorPartitionKey", "1" },
                });

                var subId6 = "bfa769d2-6722-4c25-b61b-d02a0b120dff";

                await _subjectTableClient.UpsertEntityAsync(new TableEntity(AssignToPartition(subId6).ToString(), subId6)
                {
                    { "Id", subId6 },
                    { "Title", "Psychology" },
                    { "Department", "Department of Psychology" },
                    { "ProfessorId", "0e9c175c-e9ac-4863-9747-7cf26ca4c8de" },
                    { "ProfessorFullName", "Benjamin Hayes" },
                    { "ProfessorPartitionKey", "1" },
                });

                var subId7 = "d704c674-04c9-4e02-844e-9732023be98c";

                await _subjectTableClient.UpsertEntityAsync(new TableEntity(AssignToPartition(subId7).ToString(), subId7)
                {
                    { "Id", subId7 },
                    { "Title", "Chemistry" },
                    { "Department", "Department of Chemistry" },
                    { "ProfessorId", "0e9c175c-e9ac-4863-9747-7cf26ca4c8de" },
                    { "ProfessorFullName", "Benjamin Hayes" },
                    { "ProfessorPartitionKey", "1" },
                });
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        private async Task EmptyDictionaryAsync<T>(IReliableDictionary<string, T> dictionary)
        {
            using (var tx = _stateManager.CreateTransaction())
            {
                var enumerable = await dictionary.CreateEnumerableAsync(tx);
                var enumerator = enumerable.GetAsyncEnumerator();

                while (await enumerator.MoveNextAsync(default))
                {
                    var current = enumerator.Current;
                    await dictionary.TryRemoveAsync(tx, current.Key);
                }

                await tx.CommitAsync();
            }
        }


        private async Task<bool> IsDictionaryEmptyAsync<T>(IReliableDictionary<string, T> dictionary)
        {
            using (var tx = _stateManager.CreateTransaction())
            {
                var enumerable = await dictionary.CreateEnumerableAsync(tx);
                var enumerator = enumerable.GetAsyncEnumerator();

                return !await enumerator.MoveNextAsync(CancellationToken.None);
            }
        }

        private async Task<List<GradeEntity>> GetItemsFromDictionaryAsync()
        {
            var items = new List<GradeEntity>();

            using (var tx = _stateManager.CreateTransaction())
            {
                var enumerable = await _grades.CreateEnumerableAsync(tx);
                var enumerator = enumerable.GetAsyncEnumerator();

                while (await enumerator.MoveNextAsync(CancellationToken.None))
                {
                    items.Add(enumerator.Current.Value);
                }
            }

            return items;
        }

        private bool ItemExistsInTableAsync(GradeEntity? model)
        {
            var query = _gradeTableClient.QueryAsync<GradeEntity>(x => x.SubjectId == model!.SubjectId && x.StudentId == model.StudentId);

            var results = query!.ToBlockingEnumerable().FirstOrDefault();

            return results != null;
        }

        private static int AssignToPartition(string partitionKey)
        {
            // Use SHA256 hash function to hash the partition key
            byte[] hashedValue;
            using (SHA256 sha256 = SHA256.Create())
            {
                hashedValue = sha256.ComputeHash(Encoding.UTF8.GetBytes(partitionKey));
            }

            // Convert hashed value to integer
            int intValue = BitConverter.ToInt32(hashedValue, 0);

            // Map the hashed value to one of three partitions
            int partitionIndex = Math.Abs(intValue % 3);

            return partitionIndex;
        }

        #endregion

        protected override IEnumerable<ServiceReplicaListener> CreateServiceReplicaListeners()
        {
            return this.CreateServiceRemotingReplicaListeners();
        }

        protected override async Task RunAsync(CancellationToken cancellationToken)
        {
            await InitializeTables();
            await PopulateSubjects();
            _grades = await _stateManager.GetOrAddAsync<IReliableDictionary<string, GradeEntity>>("grades");
        }
    }
}
