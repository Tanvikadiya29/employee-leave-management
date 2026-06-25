CREATE OR REPLACE FUNCTION check_leave_overlap(
    p_employee_id   INT,
    p_from_date     DATE,
    p_to_date       DATE,
    p_exclude_id    INT DEFAULT NULL
)
RETURNS BOOLEAN
LANGUAGE plpgsql
STABLE
AS $$
DECLARE
    v_count INT;
BEGIN
    SELECT COUNT(*) INTO v_count
    FROM leave_requests
    WHERE employee_id = p_employee_id
      AND status IN ('Pending', 'Approved')
      AND (p_exclude_id IS NULL OR id <> p_exclude_id)
      AND from_date <= p_to_date
      AND to_date   >= p_from_date;

    RETURN v_count > 0;
END;
$$;

CREATE OR REPLACE FUNCTION update_updated_at_column()
RETURNS TRIGGER
LANGUAGE plpgsql
AS $$
BEGIN
    NEW.updated_at = CURRENT_TIMESTAMP;
    RETURN NEW;
END;
$$;

DROP TRIGGER IF EXISTS trg_users_updated_at ON users;
CREATE TRIGGER trg_users_updated_at
    BEFORE UPDATE ON users
    FOR EACH ROW
    EXECUTE FUNCTION update_updated_at_column();

DROP TRIGGER IF EXISTS trg_lr_updated_at ON leave_requests;
CREATE TRIGGER trg_lr_updated_at
    BEFORE UPDATE ON leave_requests
    FOR EACH ROW
    EXECUTE FUNCTION update_updated_at_column();
