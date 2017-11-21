START TRANSACTION;

ALTER TABLE user_analytics_sessions
ADD COLUMN city text;

ALTER TABLE user_analytics_sessions
ADD COLUMN continent text;

COMMIT;
