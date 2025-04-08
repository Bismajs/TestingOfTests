CREATE DATABASE StudyRooms;

CREATE TABLE [dbo].[StudyRooms] (
    [Id]   INT            IDENTITY (1, 1) NOT NULL,
    [Name] NVARCHAR (MAX) NOT NULL,
    CONSTRAINT [PK_StudyRooms] PRIMARY KEY CLUSTERED ([Id] ASC)
);

-- Lägg till studierum
INSERT INTO StudyRooms (Name) VALUES ('Jupiter');
INSERT INTO StudyRooms (Name) VALUES ('Telescope');
INSERT INTO StudyRooms (Name) VALUES ('Science');