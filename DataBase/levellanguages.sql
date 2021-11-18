use systemdistributionbook;

CREATE TABLE levellanguages
(
	AddressClient VARCHAR(30) NOT NULL,
    Language ENUM("Русский", "Английский", "Немецкий", "Итальянский", "Испанский") NOT NULL,
    Level INT NOT NULL,
    FOREIGN KEY (AddressClient) REFERENCES clients (Address) ON DELETE CASCADE
);