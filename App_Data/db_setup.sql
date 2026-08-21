-- SQL Script to set up dynamic menu system and privileges
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'AppMenu')
BEGIN
    CREATE TABLE AppMenu (
        MenuId INT IDENTITY(1,1) PRIMARY KEY,
        MenuName NVARCHAR(100) NOT NULL,
        ControllerName NVARCHAR(100) NULL,
        ActionName NVARCHAR(100) NULL,
        Url NVARCHAR(255) NULL,
        Icon NVARCHAR(50) NULL,
        ParentMenuId INT NULL,
        DisplayOrder INT NOT NULL DEFAULT 0,
        Status NVARCHAR(20) NOT NULL DEFAULT 'Active',
        CONSTRAINT FK_AppMenu_Parent FOREIGN KEY (ParentMenuId) REFERENCES AppMenu(MenuId)
    );
END

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'RolePrivilege')
BEGIN
    CREATE TABLE RolePrivilege (
        PrivilegeId INT IDENTITY(1,1) PRIMARY KEY,
        UserGroupSerial INT NOT NULL,
        MenuId INT NOT NULL,
        CanView BIT NOT NULL DEFAULT 0,
        CanAdd BIT NOT NULL DEFAULT 0,
        CanEdit BIT NOT NULL DEFAULT 0,
        CanDelete BIT NOT NULL DEFAULT 0,
        CONSTRAINT FK_RolePrivilege_Menu FOREIGN KEY (MenuId) REFERENCES AppMenu(MenuId),
        CONSTRAINT UQ_Group_Menu UNIQUE (UserGroupSerial, MenuId)
    );
END

-- Clear existing data for a clean seed
DELETE FROM RolePrivilege;
DELETE FROM AppMenu;

DECLARE @DashboardId INT, @MasterId INT, @StudentId INT, @TeacherId INT, @ExamId INT, @ResultId INT, @DetailReportId INT, @ResultsReportId INT, @AnalyzeResultId INT, @ControlPanelId INT;

-- Seeding Parent Menus
INSERT INTO AppMenu (MenuName, Url, Icon, DisplayOrder) VALUES ('Dashboard', '/Home/DashBoard', 'grid-outline', 10);
SET @DashboardId = SCOPE_IDENTITY();

INSERT INTO AppMenu (MenuName, Icon, DisplayOrder) VALUES ('Master', 'settings-outline', 20);
SET @MasterId = SCOPE_IDENTITY();

INSERT INTO AppMenu (MenuName, Icon, DisplayOrder) VALUES ('Student', 'people-outline', 30);
SET @StudentId = SCOPE_IDENTITY();

INSERT INTO AppMenu (MenuName, Icon, DisplayOrder) VALUES ('Teacher', 'id-card-outline', 40);
SET @TeacherId = SCOPE_IDENTITY();

INSERT INTO AppMenu (MenuName, Icon, DisplayOrder) VALUES ('Exam', 'document-text-outline', 50);
SET @ExamId = SCOPE_IDENTITY();

INSERT INTO AppMenu (MenuName, Icon, DisplayOrder) VALUES ('Result', 'ribbon-outline', 60);
SET @ResultId = SCOPE_IDENTITY();

INSERT INTO AppMenu (MenuName, Icon, DisplayOrder) VALUES ('Details Report', 'bar-chart-outline', 70);
SET @DetailReportId = SCOPE_IDENTITY();

INSERT INTO AppMenu (MenuName, Icon, DisplayOrder) VALUES ('Results Report', 'analytics-outline', 80);
SET @ResultsReportId = SCOPE_IDENTITY();

INSERT INTO AppMenu (MenuName, Icon, DisplayOrder) VALUES ('Analyze Result', 'pie-chart-outline', 90);
SET @AnalyzeResultId = SCOPE_IDENTITY();

INSERT INTO AppMenu (MenuName, Icon, DisplayOrder) VALUES ('Control Panel', 'options-outline', 100);
SET @ControlPanelId = SCOPE_IDENTITY();

-- Master sub-menus
INSERT INTO AppMenu (MenuName, Url, ParentMenuId, DisplayOrder) VALUES ('Grade', '../Master/AddGrade', @MasterId, 1);
INSERT INTO AppMenu (MenuName, Url, ParentMenuId, DisplayOrder) VALUES ('Batch', '../Master/AddBatch', @MasterId, 2);
INSERT INTO AppMenu (MenuName, Url, ParentMenuId, DisplayOrder) VALUES ('Class', '../Master/AddClass', @MasterId, 3);
INSERT INTO AppMenu (MenuName, Url, ParentMenuId, DisplayOrder) VALUES ('Section', '../Master/AddSection', @MasterId, 4);
INSERT INTO AppMenu (MenuName, Url, ParentMenuId, DisplayOrder) VALUES ('Subject', '../Master/AddSubject', @MasterId, 5);
INSERT INTO AppMenu (MenuName, Url, ParentMenuId, DisplayOrder) VALUES ('Subject - Teacher', '../Master/AddClassSubjectTeacher', @MasterId, 6);
INSERT INTO AppMenu (MenuName, Url, ParentMenuId, DisplayOrder) VALUES ('Term', '../Master/AddTerm', @MasterId, 7);
INSERT INTO AppMenu (MenuName, Url, ParentMenuId, DisplayOrder) VALUES ('Exam', '../Master/AddExam', @MasterId, 8);

-- Student sub-menus
INSERT INTO AppMenu (MenuName, Url, ParentMenuId, DisplayOrder) VALUES ('Registration', '../Student/AddStudent', @StudentId, 1);
INSERT INTO AppMenu (MenuName, Url, ParentMenuId, DisplayOrder) VALUES ('Add to New Batch', '../Student/StudentAddToNewBatch', @StudentId, 2);
INSERT INTO AppMenu (MenuName, Url, ParentMenuId, DisplayOrder) VALUES ('Transfer', '../Student/StudentTrasfertoNewClass', @StudentId, 3);
INSERT INTO AppMenu (MenuName, Url, ParentMenuId, DisplayOrder) VALUES ('Dropout', '../Student/StudentDropout', @StudentId, 4);

-- Teacher sub-menus
INSERT INTO AppMenu (MenuName, Url, ParentMenuId, DisplayOrder) VALUES ('Registration', '../Teacher/AddTeacher', @TeacherId, 1);
INSERT INTO AppMenu (MenuName, Url, ParentMenuId, DisplayOrder) VALUES ('Assign Subjects', '../Teacher/TeacherAddToSubject', @TeacherId, 2);
INSERT INTO AppMenu (MenuName, Url, ParentMenuId, DisplayOrder) VALUES ('Dropout', '../Teacher/TeacherDropout', @TeacherId, 3);

-- Exam sub-menus
INSERT INTO AppMenu (MenuName, Url, ParentMenuId, DisplayOrder) VALUES ('Assignment Schedule', '../Exam/ExamSchedule', @ExamId, 1);
INSERT INTO AppMenu (MenuName, Url, ParentMenuId, DisplayOrder) VALUES ('Final Exam Schedule', '../Exam/FinalExamSchedule', @ExamId, 2);
INSERT INTO AppMenu (MenuName, Url, ParentMenuId, DisplayOrder) VALUES ('Final Exam - Principal', '../Exam/FinalExamScheduleApprovedbyPrincipal', @ExamId, 3);
INSERT INTO AppMenu (MenuName, Url, ParentMenuId, DisplayOrder) VALUES ('Attendance - Assignment', '../Exam/AttedanceAssignment', @ExamId, 4);
INSERT INTO AppMenu (MenuName, Url, ParentMenuId, DisplayOrder) VALUES ('Attendance - Final', '../Exam/AttedanceFinal', @ExamId, 5);
INSERT INTO AppMenu (MenuName, Url, ParentMenuId, DisplayOrder) VALUES ('Exam(Assignment) Table', '../Exam/ExamTimeTable', @ExamId, 6);
INSERT INTO AppMenu (MenuName, Url, ParentMenuId, DisplayOrder) VALUES ('Final Exam Table', '../Exam/FinalExamTimeTable', @ExamId, 7);
INSERT INTO AppMenu (MenuName, Url, ParentMenuId, DisplayOrder) VALUES ('Download All Time Table', '../Exam/DownloadExamTimeTable', @ExamId, 8);

-- Result sub-menus
INSERT INTO AppMenu (MenuName, Url, ParentMenuId, DisplayOrder) VALUES ('Add results', '../Result/AddExamResult', @ResultId, 1);
INSERT INTO AppMenu (MenuName, Url, ParentMenuId, DisplayOrder) VALUES ('Approve - Class Teacher', '../Result/ApprovedResultClassTeacher', @ResultId, 2);
INSERT INTO AppMenu (MenuName, Url, ParentMenuId, DisplayOrder) VALUES ('Approve - Batch Head', '../Result/ApprovedResultBatchHead', @ResultId, 3);
INSERT INTO AppMenu (MenuName, Url, ParentMenuId, DisplayOrder) VALUES ('Approve - Section Head', '../Result/ApprovedResultSectionHead', @ResultId, 4);
INSERT INTO AppMenu (MenuName, Url, ParentMenuId, DisplayOrder) VALUES ('Approve - Principal', '../Result/ApprovedResultPrincipal', @ResultId, 5);

-- Details Report sub-menus
INSERT INTO AppMenu (MenuName, Url, ParentMenuId, DisplayOrder) VALUES ('Batch Detail List', '../DetailReprot/BatchDetailList', @DetailReportId, 1);
INSERT INTO AppMenu (MenuName, Url, ParentMenuId, DisplayOrder) VALUES ('Class Detail List', '../DetailReprot/ClassDetailList', @DetailReportId, 2);
INSERT INTO AppMenu (MenuName, Url, ParentMenuId, DisplayOrder) VALUES ('Section Detail List', '../DetailReprot/SectionDetailList', @DetailReportId, 3);
INSERT INTO AppMenu (MenuName, Url, ParentMenuId, DisplayOrder) VALUES ('Subject Detail List', '../DetailReprot/SubjectDetailList', @DetailReportId, 4);
INSERT INTO AppMenu (MenuName, Url, ParentMenuId, DisplayOrder) VALUES ('Students Detail List', '../DetailReprot/StudentDetailsList', @DetailReportId, 5);
INSERT INTO AppMenu (MenuName, Url, ParentMenuId, DisplayOrder) VALUES ('Registered Students', '../DetailReprot/RegisteredStudents', @DetailReportId, 6);
INSERT INTO AppMenu (MenuName, Url, ParentMenuId, DisplayOrder) VALUES ('Student Detail', '../DetailReprot/StudentDetails', @DetailReportId, 7);
INSERT INTO AppMenu (MenuName, Url, ParentMenuId, DisplayOrder) VALUES ('Teachers Detail List', '../DetailReprot/TeacherDetailList', @DetailReportId, 8);
INSERT INTO AppMenu (MenuName, Url, ParentMenuId, DisplayOrder) VALUES ('Teacher Detail', '../DetailReprot/TeacherDetails', @DetailReportId, 9);

-- Results Report sub-menus
INSERT INTO AppMenu (MenuName, Url, ParentMenuId, DisplayOrder) VALUES ('Batch/Class Result', '../ResultReport/BatchClassTotalReport', @ResultsReportId, 1);
INSERT INTO AppMenu (MenuName, Url, ParentMenuId, DisplayOrder) VALUES ('Batch/Class Attendance', '../ResultReport/BatchClassAttendanceReport', @ResultsReportId, 2);
INSERT INTO AppMenu (MenuName, Url, ParentMenuId, DisplayOrder) VALUES ('Class/Subject/Year Report', '../ResultReport/ClassSubjectReport', @ResultsReportId, 3);
INSERT INTO AppMenu (MenuName, Url, ParentMenuId, DisplayOrder) VALUES ('Class/Subject/Exam Report', '../ResultReport/ClassSubjectExamReport', @ResultsReportId, 4);
INSERT INTO AppMenu (MenuName, Url, ParentMenuId, DisplayOrder) VALUES ('Class/Highest Marks', '../ResultReport/ClassSubjectHighestMarks', @ResultsReportId, 5);
INSERT INTO AppMenu (MenuName, Url, ParentMenuId, DisplayOrder) VALUES ('Student Exam Report', '../ResultReport/StudentExamResultReport', @ResultsReportId, 6);
INSERT INTO AppMenu (MenuName, Url, ParentMenuId, DisplayOrder) VALUES ('Student Term Report', '../ResultReport/StudentTermReport', @ResultsReportId, 7);

-- Analyze Result sub-menus
INSERT INTO AppMenu (MenuName, Url, ParentMenuId, DisplayOrder) VALUES ('Attendance Analyze', '../Analyze/AttendanceReport', @AnalyzeResultId, 1);
INSERT INTO AppMenu (MenuName, Url, ParentMenuId, DisplayOrder) VALUES ('Subject Marks Range', '../Analyze/SubjectRangeReport', @AnalyzeResultId, 2);
INSERT INTO AppMenu (MenuName, Url, ParentMenuId, DisplayOrder) VALUES ('Student Performance', '../Analyze/StudentPerformanceReport', @AnalyzeResultId, 3);
INSERT INTO AppMenu (MenuName, Url, ParentMenuId, DisplayOrder) VALUES ('Average Performance', '../Analyze/MarksYearlyAnalyze', @AnalyzeResultId, 4);

-- Control Panel sub-menus
INSERT INTO AppMenu (MenuName, Url, ParentMenuId, DisplayOrder) VALUES ('Create User - Teacher', '../Admin/CreateUser', @ControlPanelId, 1);
INSERT INTO AppMenu (MenuName, Url, ParentMenuId, DisplayOrder) VALUES ('Create User - Student', '../Admin/CreateUserStudent', @ControlPanelId, 2);
INSERT INTO AppMenu (MenuName, Url, ParentMenuId, DisplayOrder) VALUES ('Create User - Parent', '../Admin/CreateUserParent', @ControlPanelId, 3);
INSERT INTO AppMenu (MenuName, Url, ParentMenuId, DisplayOrder) VALUES ('Add User Group', '../Admin/AddUserGroup', @ControlPanelId, 4);

-- Give Admin (UserGroupSerial = 1) access to everything
INSERT INTO RolePrivilege (UserGroupSerial, MenuId, CanView, CanAdd, CanEdit, CanDelete)
SELECT 1, MenuId, 1, 1, 1, 1 FROM AppMenu;

-- Seed Teacher (UserGroupSerial = 2) access matching Layout checks
DECLARE @TeacherGroupId INT = 2;
INSERT INTO RolePrivilege (UserGroupSerial, MenuId, CanView, CanAdd, CanEdit, CanDelete)
SELECT @TeacherGroupId, MenuId, 1, 0, 0, 0 
FROM AppMenu 
WHERE MenuName IN ('Dashboard', 'Exam', 'Assignment Schedule', 'Attendance - Assignment', 'Result', 'Add results', 'Details Report', 'Results Report', 'Analyze Result');

-- Stored Procedures for Role Privileges
GO
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'GetActiveMenuIds')
    DROP PROCEDURE GetActiveMenuIds;
GO
CREATE PROCEDURE GetActiveMenuIds
AS
BEGIN
    SELECT MenuId FROM AppMenu WHERE Status = 'Active';
END;
GO

IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'DeleteRolePrivilegeByUserGroup')
    DROP PROCEDURE DeleteRolePrivilegeByUserGroup;
GO
CREATE PROCEDURE DeleteRolePrivilegeByUserGroup
    @UserGroupSerial INT
AS
BEGIN
    DELETE FROM RolePrivilege WHERE UserGroupSerial = @UserGroupSerial;
END;
GO

IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'InsertRolePrivilege')
    DROP PROCEDURE InsertRolePrivilege;
GO
CREATE PROCEDURE InsertRolePrivilege
    @UserGroupSerial INT,
    @MenuId INT,
    @CanView BIT,
    @CanAdd BIT,
    @CanEdit BIT,
    @CanDelete BIT
AS
BEGIN
    INSERT INTO RolePrivilege (UserGroupSerial, MenuId, CanView, CanAdd, CanEdit, CanDelete)
    VALUES (@UserGroupSerial, @MenuId, @CanView, @CanAdd, @CanEdit, @CanDelete);
END;
GO

PRINT 'Database Setup and Seeding completed successfully!';
