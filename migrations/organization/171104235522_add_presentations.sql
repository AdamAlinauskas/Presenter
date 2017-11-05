START TRANSACTION;

CREATE TABLE presentations (
    id bigserial PRIMARY KEY,
    document_id bigint NOT NULL,
    name varchar(500) NOT NULL,
    created_at timestamp NOT NULL DEFAULT (now() AT TIME ZONE 'UTC'),
    created_by bigint NOT NULL
);

COMMIT;
