INSERT INTO users (
    first_name,
    last_name,
    email,
    password_hash,
    role_id,
    department,
    designation,
    date_of_joining,
    is_active
)
VALUES (
    'System',
    'Admin',
    'admin@company.com',
    '$2a$10$K/poCT6n2ml5PC1H02753e7DeRnx4myVdD7JlXW6sksxTI3pFoI7S',
    (SELECT id FROM roles WHERE role_name = 'Admin'),
    'Information Technology',
    'System Administrator',
    CURRENT_DATE,
    TRUE
)
ON CONFLICT ON CONSTRAINT uq_users_email DO NOTHING;

