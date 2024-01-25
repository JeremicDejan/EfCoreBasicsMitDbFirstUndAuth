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

--Optionen f�r Rollen

--bit im AppUser f�r Admin ja/nein (Unterst�tzt max. 2 verschiedenen Rollen)
--varchar im AppUser f�r Rollenname (Unterst�tzt beliebig viele Rollen; Achtung: Tabelle ist dann nicht in der 2NF)
--Separate Tabelle in der Rollennamen definiert sind + FK beim AppUser (Unterst�tzt beliebig viele Rollen)
--Separate Tabelle in der Rollennamen definiert sind + Zwischentabelle (Unterst�tzt beliebig viele Rollen, ein Benutzer kann gleichzeitig mehr als eine Rolle haben)