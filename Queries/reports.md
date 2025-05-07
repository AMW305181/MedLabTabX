IF NOT EXISTS (SELECT * FROM sysobjects WHERE name = 'Reports' AND xtype = 'U')
BEGIN
    CREATE TABLE Reports (
        id INT IDENTITY(1,1) NOT NULL,
        SampleId INT NOT NULL,
        PatientId INT NOT NULL,
        NurseId INT NOT NULL,
        AnalystId INT NOT NULL,
        CreationDate DATE NOT NULL,
        CreationTime TIME NOT NULL,
        Results NVARCHAR(MAX) NOT NULL,
        PRIMARY KEY(id),
        FOREIGN KEY (SampleId) REFERENCES TestHistory(id)
        ON UPDATE CASCADE ON DELETE NO ACTION,
        FOREIGN KEY (PatientId) REFERENCES Users(id)
        ON UPDATE NO ACTION ON DELETE NO ACTION,
        FOREIGN KEY (NurseId) REFERENCES Users(id)
        ON UPDATE NO ACTION ON DELETE NO ACTION,
        FOREIGN KEY (AnalystId) REFERENCES Users(id)
        ON UPDATE NO ACTION ON DELETE NO ACTION
    );
END;

INSERT INTO Reports (SampleId, PatientId, NurseId, AnalystId,CreationDate, CreationTime, Results)
VALUES 
(10, 4,2, 3,CAST('2024-05-14'AS DATE), CAST('12:15:00.00000000' AS TIME (7)), 'Wyniki badania'),
(11,4,2,3,CAST('2024-05-14'AS DATE), CAST('12:15:00.00000000' AS TIME (7)), 'Wyniki badania')