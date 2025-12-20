-- Diagnostic Script for Employee Check-In Issue
-- Run this script to verify employee accounts are properly configured

-- 1. List all employee users and their linkage status
SELECT 
    u.Id AS UserId,
    u.UserName,
    u.Email,
    u.UserType,
    u.EmployeId,
    e.id_utilisateur AS EmployeeRecordId,
    e.prenom_emp AS FirstName,
    e.nom_emp AS LastName,
    e.CIN,
    CASE 
        WHEN u.EmployeId IS NULL THEN 'NOT LINKED'
        WHEN e.id_utilisateur IS NULL THEN 'BROKEN LINK'
        ELSE 'OK'
    END AS LinkStatus
FROM AspNetUsers u
LEFT JOIN Employe e ON u.EmployeId = e.id_utilisateur
WHERE u.UserType = 'Employe'
ORDER BY u.Email;

-- 2. Check employee roles
SELECT 
    u.UserName,
    u.Email,
    r.Name AS RoleName
FROM AspNetUsers u
INNER JOIN AspNetUserRoles ur ON u.Id = ur.UserId
INNER JOIN AspNetRoles r ON ur.RoleId = r.Id
WHERE u.UserType = 'Employe'
ORDER BY u.Email;

-- 3. Check today's presence records
DECLARE @Today DATE = CAST(GETDATE() AS DATE);

SELECT 
    p.id_pr AS PresenceId,
    p.id_utilisateur AS EmployeeId,
    e.prenom_emp + ' ' + e.nom_emp AS EmployeeName,
    e.CIN,
    p.dateP AS Date,
    p.heure_arrive AS ArrivalTime,
    p.heure_depart AS DepartureTime,
    CASE 
        WHEN p.heure_depart IS NOT NULL THEN 'Complete'
        WHEN p.heure_arrive IS NOT NULL THEN 'Checked In'
        ELSE 'Unknown'
    END AS Status
FROM Presence p
INNER JOIN Employe e ON p.id_utilisateur = e.id_utilisateur
WHERE p.dateP = @Today
ORDER BY p.heure_arrive;

-- 4. Check employee records without user accounts
SELECT 
    e.id_utilisateur,
    e.prenom_emp,
    e.nom_emp,
    e.CIN,
    e.dateEmbauche
FROM Employe e
LEFT JOIN AspNetUsers u ON e.id_utilisateur = u.EmployeId
WHERE u.Id IS NULL;

-- 5. Fix broken links (ONLY IF NEEDED - Review before running)
-- Uncomment and modify the WHERE clause to fix specific broken links

/*
UPDATE AspNetUsers
SET EmployeId = (
    SELECT TOP 1 id_utilisateur 
    FROM Employe 
    WHERE CIN = 'REPLACE_WITH_ACTUAL_CIN'
)
WHERE Email = 'REPLACE_WITH_ACTUAL_EMAIL@example.com';
*/

-- 6. Check all presence records for a specific employee
-- Replace 'employee@example.com' with actual email
/*
SELECT 
    p.id_pr,
    p.dateP,
    p.heure_arrive,
    p.heure_depart,
    DATEDIFF(MINUTE, p.heure_arrive, ISNULL(p.heure_depart, CAST(GETDATE() AS TIME))) AS MinutesWorked
FROM Presence p
INNER JOIN Employe e ON p.id_utilisateur = e.id_utilisateur
INNER JOIN AspNetUsers u ON e.id_utilisateur = u.EmployeId
WHERE u.Email = 'employee@example.com'
ORDER BY p.dateP DESC;
*/

-- 7. Monthly presence statistics for all employees
SELECT 
    e.prenom_emp + ' ' + e.nom_emp AS EmployeeName,
    e.CIN,
    COUNT(p.id_pr) AS DaysPresent,
    SUM(
        CASE 
            WHEN p.heure_arrive IS NOT NULL AND p.heure_depart IS NOT NULL
            THEN DATEDIFF(MINUTE, p.heure_arrive, p.heure_depart)
            ELSE 0
        END
    ) / 60.0 AS TotalHoursWorked
FROM Employe e
LEFT JOIN Presence p ON e.id_utilisateur = p.id_utilisateur 
    AND p.dateP >= DATEFROMPARTS(YEAR(GETDATE()), MONTH(GETDATE()), 1)
    AND p.dateP <= GETDATE()
GROUP BY e.id_utilisateur, e.prenom_emp, e.nom_emp, e.CIN
ORDER BY e.nom_emp;
