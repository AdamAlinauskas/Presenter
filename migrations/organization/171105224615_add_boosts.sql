START TRANSACTION;

CREATE TABLE boosts (
    id bigserial PRIMARY KEY,
    message_id bigint NOT NULL REFERENCES messages(id),
    created_at timestamp NOT NULL DEFAULT (now() AT TIME ZONE 'UTC'),
    created_by bigint NOT NULL,
    UNIQUE(message_id, created_by)
);

COMMIT;
