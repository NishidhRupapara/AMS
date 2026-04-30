import { Component, OnInit, ChangeDetectorRef, NgZone } from '@angular/core';
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
  currentUrl: string = '';

  constructor(
    private router: Router,
    private cdr: ChangeDetectorRef,
    private zone: NgZone
  ) {
    this.router.events.subscribe(() => {
      this.zone.run(() => {
        this.currentUrl = this.router.url;
        this.cdr.detectChanges();
      });
    });
  }

  ngOnInit(): void {
    this.currentUrl = this.router.url;
    const name = sessionStorage.getItem("sessionStudentName");
    if (name) {
      this.zone.run(() => {
        this.studentName = name.replace(/['"]/g, '').trim();
        this.cdr.detectChanges();
      });
    }
  }

  navigateTo(path: string): void {
    this.zone.run(() => {
      this.router.navigate([path]);
      this.cdr.detectChanges();
    });
  }

  isActive(path: string): boolean {
    return this.currentUrl === path || this.currentUrl.startsWith(path + '/');
  }

  handleLogout(): void {
    this.zone.run(() => {
      if (confirm("Are you sure you want to logout?")) {
        sessionStorage.clear();
        this.router.navigate(['/student-login']);
        this.cdr.detectChanges();
      }
    });
  }
}