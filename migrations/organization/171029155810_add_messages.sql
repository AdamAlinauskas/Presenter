START TRANSACTION;

CREATE TABLE messages (
    id bigserial PRIMARY KEY,
    text text NOT NULL,
    conversation_id bigint NOT NULL REFERENCES conversations(id), -- Might become nullable when we implement DMs
    replies_to bigint NULL REFERENCES messages(id),
    created_at timestamp NOT NULL DEFAULT (now() AT TIME ZONE 'UTC'),
    created_by bigint NOT NULL
);

CREATE TABLE message_events (
    id bigserial PRIMARY KEY,
    message_id bigint NOT NULL REFERENCES messages(id),
    event_type text NOT NULL, -- Might want to be an enum type at some point, but unsure on design
    created_at timestamp NOT NULL DEFAULT (now() AT TIME ZONE 'UTC'),
    created_by bigint NOT NULL
);

COMMIT;
