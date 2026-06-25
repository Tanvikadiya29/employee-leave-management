CREATE OR REPLACE VIEW v_leave_requests_detailed AS
SELECT
    lr.id                                               AS leave_id,
    lr.employee_id,
    u.first_name  || ' ' || u.last_name                AS employee_name,
    u.email                                             AS employee_email,
    u.department,
    u.designation,
    lr.from_date,
    lr.to_date,
    (lr.to_date - lr.from_date + 1)                    AS total_days,
    lr.reason,
    lr.status,
    lr.remarks,
    lr.reviewed_at,
    rev.first_name || ' ' || rev.last_name             AS reviewed_by_name,
    lr.created_at                                       AS applied_on
FROM leave_requests lr
JOIN  users u   ON u.id   = lr.employee_id
LEFT JOIN users rev ON rev.id = lr.reviewed_by
ORDER BY lr.created_at DESC;

CREATE OR REPLACE VIEW v_employee_leave_summary AS
SELECT
    u.id                                                AS employee_id,
    u.first_name || ' ' || u.last_name                 AS employee_name,
    COUNT(*) FILTER (WHERE lr.status = 'Pending')       AS pending_count,
    COUNT(*) FILTER (WHERE lr.status = 'Approved')      AS approved_count,
    COUNT(*) FILTER (WHERE lr.status = 'Rejected')      AS rejected_count,
    COUNT(*)                                            AS total_requests,
    SUM(lr.to_date - lr.from_date + 1)
        FILTER (WHERE lr.status = 'Approved')           AS approved_days_taken
FROM users u
LEFT JOIN leave_requests lr ON lr.employee_id = u.id
WHERE u.is_active = TRUE
  AND u.role_id   = (SELECT id FROM roles WHERE role_name = 'Employee')
GROUP BY u.id, u.first_name, u.last_name
ORDER BY u.first_name;

