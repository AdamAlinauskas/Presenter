START TRANSACTION;

CREATE TABLE user_analytics_sessions (
    id bigserial PRIMARY KEY,
    presentation_id bigint REFERENCES presentations(id),
    document_id bigint REFERENCES files(id),
    created_at timestamp NOT NULL DEFAULT (now() AT TIME ZONE 'UTC'),
    created_by bigint NOT NULL,
    longitude numeric,
    latitude numeric,
    country text,
    ip_address text,
    CONSTRAINT chk_user_analytics_sessions CHECK ( (presentation_id IS NOT NULL AND document_id IS NULL) 
                                            or 
                                            (document_id IS NOT NULL AND presentation_id IS NULL) )
);

CREATE TABLE page_analytics (
    id bigserial PRIMARY KEY,
    user_analytics_sessions_id bigint REFERENCES user_analytics_sessions(id),
    page_number numeric NOT NULL,
    time_on_page numeric NOT NULL DEFAULT(0)
);

COMMIT;
