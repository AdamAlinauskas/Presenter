START TRANSACTION;


ALTER TABLE user_analytics_sessions
ADD COLUMN duration numeric NOT NULL DEFAULT(0);

COMMIT;
