-- Migration: addorgs
-- Created at: 2017-10-15 21:56:41
-- ====  UP  ====

BEGIN;

CREATE TABLE Organizations (
	Id BIGSERIAL NOT NULL PRIMARY KEY,
	Name TEXT NOT NULL
);

INSERT INTO Organizations (Name) VALUES ('Default');

COMMIT;

-- ==== DOWN ====

BEGIN;

DROP TABLE Organizations;

COMMIT;
