import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { HttpClient } from '@angular/common/http';
import { StudentSidebarComponent } from '../student-sidebar/student-sidebar';

@Component({
  selector: 'app-student-home',
  standalone: true,
  imports: [CommonModule, StudentSidebarComponent],
  templateUrl: './student-home.html',
  styleUrls: ['./student-home.css']
})
export class StudentHomeComponent implements OnInit {
  studentName: string = 'Student';
  studentId: string | null = null;
  studentDept: string | null = null;
  
  stats = {
    totalClasses: 0,
    presentDays: 0,
    attendancePercentage: 0
  };

  latestNotice: any = null;
  isLoading: boolean = true;

  constructor(private http: HttpClient, private cdr: ChangeDetectorRef) {}

  ngOnInit(): void {
    const rawId = sessionStorage.getItem("sessionStudentId");
    this.studentId = rawId ? rawId.toString().replace(/['"]/g, '').trim() : null;
    
    const rawName = sessionStorage.getItem("sessionStudentName");
    this.studentName = rawName ? rawName.toString().replace(/['"]/g, '').trim() : 'Student';
    
    this.studentDept = sessionStorage.getItem("sessionStudentDept");

    if (this.studentId) {
      this.loadDashboardData();
    }
  }

  loadDashboardData() {
    // 1. Fetch Attendance Stats for this specific student
    this.http.get<any[]>(`http://localhost:5139/api/Student/my-attendance/${this.studentId}`)
      .subscribe(data => {
        const attendanceData = Array.isArray(data) ? data : [];
        this.stats.totalClasses = attendanceData.length;
        this.stats.presentDays = attendanceData.filter(a => (a.status || a.Status) === 'Present').length;
        this.stats.attendancePercentage = this.stats.totalClasses > 0 
          ? Math.round((this.stats.presentDays / this.stats.totalClasses) * 100) 
          : 0;
        this.isLoading = false;
        this.cdr.detectChanges();
      });

    // 2. Fetch Latest Notice for their department
    this.http.get<any[]>(`http://localhost:5139/api/Admin/notices`)
      .subscribe(notices => {
        this.latestNotice = notices[0]; // Gets the most recent one
        this.cdr.detectChanges();
      });
  }
}