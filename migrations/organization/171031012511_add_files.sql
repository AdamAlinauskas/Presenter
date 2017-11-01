START TRANSACTION;

CREATE TABLE files (
    id bigserial PRIMARY KEY,
    file_name varchar(200) NOT NULL,
    awsKey varchar(500) NOT NULL,
    created_at timestamp NOT NULL DEFAULT (now() AT TIME ZONE 'UTC'),
    created_by bigint NOT NULL
);

COMMIT;
