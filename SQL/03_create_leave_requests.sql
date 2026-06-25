DO $$
BEGIN
    IF NOT EXISTS (
        SELECT 1 FROM pg_type WHERE typname = 'leave_status'
    ) THEN
        CREATE TYPE leave_status AS ENUM ('Pending', 'Approved', 'Rejected');
    END IF;
END
$$;

CREATE TABLE IF NOT EXISTS leave_requests (
    id              SERIAL          PRIMARY KEY,
    employee_id     INT             NOT NULL,
    from_date       DATE            NOT NULL,
    to_date         DATE            NOT NULL,
    reason          TEXT            NOT NULL,
    status          leave_status    NOT NULL DEFAULT 'Pending',
    reviewed_by     INT,
    reviewed_at     TIMESTAMP,
    remarks         TEXT,
    created_at      TIMESTAMP       NOT NULL DEFAULT CURRENT_TIMESTAMP,
    updated_at      TIMESTAMP       NOT NULL DEFAULT CURRENT_TIMESTAMP,

    CONSTRAINT fk_lr_employee
        FOREIGN KEY (employee_id)
        REFERENCES users(id)
        ON DELETE CASCADE
        ON UPDATE CASCADE,

    CONSTRAINT fk_lr_reviewer
        FOREIGN KEY (reviewed_by)
        REFERENCES users(id)
        ON DELETE SET NULL
        ON UPDATE CASCADE,

    CONSTRAINT chk_lr_date_order
        CHECK (to_date >= from_date),

    CONSTRAINT chk_lr_reason_not_empty
        CHECK (LENGTH(TRIM(reason)) > 0),

    CONSTRAINT chk_lr_review_consistency
        CHECK (
            (reviewed_by IS NULL AND reviewed_at IS NULL)
            OR
            (reviewed_by IS NOT NULL AND reviewed_at IS NOT NULL)
        ),

    CONSTRAINT chk_lr_remarks_only_after_review
        CHECK (
            remarks IS NULL
            OR reviewed_by IS NOT NULL
        )
);

CREATE INDEX IF NOT EXISTS idx_lr_employee_id
    ON leave_requests(employee_id);

CREATE INDEX IF NOT EXISTS idx_lr_status
    ON leave_requests(status);

CREATE INDEX IF NOT EXISTS idx_lr_overlap_check
    ON leave_requests(employee_id, from_date, to_date)
    WHERE status IN ('Pending', 'Approved');

CREATE INDEX IF NOT EXISTS idx_lr_created_at
    ON leave_requests(created_at DESC);
