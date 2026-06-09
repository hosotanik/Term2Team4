CREATE DATABASE  ToReserveDB;

CREATE TABLE reservation
(
	id INT PRIMARY KEY IDENTITY(1,1),
	conference_name NVARCHAR(20) NOT NULL,
	start_date DATETIME NOT NULL,
	end_date DATETIME NOT NULL,
	reservation_name NVARCHAR(20) NOT NULL
);

INSERT INTO reservation(conference_name,start_date,end_date,reservation_name)
VALUES(N'会議室A','2026-06-09 10:00:00','2026-06-15 11:00:00',N'細谷')

SELECT * FROM reservation