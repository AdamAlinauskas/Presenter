START TRANSACTION;

CREATE TABLE organizations (
	id SERIAL PRIMARY KEY,
	schema_name text NOT NULL UNIQUE,
	display_name character varying(50) NOT NULL,
	CONSTRAINT allowed_schema CHECK (schema_name ~ '^[a-z][a-z0-9]+$')
	-- Restrict to names which are valid subdomains AND schema names
);

COMMIT;
