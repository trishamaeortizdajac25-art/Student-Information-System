CREATE DATABASE IF NOT EXISTS sis_db
    CHARACTER SET utf8mb4
    COLLATE utf8mb4_unicode_ci;

USE sis_db;

-- Drop in dependency order so the script can be re-run cleanly
DROP VIEW  IF EXISTS vw_year_level_counts;
DROP VIEW  IF EXISTS vw_program_counts;
DROP VIEW  IF EXISTS vw_student_information;
DROP TABLE IF EXISTS activity_logs;
DROP TABLE IF EXISTS students;
DROP TABLE IF EXISTS teachers;
DROP TABLE IF EXISTS programs;
DROP TABLE IF EXISTS year_levels;

-- ============================================================
-- 1. LOOKUP TABLES (feed the ComboBoxes / quick filters)
-- ============================================================
CREATE TABLE year_levels (
    year_level  VARCHAR(20) NOT NULL,
    sort_order  TINYINT UNSIGNED NOT NULL,
    PRIMARY KEY (year_level)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

CREATE TABLE programs (
    program_id    INT UNSIGNED NOT NULL AUTO_INCREMENT,
    program_name  VARCHAR(100) NOT NULL,
    PRIMARY KEY (program_id),
    UNIQUE KEY uq_programs_name (program_name)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- ============================================================
-- 2. STUDENTS (core table + student login account)
-- ============================================================
CREATE TABLE students (
    student_id       INT UNSIGNED NOT NULL AUTO_INCREMENT,
    student_number   VARCHAR(30)  NOT NULL,              -- visible Student ID, e.g. 2026-0001
    first_name       VARCHAR(50)  NOT NULL,
    middle_name      VARCHAR(50)  NULL,
    last_name        VARCHAR(50)  NOT NULL,
    gender           ENUM('Male','Female','Other','Prefer not to say') NOT NULL,
    birthdate        DATE         NOT NULL,
    year_level       VARCHAR(20)  NOT NULL,
    program          VARCHAR(100) NOT NULL,
    block            VARCHAR(20)  NOT NULL,              -- section, e.g. Block A
    contact_number   VARCHAR(20)  NOT NULL,              -- VARCHAR keeps the leading zero
    email            VARCHAR(100) NULL,
    address          VARCHAR(255) NULL,
    status           ENUM('Active','Graduated','Dropped','On Leave') NOT NULL DEFAULT 'Active',
    username         VARCHAR(50)  NOT NULL,              -- student login
    password_hash    VARCHAR(255) NOT NULL,              -- hashed, never plain text
    profile_picture  MEDIUMBLOB   NULL,                  -- optional photo (up to 16 MB)
    is_deleted       TINYINT(1)   NOT NULL DEFAULT 0,    -- soft delete (recycle bin)
    created_at       TIMESTAMP    NOT NULL DEFAULT CURRENT_TIMESTAMP,
    updated_at       TIMESTAMP    NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
    PRIMARY KEY (student_id),
    UNIQUE KEY uq_students_number   (student_number),
    UNIQUE KEY uq_students_username (username),
    CONSTRAINT fk_students_year_level FOREIGN KEY (year_level)
        REFERENCES year_levels (year_level) ON UPDATE CASCADE ON DELETE RESTRICT,
    CONSTRAINT fk_students_program FOREIGN KEY (program)
        REFERENCES programs (program_name) ON UPDATE CASCADE ON DELETE RESTRICT,
    INDEX idx_students_year_level (year_level),
    INDEX idx_students_gender     (gender),
    INDEX idx_students_name       (last_name, first_name),
    INDEX idx_students_program    (program),
    INDEX idx_students_status     (status)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- ============================================================
-- 3. TEACHERS (no public sign-up; accounts are created by an admin)
-- ============================================================
CREATE TABLE teachers (
    teacher_id      INT UNSIGNED NOT NULL AUTO_INCREMENT,
    teacher_number  VARCHAR(30)  NOT NULL,
    first_name      VARCHAR(50)  NOT NULL,
    middle_name     VARCHAR(50)  NULL,
    last_name       VARCHAR(50)  NOT NULL,
    username        VARCHAR(50)  NOT NULL,
    password_hash   VARCHAR(255) NOT NULL,
    is_active       TINYINT(1)   NOT NULL DEFAULT 1,
    created_at      TIMESTAMP    NOT NULL DEFAULT CURRENT_TIMESTAMP,
    PRIMARY KEY (teacher_id),
    UNIQUE KEY uq_teachers_number   (teacher_number),
    UNIQUE KEY uq_teachers_username (username)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- ============================================================
-- 4. ACTIVITY LOGS (who did what, and when)
-- No foreign keys on purpose: logs must survive permanent deletes.
-- ============================================================
CREATE TABLE activity_logs (
    log_id            INT UNSIGNED NOT NULL AUTO_INCREMENT,
    actor_type        ENUM('Student','Teacher') NOT NULL,
    actor_id          INT UNSIGNED NULL,
    action            ENUM('LOGIN','REGISTER','ADD','EDIT','DELETE','RESTORE') NOT NULL,
    target_student_id INT UNSIGNED NULL,
    details           VARCHAR(255) NULL,
    logged_at         TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    PRIMARY KEY (log_id),
    INDEX idx_logs_logged_at (logged_at)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- ============================================================
-- 5. VIEWS
-- ============================================================

-- Student list with calculated full name and age (no photo, so it stays fast)
CREATE VIEW vw_student_information AS
SELECT
    student_id,
    student_number,
    first_name,
    middle_name,
    last_name,
    CONCAT(first_name, ' ', COALESCE(CONCAT(middle_name, ' '), ''), last_name) AS full_name,
    gender,
    year_level,
    program,
    block,
    birthdate,
    TIMESTAMPDIFF(YEAR, birthdate, CURDATE()) AS age,
    contact_number,
    email,
    address,
    status
FROM students
WHERE is_deleted = 0;

-- Teacher homepage: student count per year level (shows 0 for empty levels).
-- A view cannot be relied on for ordering, so ORDER BY sort_order when you SELECT from it.
CREATE VIEW vw_year_level_counts AS
SELECT
    yl.year_level,
    yl.sort_order,
    COUNT(s.student_id) AS total_students
FROM year_levels yl
LEFT JOIN students s
       ON s.year_level = yl.year_level AND s.is_deleted = 0
GROUP BY yl.year_level, yl.sort_order;

-- Dashboard: student count per program
CREATE VIEW vw_program_counts AS
SELECT
    p.program_name,
    COUNT(s.student_id) AS total_students
FROM programs p
LEFT JOIN students s
       ON s.program = p.program_name AND s.is_deleted = 0
GROUP BY p.program_id, p.program_name
ORDER BY p.program_name;

-- ============================================================
-- 6. SAMPLE DATA
-- Passwords are stored as SHA-256 hex hashes (SHA2(..., 256)).
--
-- LOGINS YOU CAN TRY:
--   Teacher : jonpel       / pass1
--             lasteph      / pass12
--             arsat        / pass123
--   Student : test.student / test123
--             juan.delacruz (and the other students) / student123
-- ============================================================

-- Year levels MUST match the values used in the students below (foreign key).
INSERT INTO year_levels (year_level, sort_order) VALUES
    ('1st Year', 1),
    ('2nd Year', 2),
    ('3rd Year', 3),
    ('4th Year', 4);

-- The ONLY five programs in the system: BSIT, BSCS, BSMet, BSBio, BSChem.
-- Every program used by a student below MUST be listed here (foreign key).
INSERT INTO programs (program_name) VALUES
    ('BS Information Technology'),
    ('BS Computer Science'),
    ('BS Meteorology'),
    ('BS Biology'),
    ('BS Chemistry');

-- Teachers (final names/logins already applied)
INSERT INTO teachers (teacher_number, first_name, middle_name, last_name, username, password_hash) VALUES
    ('T-0001', 'Jonathan',      NULL, 'Pelon',   'jonpel',  SHA2('pass1', 256)),
    ('T-0002', 'Lance Stephen', NULL, 'Bronzal', 'lasteph', SHA2('pass12', 256)),
    ('T-0003', 'Arlene',        NULL, 'Satuito', 'arsat',   SHA2('pass123', 256));

INSERT INTO students
    (student_number, first_name, middle_name, last_name, gender, birthdate,
     year_level, program, block, contact_number, email, address, status, username, password_hash)
VALUES
    ('2026-0001', 'Juan',    'Santos',  'Dela Cruz',  'Male',   '2008-03-14', '1st Year',  'BS Information Technology',  'A', '09171234567', 'juan.delacruz@example.com',   'Legazpi City, Albay', 'Active',    'juan.delacruz',   SHA2('student123', 256)),
    ('2026-0002', 'Maria',   NULL,      'Santos',     'Female', '2007-07-22', '1st Year',  'BS Computer Science',        'B', '09281234568', 'maria.santos@example.com',    'Daraga, Albay',       'Active',    'maria.santos',    SHA2('student123', 256)),
    ('2026-0003', 'Angela',  'Cruz',    'Garcia',     'Female', '2008-01-30', '1st Year',  'BS Biology',                 'A', '09451234570', 'angela.garcia@example.com',   'Tabaco City, Albay',  'Active',    'angela.garcia',   SHA2('student123', 256)),
    ('2026-0004', 'Sofia',   NULL,      'Ramos',      'Female', '2008-08-11', '1st Year',  'BS Information Technology',  'C', '09891234574', 'sofia.ramos@example.com',     'Guinobatan, Albay',   'Active',    'sofia.ramos',     SHA2('student123', 256)),
    ('2025-0011', 'Katrina', 'Lim',     'Mendoza',    'Female', '2006-05-09', '2nd Year',  'BS Information Technology',  'A', '09671234572', 'katrina.mendoza@example.com', 'Polangui, Albay',     'Active',    'katrina.mendoza', SHA2('student123', 256)),
    ('2025-0012', 'Bianca',  NULL,      'Torres',     'Female', '2006-10-16', '2nd Year',  'BS Computer Science',        'B', '09181234576', 'bianca.torres@example.com',   'Daraga, Albay',       'Active',    'bianca.torres',   SHA2('student123', 256)),
    ('2025-0013', 'Miguel',  'Tan',     'Aquino',     'Male',   '2006-12-02', '2nd Year',  'BS Computer Science',        'A', '09201234577', 'miguel.aquino@example.com',   'Legazpi City, Albay', 'Active',    'miguel.aquino',   SHA2('student123', 256)),
    ('2024-0021', 'Jose',    NULL,      'Reyes',      'Male',   '2005-11-05', '3rd Year',  'BS Information Technology',  'B', '09391234569', 'jose.reyes@example.com',      'Camalig, Albay',      'Active',    'jose.reyes',      SHA2('student123', 256)),
    ('2024-0022', 'Carlo',   NULL,      'Navarro',    'Male',   '2005-02-03', '3rd Year',  'BS Meteorology',               'A', '09911234575', 'carlo.navarro@example.com',   'Oas, Albay',          'Dropped',   'carlo.navarro',   SHA2('student123', 256)),
    ('2023-0031', 'Mark',    'Dy',      'Villanueva', 'Male',   '2004-09-18', '4th Year',  'BS Chemistry',               'A', '09561234571', 'mark.villanueva@example.com', 'Ligao City, Albay',   'On Leave',  'mark.villanueva', SHA2('student123', 256)),
    ('2023-0032', 'Paolo',   NULL,      'Bautista',   'Male',   '2004-12-27', '4th Year',  'BS Computer Science',        'B', '09781234573', 'paolo.bautista@example.com',  'Legazpi City, Albay', 'Graduated', 'paolo.bautista',  SHA2('student123', 256)),
    ('2023-0033', 'Hannah',  'Reyes',   'Castillo',   'Female', '2004-04-21', '4th Year',  'BS Biology',                 'A', '09151234578', 'hannah.castillo@example.com', 'Daraga, Albay',       'Active',    'hannah.castillo', SHA2('student123', 256)),
    ('2026-0099', 'Test',    NULL,      'Student',    'Male',   '2008-06-15', '1st Year',  'BS Information Technology',  'A', '09123456789', 'test.student@example.com',    'Naga City, Camarines Sur', 'Active', 'test.student',  SHA2('test123', 256));

INSERT INTO activity_logs (actor_type, actor_id, action, target_student_id, details) VALUES
    ('Teacher', 1, 'LOGIN',    NULL, 'Teacher Jonathan Pelon signed in'),
    ('Student', 1, 'REGISTER', 1,    'Student 2026-0001 registered'),
    ('Teacher', 1, 'EDIT',     10,   'Status changed to On Leave');

-- ============================================================
-- 7. QUICK CHECKS (run these after importing)
-- ============================================================

-- Expected: 1st Year 5, 2nd Year 3, 3rd Year 2, 4th Year 3
SELECT year_level, total_students
FROM vw_year_level_counts
ORDER BY sort_order;

-- Each should return exactly 1 row if the test logins work:
SELECT username FROM teachers WHERE username = 'jonpel'       AND password_hash = SHA2('pass1', 256);
SELECT username FROM students WHERE username = 'test.student' AND password_hash = SHA2('test123', 256);

USE sis_db;

ALTER TABLE students
    MODIFY student_number  VARCHAR(30) NULL,
    MODIFY gender          ENUM('Male','Female','Other','Prefer not to say') NULL,
    MODIFY birthdate       DATE         NULL,
    MODIFY year_level      VARCHAR(20)  NULL,
    MODIFY program         VARCHAR(100) NULL,
    MODIFY block           VARCHAR(20)  NULL,
    MODIFY contact_number  VARCHAR(20)  NULL;