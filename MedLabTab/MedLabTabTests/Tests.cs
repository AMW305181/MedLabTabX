using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using MedLabTab.DatabaseModels;
using MedLabTab.DatabaseManager;

namespace MedLabTabTests
{
    [TestClass]
    public class Tests
    {
        private DbContextOptions<MedLabContext> _contextOptions;

        [TestInitialize]
        public void Setup()
        {
            // Inicjalizacja InMemoryDb
            _contextOptions = new DbContextOptionsBuilder<MedLabContext>()
                .UseInMemoryDatabase(databaseName: $"TestDb_{System.Guid.NewGuid()}")
                .Options;
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

            var patient = new User
            {
                Login = "pacjent_testowy",
                Password = "haslo123",
                UserType = 4,
                IsActive = true,
                Name = "Jan",
                Surname = "Kowalski",
                PESEL = "12345678901",
                PhoneNumber = "1234567890",
            };
            context.Users.Add(patient);
            context.SaveChanges();

            var nurse = new User
            {
                Login = "nurse_testowa",
                Password = "haslo123",
                UserType = 2,
                IsActive = true,
                Name = "Anna",
                Surname = "Nowak",
                PESEL = "98765432101",
                PhoneNumber = "1234567890",
            };
            context.Users.Add(nurse);
            context.SaveChanges();

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

            // Init test context
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

            // Act
            var nameTaken = DbManager.IsTestNameTaken(testName);
            Assert.IsFalse(nameTaken, "Nazwa testu powinna byæ wolna");

            var result = DbManager.AddTest(test);

            // Assert
            Assert.IsTrue(result, "Dodanie testu przez DbManager powinno siê powieœæ");

            var savedTest = context.Tests.FirstOrDefault(t => t.TestName == testName);
            Assert.IsNotNull(savedTest, "Test powinien zostaæ zapisany w bazie");
            Assert.AreEqual("Opis testu integracyjnego", savedTest.Description);
            Assert.AreEqual(category.id, savedTest.Category);
        }

        [TestMethod]
        public void IntegrationTest_ShouldAddNewUser_WhenValidInputs()
        {
            // Arrange
            using var context = new MedLabContext(_contextOptions);

            // Init test context
            DbManager.InitForTesting(context);

            // Dane testowe, symuluj¹ce dane z formularza
            string name = "Jan";
            string surname = "Kowalski";
            string pesel = "12345678901";
            string phone = "123456789";
            string login = "jan.kowalski";
            string password = "haslo123";
            string repeatPassword = "haslo123";
            int userType = 4; // Pacjent

            // Symulacja walidacji
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

            // Sprawdzenie, czy login i PESEL s¹ wolne
            Assert.IsFalse(DbManager.IsLoginTaken(login), "Login nie powinien byæ zajêty.");
            Assert.IsFalse(DbManager.IsPESELTaken(pesel), "PESEL nie powinien byæ zajêty.");

            // Utworzenie u¿ytkownika
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

            // Act – dodanie u¿ytkownika do bazy
            bool result = DbManager.AddUser(newUser);

            // Assert
            Assert.IsTrue(result, "U¿ytkownik powinien zostaæ poprawnie dodany.");
            var savedUser = context.Users.FirstOrDefault(u => u.Login == login);
            Assert.IsNotNull(savedUser);
            Assert.AreEqual(name, savedUser.Name);
            Assert.AreEqual(surname, savedUser.Surname);
            Assert.AreEqual(pesel, savedUser.PESEL);
            Assert.AreEqual(phone, savedUser.PhoneNumber);
            Assert.AreEqual(userType, savedUser.UserType);
        }

    }
}
