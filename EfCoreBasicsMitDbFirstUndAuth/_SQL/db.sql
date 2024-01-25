USE master
GO

CREATE DATABASE EfCoreDbFirstAuth
GO

USE EfCoreDbFirstAuth
GO

CREATE TABLE AppRoles
(
	Id int identity primary key,
	RoleName varchar(50) unique not null
)
GO

INSERT INTO AppRoles VALUES('User')
GO

INSERT INTO AppRoles VALUES('Admin')
GO

CREATE TABLE AppUser
(
  Id int identity primary key,
  Username varchar(100) unique not null,
  PwHash binary(32) not null,
  Salt binary(32) not null,
  RoleId int foreign key references AppRoles(Id) not null
)
GO

--Optionen für Rollen

--bit im AppUser für Admin ja/nein (Unterstützt max. 2 verschiedenen Rollen)
--varchar im AppUser für Rollenname (Unterstützt beliebig viele Rollen; Achtung: Tabelle ist dann nicht in der 2NF)
--Separate Tabelle in der Rollennamen definiert sind + FK beim AppUser (Unterstützt beliebig viele Rollen)
--Separate Tabelle in der Rollennamen definiert sind + Zwischentabelle (Unterstützt beliebig viele Rollen, ein Benutzer kann gleichzeitig mehr als eine Rolle haben)