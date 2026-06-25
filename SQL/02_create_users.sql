CREATE TABLE IF NOT EXISTS users (
    id               SERIAL         PRIMARY KEY,
    first_name       VARCHAR(100)   NOT NULL,
    last_name        VARCHAR(100)   NOT NULL,
    email            VARCHAR(200)   NOT NULL,
    password_hash    TEXT           NOT NULL,
    role_id          INT            NOT NULL,
    department       VARCHAR(100),
    designation      VARCHAR(100),
    date_of_joining  DATE,
    is_active        BOOLEAN        NOT NULL DEFAULT TRUE,
    created_at       TIMESTAMP      NOT NULL DEFAULT CURRENT_TIMESTAMP,
    updated_at       TIMESTAMP      NOT NULL DEFAULT CURRENT_TIMESTAMP,

    CONSTRAINT uq_users_email
        UNIQUE (email),

    CONSTRAINT fk_users_role_id
        FOREIGN KEY (role_id)
        REFERENCES roles(id)
        ON DELETE RESTRICT
        ON UPDATE CASCADE,

    CONSTRAINT chk_users_email_format
        CHECK (email ~* '^[A-Za-z0-9._%+\-]+@[A-Za-z0-9.\-]+\.[A-Za-z]{2,}$'),

    CONSTRAINT chk_users_names_not_empty
        CHECK (
            LENGTH(TRIM(first_name)) > 0
            AND LENGTH(TRIM(last_name)) > 0
        )
);

CREATE UNIQUE INDEX IF NOT EXISTS idx_users_email
    ON users(email);

CREATE INDEX IF NOT EXISTS idx_users_role_id
    ON users(role_id);

CREATE INDEX IF NOT EXISTS idx_users_is_active
    ON users(is_active)
    WHERE is_active = TRUE;
