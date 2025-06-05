using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using MedLabTab.DatabaseModels;
using MedLabTab.DatabaseManager;
using MedLabTab.ViewModels;

namespace MedLabTabTests
{
    [TestClass]
    public class Tests
    {
        private DbContextOptions<MedLabContext> _contextOptions;

        [TestInitialize]
        public void Setup()
        {
            _contextOptions = new DbContextOptionsBuilder<MedLabContext>()
                .UseInMemoryDatabase(databaseName: $"TestDb_{System.Guid.NewGuid()}")
                .Options;
        }

        private User CreateUser(MedLabContext context, string login, string name, string surname, string pesel, string phone, string password, int userType)
        {
            var user = new User
            {
                Login = login,
                Password = password,
                UserType = userType,
                IsActive = true,
                Name = name,
                Surname = surname,
                PESEL = pesel,
                PhoneNumber = phone,
            };

            context.Users.Add(user);
            context.SaveChanges();

            return user;
        }

        [TestMethod]
        public void IntegrationTest_ShouldCreateVisitWithTestPatientNurseSchedule()
        {
            using var context = new MedLabContext(_contextOptions);

            // Arrange
            var category = new CategoryDictionary { CategoryName = "Badania krwi" };
            context.CategoryDictionaries.Add(category);
            context.SaveChanges();

            var test = new Test
            {
                TestName = "Morfologia",
                Description = "Pe³na morfologia krwi",
                Price = 40.0f,
                IsActive = true,
                Category = category.id
            };
            context.Tests.Add(test);
            context.SaveChanges();

            var patient = CreateUser(context, "pacjent_testowy", "Jan", "Kowalski", "12345678901", "1234567890", "haslo123", 4);
            var nurse = CreateUser(context, "nurse_testowa", "Anna", "Nowak", "98765432101", "1234567890", "haslo123", 2);

            var scheduleSlot = new Schedule
            {
                NurseId = nurse.id,
                Date = DateOnly.FromDateTime(DateTime.Today.AddDays(1)),
                Time = new TimeOnly(9, 0)
            };
            context.Schedules.Add(scheduleSlot);
            context.SaveChanges();

            var visit = new Visit
            {
                PatientId = patient.id,
                TimeSlotId = scheduleSlot.id,
                IsActive = true
            };
            context.Visits.Add(visit);
            context.SaveChanges();

            var testHistory = new TestHistory
            {
                VisitId = visit.id,
                TestId = test.id,
                PatientId = patient.id,
                Status = 1
            };
            context.TestHistories.Add(testHistory);
            context.SaveChanges();

            // Assert
            var savedVisit = context.Visits
                .Include(v => v.TimeSlot)
                .Include(v => v.Patient)
                .FirstOrDefault(v => v.id == visit.id);

            Assert.IsNotNull(savedVisit);
            Assert.AreEqual(patient.id, savedVisit.PatientId);
            Assert.AreEqual(scheduleSlot.id, savedVisit.TimeSlotId);

            var savedTest = context.TestHistories
                .Include(th => th.Test)
                .Include(th => th.Patient)
                .Include(th => th.Visit)
                .FirstOrDefault(th => th.VisitId == visit.id);

            Assert.IsNotNull(savedTest);
            Assert.AreEqual(test.TestName, savedTest.Test.TestName);
        }

        [TestMethod]
        public void IntegrationTest_ShouldAddTestUsingDbManager()
        {
            using var context = new MedLabContext(_contextOptions);

            DbManager.InitForTesting(context);

            var category = new CategoryDictionary { CategoryName = "Diagnostyka" };
            context.CategoryDictionaries.Add(category);
            context.SaveChanges();

            var testName = "Test Integracyjny";
            var test = new Test
            {
                TestName = testName,
                Description = "Opis testu integracyjnego",
                Price = 99.99f,
                Category = category.id,
                IsActive = true
            };

            var nameTaken = DbManager.IsTestNameTaken(testName);
            Assert.IsFalse(nameTaken);

            var result = DbManager.AddTest(test);

            Assert.IsTrue(result);
            var savedTest = context.Tests.FirstOrDefault(t => t.TestName == testName);
            Assert.IsNotNull(savedTest);
            Assert.AreEqual("Opis testu integracyjnego", savedTest.Description);
            Assert.AreEqual(category.id, savedTest.Category);
        }

        [TestMethod]
        public void IntegrationTest_ShouldAddNewUser_WhenValidInputs()
        {
            using var context = new MedLabContext(_contextOptions);

            DbManager.InitForTesting(context);

            // Dane testowe
            string name = "Jan";
            string surname = "Kowalski";
            string pesel = "12345678901";
            string phone = "123456789";
            string login = "jan.kowalski";
            string password = "haslo123";
            string repeatPassword = "haslo123";
            int userType = 4; // Pacjent

            bool valid = !string.IsNullOrWhiteSpace(name)
                && !string.IsNullOrWhiteSpace(surname)
                && !string.IsNullOrWhiteSpace(pesel)
                && pesel.Length == 11
                && !string.IsNullOrWhiteSpace(phone)
                && phone.Length >= 9
                && !string.IsNullOrWhiteSpace(login)
                && !string.IsNullOrWhiteSpace(password)
                && password.Length >= 6
                && password == repeatPassword;

            Assert.IsTrue(valid, "Walidacja danych powinna przejœæ.");

            Assert.IsFalse(DbManager.IsLoginTaken(login), "Login nie powinien byæ zajêty.");
            Assert.IsFalse(DbManager.IsPESELTaken(pesel), "PESEL nie powinien byæ zajêty.");

            var newUser = new User
            {
                Name = name,
                Surname = surname,
                PESEL = pesel,
                PhoneNumber = phone,
                Login = login,
                Password = password, 
                UserType = userType,
                IsActive = true
            };

            // Act
            bool result = DbManager.AddUser(newUser);

            // Assert
            Assert.IsTrue(result, "U¿ytkownik powinien zostaæ poprawnie dodany.");

            var savedUser = context.Users.FirstOrDefault(u => u.Login == login);
            Assert.IsNotNull(savedUser, "U¿ytkownik powinien istnieæ w bazie.");
            Assert.AreEqual(name, savedUser.Name);
            Assert.AreEqual(surname, savedUser.Surname);
            Assert.AreEqual(pesel, savedUser.PESEL);
            Assert.AreEqual(phone, savedUser.PhoneNumber);
            Assert.AreEqual(userType, savedUser.UserType);
            string hashedPassword = DbManager.GetHashedPassword(login);
            Assert.IsNotNull(hashedPassword, "Zapisane has³o powinno istnieæ.");
            Assert.IsTrue(PasswordHasher.Verify(password, hashedPassword), "Zapisane has³o powinno byæ poprawne.");
        }

    }
}
