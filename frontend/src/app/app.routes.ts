import { Routes } from '@angular/router';

// 🛡️ Guards
import { adminAuthGuard } from './guards/admin-auth-guard';
import { facultyAuthGuard } from './guards/faculty-auth-guard';
import { studentAuthGuard } from './guards/student-auth-guard';

// 🔐 Authentication Components
import { FacultyLoginComponent } from './faculty/faculty-login/faculty-login';
import { AdminLoginComponent } from './admin/admin-login/admin-login';
import { FacultyRegistrationComponent } from './faculty/faculty-registration/faculty-registration';
import { StudentLoginComponent } from './student/student-login/student-login';
import { StudentRegistrationComponent } from './student/student-registration/student-registration';

// 🏠 Admin Pages
import { AdminHomeComponent } from './admin/admin-home/admin-home';
import { AddStudentComponent } from './admin/add-student/add-student';
import { ViewStudentsComponent } from './admin/view-students/view-students';
import { StudentDetailComponent } from './admin/student-detail/student-detail';
import { StudentEditComponent } from './admin/student-edit/student-edit';
import { AddDepartmentComponent } from './admin/add-department/add-department';
import { AddFacultyComponent } from './admin/add-faculty/add-faculty';
import { ViewFacultyComponent } from './admin/view-faculty/view-faculty'; 
import { FacultyDetailComponent } from './admin/faculty-detail/faculty-detail';
import { FacultyEditComponent } from './admin/faculty-edit/faculty-edit';
import { FecultyStudentComponent } from './admin/feculty-student/feculty-student';
import { DepartmentDetailsComponent } from './admin/department-details/department-details';
import { NoticeComponent } from './admin/notice/notice';
import { AddNoticeComponent } from './admin/add-notice/add-notice';
import { SuggestionComponent } from './admin/suggestion/suggestion';
import { ViewAttendanceComponent } from './admin/view-attendance/view-attendance';
import { EditAttendanceComponent } from './admin/edit-attendance/edit-attendance';
import { AttendanceDetailComponent } from './admin/attendance-detail/attendance-detail';
import { AdminExamComponent } from './admin/admin-exam/admin-exam';
import { AdminLeavesComponent } from './admin/admin-leaves/admin-leaves';
import { AdminAssignmentsComponent } from './admin/admin-assignments/admin-assignments';
import { AdminMaterialsComponent } from './admin/admin-materials/admin-materials';

// 👨‍🏫 Faculty Pages
import { FacultyHomeComponent } from './faculty/faculty-home/faculty-home';
import { TakeAttendanceComponent } from './faculty/take-attendance/take-attendance'; 
import { AttendanceHistoryComponent } from './faculty/attendance-history/attendance-history'; 
import { StudentRecordComponent } from './faculty/student-record/student-record'; 
import { StudentCenterComponent } from './faculty/student-center/student-center';
import { FacultyNoticeComponent } from './faculty/faculty-notice/faculty-notice';
import { FacultySuggestionComponent } from './faculty/faculty-suggestion/faculty-suggestion';
import { FacultyProfileComponent } from './faculty/faculty-profile/faculty-profile'; 
import { FacultyLeaveComponent } from './faculty/faculty-leave/faculty-leave';
import { StudyMaterialComponent } from './faculty/study-material/study-material';
import { AssignmentComponent } from './faculty/assignment/assignment';
import { FacultyExamComponent } from './faculty/faculty-exam/faculty-exam';
import { FacultyExamStudentsComponent } from './faculty/faculty-exam-students/faculty-exam-students';

// 🎓 Student Pages
import { StudentHomeComponent } from './student/student-home/student-home'; 
import { ViewMyAttendanceComponent } from './student/view-my-attendance/view-my-attendance';
import { ViewResultsComponent } from './student/view-results/view-results';
import { ViewAssignmentsComponent } from './student/view-assignments/view-assignments';
import { ViewStudyMaterialComponent } from './student/view-study-material/view-study-material';
import { StudentProfileComponent } from './student/student-profile/student-profile';
import { StudentNoticesComponent } from './student/student-notices/student-notices';
import { StudentSuggestionComponent } from './student/student-suggestion/student-suggestion';
import { StudentExamComponent } from './student/student-exam/student-exam';
import { LandingPageComponent } from './landing-page/landing-page';

export const routes: Routes = [
  // ----------------------------------------------------
  // 0. LANDING PAGE
  // ----------------------------------------------------
  { path: '', component: LandingPageComponent },

  // ----------------------------------------------------
  // 1. PUBLIC ROUTES
  // ----------------------------------------------------
  { path: 'faculty-login', component: FacultyLoginComponent },
  { path: 'admin-login', component: AdminLoginComponent },
  { path: 'faculty-registration', component: FacultyRegistrationComponent },
  { path: 'student-login', component: StudentLoginComponent },
  { path: 'student-registration', component: StudentRegistrationComponent },

  // ... (rest of the routes)
  { path: 'admin-home', component: AdminHomeComponent, canActivate: [adminAuthGuard] },
  { path: 'add-student', component: AddStudentComponent, canActivate: [adminAuthGuard] },
  { path: 'view-students', component: ViewStudentsComponent, canActivate: [adminAuthGuard] },
  { path: 'student/view/:id', component: StudentDetailComponent, canActivate: [adminAuthGuard] },
  { path: 'student/edit/:id', component: StudentEditComponent, canActivate: [adminAuthGuard] },
  { path: 'view-attendance', component: ViewAttendanceComponent, canActivate: [adminAuthGuard] },
  { path: 'attendance/edit/:id', component: EditAttendanceComponent, canActivate: [adminAuthGuard] }, 
  { path: 'attendance/view/:id', component: AttendanceDetailComponent, canActivate: [adminAuthGuard] },
  { path: 'add-faculty', component: AddFacultyComponent, canActivate: [adminAuthGuard] },
  { path: 'view-faculty', component: ViewFacultyComponent, canActivate: [adminAuthGuard] },
  { path: 'faculty/view/:id', component: FacultyDetailComponent, canActivate: [adminAuthGuard] },
  { path: 'faculty/edit/:id', component: FacultyEditComponent, canActivate: [adminAuthGuard] },
  { path: 'faculty-students', component: FecultyStudentComponent, canActivate: [adminAuthGuard] },
  { path: 'add-department', component: AddDepartmentComponent, canActivate: [adminAuthGuard] },
  { path: 'view-departments', component: DepartmentDetailsComponent, canActivate: [adminAuthGuard] },
  { path: 'add-notice', component: AddNoticeComponent, canActivate: [adminAuthGuard] },
  { path: 'view-notices', component: NoticeComponent, canActivate: [adminAuthGuard] },
  { path: 'view-suggestions', component: SuggestionComponent, canActivate: [adminAuthGuard] },
  { path: 'admin-exams', component: AdminExamComponent, canActivate: [adminAuthGuard] },
  { path: 'admin-leaves', component: AdminLeavesComponent, canActivate: [adminAuthGuard] },
  { path: 'admin-assignments', component: AdminAssignmentsComponent, canActivate: [adminAuthGuard] },
  { path: 'admin-materials', component: AdminMaterialsComponent, canActivate: [adminAuthGuard] },

  // 👨‍🏫 Faculty Pages
  { path: 'faculty-home', component: FacultyHomeComponent, canActivate: [facultyAuthGuard] },
  { path: 'take-attendance', component: TakeAttendanceComponent, canActivate: [facultyAuthGuard] },
  { path: 'attendance-history', component: AttendanceHistoryComponent, canActivate: [facultyAuthGuard] },
  { path: 'student-report', component: StudentRecordComponent, canActivate: [facultyAuthGuard] },
  { path: 'students', component: StudentCenterComponent, canActivate: [facultyAuthGuard] },
  { path: 'notice', component: FacultyNoticeComponent, canActivate: [facultyAuthGuard] },
  { path: 'suggestion', component: FacultySuggestionComponent, canActivate: [facultyAuthGuard] },
  { path: 'faculty-leave', component: FacultyLeaveComponent, canActivate: [facultyAuthGuard] },
  { path: 'study-material', component: StudyMaterialComponent, canActivate: [facultyAuthGuard] },
  { path: 'assignment', component: AssignmentComponent, canActivate: [facultyAuthGuard] },
  { path: 'exam-creation', component: FacultyExamComponent, canActivate: [facultyAuthGuard] },
  { path: 'exam-students', component: FacultyExamStudentsComponent, canActivate: [facultyAuthGuard] },
  { path: 'profile', component: FacultyProfileComponent, canActivate: [facultyAuthGuard] },

  // 🎓 Student Pages
  { path: 'student-home', component: StudentHomeComponent, canActivate: [studentAuthGuard] },
  { path: 'view-my-attendance', component: ViewMyAttendanceComponent, canActivate: [studentAuthGuard] },
  { path: 'view-results', component: ViewResultsComponent, canActivate: [studentAuthGuard] },
  { path: 'my-assignments', component: ViewAssignmentsComponent, canActivate: [studentAuthGuard] },
  { path: 'my-study-material', component: ViewStudyMaterialComponent, canActivate: [studentAuthGuard] },
  { path: 'student-profile', component: StudentProfileComponent, canActivate: [studentAuthGuard] },
  { path: 'student-notices', component: StudentNoticesComponent, canActivate: [studentAuthGuard] },
  { path: 'student-suggestion', component: StudentSuggestionComponent, canActivate: [studentAuthGuard] },
  { path: 'exam-attempt', component: StudentExamComponent, canActivate: [studentAuthGuard] },

  // ----------------------------------------------------
  // 5. DEFAULT & WILDCARD
  // ----------------------------------------------------
  { path: '**', redirectTo: '' } 
];