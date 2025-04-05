IF NOT EXISTS (SELECT * FROM sysobjects WHERE name = 'StatusDictionary' AND xtype = 'U')
BEGIN
    CREATE TABLE StatusDictionary (
        id INT IDENTITY(1,1) NOT NULL, 
        StatusName NVARCHAR(255) NOT NULL,
        PRIMARY KEY(id)
    );
END;

IF NOT EXISTS (SELECT * FROM sysobjects WHERE name = 'UserTypes' AND xtype = 'U')
BEGIN
    CREATE TABLE UserTypes (
        id INT IDENTITY(1,1) NOT NULL,
        TypeName NVARCHAR(255) NOT NULL,
        PRIMARY KEY(id)
    );
END;

IF NOT EXISTS (SELECT * FROM sysobjects WHERE name = 'CategoryDictionary' AND xtype = 'U')
BEGIN
    CREATE TABLE CategoryDictionary (
        id INT IDENTITY(1,1) NOT NULL,
        CategoryName NVARCHAR(255) NOT NULL,
        PRIMARY KEY(id)
    );
END;

IF NOT EXISTS (SELECT * FROM sysobjects WHERE name = 'Users' AND xtype = 'U')
BEGIN
    CREATE TABLE Users (
        id INT IDENTITY(1,1) NOT NULL,
        Login VARCHAR(255) NOT NULL,
        UserType INT NOT NULL,
        Password VARCHAR(20) NOT NULL,
        IsActive BIT NOT NULL,
        Name NVARCHAR(255) NOT NULL,
        Surname NVARCHAR(255) NOT NULL,
        PESEL VARCHAR(11) NOT NULL,
        PhoneNumber VARCHAR(20),
        PRIMARY KEY(id),
        FOREIGN KEY (UserType) REFERENCES UserTypes(id)
        ON UPDATE CASCADE ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (SELECT * FROM sysobjects WHERE name = 'Tests' AND xtype = 'U')
BEGIN
    CREATE TABLE Tests (
        id INT IDENTITY(1,1) NOT NULL,
        TestName NVARCHAR(255) NOT NULL,
        Description NVARCHAR(MAX) NOT NULL,
        Price REAL NOT NULL,
        Category INT NOT NULL,
        IsActive BIT NOT NULL,
        PRIMARY KEY(id),
        FOREIGN KEY (Category) REFERENCES CategoryDictionary(id)
        ON UPDATE CASCADE ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (SELECT * FROM sysobjects WHERE name = 'Schedules' AND xtype = 'U')
BEGIN
    CREATE TABLE Schedules (
        id INT IDENTITY(1,1) NOT NULL,
        NurseId INT NOT NULL,
        Date DATE NOT NULL,
        Time TIME NOT NULL,
        PRIMARY KEY(id),
        FOREIGN KEY (NurseId) REFERENCES Users(id)
        ON UPDATE CASCADE ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (SELECT * FROM sysobjects WHERE name = 'Visits' AND xtype = 'U')
BEGIN
    CREATE TABLE Visits (
        id INT IDENTITY(1,1) NOT NULL,
        Cost REAL NOT NULL,
        PaymentStatus BIT NOT NULL,
        IsActive BIT NOT NULL,
        PatientId INT NOT NULL,
        TimeSlotId INT,
        PRIMARY KEY(id),
        FOREIGN KEY (PatientId) REFERENCES Users(id)
        ON UPDATE CASCADE ON DELETE NO ACTION,
		FOREIGN KEY (TimeSlotId) REFERENCES Schedules(id)
        ON UPDATE NO ACTION ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (SELECT * FROM sysobjects WHERE name = 'TestHistory' AND xtype = 'U')
BEGIN
    CREATE TABLE TestHistory (
        id INT IDENTITY(1,1) NOT NULL,
        VisitId INT NOT NULL,
        TestId INT NOT NULL,
        PatientId INT NOT NULL,
        Status INT NOT NULL,
        AnalystId INT,
        PRIMARY KEY(id),
        FOREIGN KEY (Status) REFERENCES StatusDictionary(id)
        ON UPDATE CASCADE ON DELETE NO ACTION,
        FOREIGN KEY (TestId) REFERENCES Tests(id)
        ON UPDATE NO ACTION ON DELETE NO ACTION,
        FOREIGN KEY (VisitId) REFERENCES Visits(id)
        ON UPDATE CASCADE ON DELETE NO ACTION,
        FOREIGN KEY (AnalystId) REFERENCES Users(id)
        ON UPDATE NO ACTION ON DELETE NO ACTION,
        FOREIGN KEY (PatientId) REFERENCES Users(id)
        ON UPDATE NO ACTION ON DELETE NO ACTION
    );
END;


Scaffold-DbContext "Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=MedLab;Integrated Security=True;Connect Timeout=30;Encrypt=False;Trust Server Certificate=False;Application Intent=ReadWrite;Multi Subnet Failover=False" Microsoft.EntityFrameworkCore.SqlServer -OutputDir DatabaseModels -DataAnnotations -force -UseDatabaseNames






