START TRANSACTION;

CREATE TABLE conversations (
    id bigserial PRIMARY KEY,
    topic text,
    created_at timestamp NOT NULL DEFAULT (now() AT TIME ZONE 'UTC'),
    created_by bigint
);

COMMIT;
