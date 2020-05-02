use systemdistributionbook;

CREATE TABLE books
(
	AddressClient VARCHAR(30) NOT NULL,
    Language ENUM("Русский", "Английский", "Немецкий", "Итальянский", "Испанский") NOT NULL,
    Name VARCHAR(20) NOT NULL,
    Pages INT NOT NULL,
    DataGetting DATETIME NOT NULL,
    DataReading DATETIME,
    Id INT PRIMARY KEY AUTO_INCREMENT,
    FOREIGN KEY (AddressClient) REFERENCES clients (Address) ON DELETE CASCADE
);