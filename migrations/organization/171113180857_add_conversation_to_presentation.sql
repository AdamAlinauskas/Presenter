START TRANSACTION;

ALTER TABLE presentations
ADD COLUMN conversation_id bigint NOT NULL,
ADD CONSTRAINT fk_conversation FOREIGN KEY (conversation_id) REFERENCES conversations (id);

COMMIT;
