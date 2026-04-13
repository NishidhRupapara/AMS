import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router, RouterModule } from '@angular/router';

@Component({
  selector: 'app-student-sidebar',
  standalone: true,
  imports: [CommonModule, RouterModule],
  templateUrl: './student-sidebar.html',
  styleUrls: ['./student-sidebar.css']
})
export class StudentSidebarComponent implements OnInit {
  studentName: string = 'Student';

  constructor(private router: Router) {}

  ngOnInit(): void {
    const name = sessionStorage.getItem("sessionStudentName");
    if (name) {
      this.studentName = name.replace(/['"]/g, '').trim();
    }
  }

  handleLogout(): void {
    if (confirm("Are you sure you want to logout?")) {
      sessionStorage.clear();
      this.router.navigate(['/student-login']);
    }
  }
}