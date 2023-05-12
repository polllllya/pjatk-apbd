-- Created by Vertabelo (http://vertabelo.com)
-- Last modification date: 2023-03-27 14:47:43.379

-- tables
-- Table: Animal
CREATE TABLE Animal (
    IdAnimal int  NOT NULL IDENTITY,
    Name nvarchar(200)  NOT NULL,
    Description nvarchar(200)  NULL,
    Category nvarchar(200)  NOT NULL,
    Area nvarchar(200)  NOT NULL,
    CONSTRAINT Animal_pk PRIMARY KEY  (IdAnimal)
);

-- End of file.

