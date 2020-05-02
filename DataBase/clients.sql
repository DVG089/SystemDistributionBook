USE systemdistributionbook;
 
CREATE TABLE clients
(
    Address VARCHAR(30) PRIMARY KEY,
    Surname VARCHAR(30) NOT NULL,
    Name VARCHAR(30) NOT NULL,
    PagesPerDay INT NOT NULL,
    ReadingIntervalActive INT NOT NULL,
    ReadingIntervalPassive INT NOT NULL,
    DataRegistration DATETIME NOT NULL,
    Subscription VARCHAR(10) NOT NULL
);