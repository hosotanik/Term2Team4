CREATE DATABASE  ToReserveDB;

CREATE TABLE reservation
(
	id INT PRIMARY KEY IDENTITY(1,1),
	conference_name NVARCHAR(20) NOT NULL,
	start_at DATETIME NOT NULL,
	end_at DATETIME NOT NULL,
	reservation_name NVARCHAR(20) NOT NULL
);

INSERT INTO reservation(conference_name,start_at,end_at,reservation_name)
VALUES(N'会議室A','2026-06-09 10:00:00','2026-06-09 11:00:00',N'細谷')

SELECT * FROM reservation

drop table reservation;