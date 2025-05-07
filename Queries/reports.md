IF NOT EXISTS (SELECT * FROM sysobjects WHERE name = 'Reports' AND xtype = 'U')
BEGIN
    CREATE TABLE Reports (
        id INT IDENTITY(1,1) NOT NULL,
        SampleId INT NOT NULL,
        LastUpdateDate DATE NOT NULL,
        LastUpdateTime TIME NOT NULL,
        Results NVARCHAR(MAX) NOT NULL,
        PRIMARY KEY(id),
        FOREIGN KEY (SampleId) REFERENCES TestHistory(id)
        ON UPDATE CASCADE ON DELETE NO ACTION,
    );
END;

INSERT INTO Reports (SampleId,CreationDate, CreationTime, Results)
VALUES 
(10,CAST('2024-05-14'AS DATE), CAST('12:15:00.00000000' AS TIME (7)), 'Wyniki badania'),
(11,CAST('2024-05-14'AS DATE), CAST('12:15:00.00000000' AS TIME (7)), 'Wyniki badania')