CREATE TABLE IF NOT EXISTS roles (
    id          SERIAL PRIMARY KEY,
    role_name   VARCHAR(50)  NOT NULL,
    created_at  TIMESTAMP    NOT NULL DEFAULT CURRENT_TIMESTAMP,

    CONSTRAINT uq_roles_role_name UNIQUE (role_name)
);

INSERT INTO roles (role_name) VALUES ('Admin')
    ON CONFLICT ON CONSTRAINT uq_roles_role_name DO NOTHING;

INSERT INTO roles (role_name) VALUES ('Employee')
    ON CONFLICT ON CONSTRAINT uq_roles_role_name DO NOTHING;
