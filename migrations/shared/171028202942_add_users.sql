START TRANSACTION;

-- Migration here
CREATE TABLE users (
	id BIGSERIAL PRIMARY KEY,
    email text,
    name text,
    picture text,
    identifier text UNIQUE -- Auth0 provided id
);

COMMIT;
