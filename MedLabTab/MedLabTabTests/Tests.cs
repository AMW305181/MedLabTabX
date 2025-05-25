using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System.Linq;
using MedLabTab.DatabaseModels;

namespace MedLabTabTests
{
    [TestClass]
    public class Tests
    {
        private MedLabContext _context;
        private IDbContextTransaction _transaction;

        [TestInitialize]
        public void SetupTransactionalTest()
        {
            // U¿ywa prawdziwej bazy, ale w transakcji która zostanie cofniêta
            _context = new MedLabContext();
            _transaction = _context.Database.BeginTransaction();
        }

        [TestCleanup]
        public void CleanupTransactionalTest()
        {
            // Rollback - cofniêcie wszystkich zmian z testu
            _transaction?.Rollback();
            _transaction?.Dispose();
            _context?.Dispose();
        }

        [TestMethod]
        public void IntegrationTest_ShouldCreateVisitWithTestPatientNurseSchedule()
        {
            // Test u¿ywa prawdziwej bazy, ale wszystko zostanie cofniête po teœcie
            // Arrange
            var category = new CategoryDictionary { CategoryName = "Badania krwi" };
            _context.CategoryDictionaries.Add(category);
            _context.SaveChanges();

            //Dodanie testu - u¿ywamy bezpoœrednio context zamiast DbManager
            var test = new Test
            {
                TestName = "Morfologia",
                Description = "Pe³na morfologia krwi",
                Price = 40.0f,
                IsActive = true,
                Category = category.id
            };
            _context.Tests.Add(test);
            _context.SaveChanges();

            //Dodanie pacjenta
            var patient = new User
            {
                Login = "pacjent_testowy",
                Password = "haslo123",
                UserType = 4, // Pacjent
                IsActive = true,
                Name = "Jan",
                Surname = "Kowalski",
                PESEL = "12345678901",
                PhoneNumber = "1234567890",
            };
            _context.Users.Add(patient);
            _context.SaveChanges();

            //Dodanie pielêgniarki
            var nurse = new User
            {
                Login = "nurse_testowa",
                Password = "haslo123",
                UserType = 2, // Pielêgniarka
                IsActive = true,
                Name = "Anna",
                Surname = "Nowak",
                PESEL = "98765432101",
                PhoneNumber = "1234567890",
            };
            _context.Users.Add(nurse);
            _context.SaveChanges();

            //Dodanie slotu harmonogramu
            var scheduleSlot = new Schedule
            {
                NurseId = nurse.id,
                Date = DateOnly.FromDateTime(DateTime.Today.AddDays(1)), //dzisiaj + 1 dzien
                Time = new TimeOnly(9, 0) // 9:00
            };
            _context.Schedules.Add(scheduleSlot);
            _context.SaveChanges();

            //Utworzenie wizyty
            var visit = new Visit
            {
                PatientId = patient.id,
                TimeSlotId = scheduleSlot.id,
                IsActive = true
            };
            _context.Visits.Add(visit);
            _context.SaveChanges();

            //Dodanie historii testu
            var testHistory = new TestHistory
            {
                VisitId = visit.id,
                TestId = test.id,
                PatientId = patient.id,
                Status = 1 // Nowy
            };
            _context.TestHistories.Add(testHistory);
            _context.SaveChanges();

            // Assert – sprawdzanie czy dane siê zapisa³y w ramach transakcji
            var savedVisit = _context.Visits
                .Include(v => v.TimeSlot)
                .Include(v => v.Patient)
                .FirstOrDefault(v => v.id == visit.id);
            Assert.IsNotNull(savedVisit);
            Assert.AreEqual(patient.id, savedVisit.PatientId);
            Assert.AreEqual(scheduleSlot.id, savedVisit.TimeSlotId);

            var savedTest = _context.TestHistories
                .Include(th => th.Test)
                .Include(th => th.Patient)
                .Include(th => th.Visit)
                .FirstOrDefault(th => th.VisitId == visit.id);
            Assert.IsNotNull(savedTest);
            Assert.AreEqual(test.TestName, savedTest.Test.TestName);

            // Po zakoñczeniu testu wszystko zostanie automatycznie cofniête przez rollback
        }
    }
}