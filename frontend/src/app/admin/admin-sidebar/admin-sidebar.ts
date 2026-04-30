import { Component, OnInit, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule, Router, NavigationEnd } from '@angular/router';
import { filter } from 'rxjs/operators';
import { Subscription } from 'rxjs';

@Component({
  selector: 'app-admin-sidebar',
  standalone: true,
  imports: [CommonModule, RouterModule],
  templateUrl: './admin-sidebar.html',
  styleUrls: ['./admin-sidebar.css']
})
export class AdminSidebarComponent implements OnInit, OnDestroy {
  // Object to track which submenus are open
  openSubmenus: any = {
    student: false,
    faculty: false,
    department: false,
    notice: false,
    exam: false,
    academic: false
  };

  private routerSub?: Subscription;

  constructor(private router: Router) {}

  ngOnInit() {
    this.checkActiveRoute();
    
    this.routerSub = this.router.events.pipe(
      filter(event => event instanceof NavigationEnd)
    ).subscribe(() => {
      this.checkActiveRoute();
    });
  }

  ngOnDestroy() {
    this.routerSub?.unsubscribe();
  }

  checkActiveRoute() {
    const url = this.router.url;
    if (url.includes('student') || url.includes('attendance')) {
      this.openSubmenus.student = true;
    } else if (url.includes('faculty')) {
      this.openSubmenus.faculty = true;
    } else if (url.includes('department')) {
      this.openSubmenus.department = true;
    } else if (url.includes('notice') || url.includes('suggestion')) {
      this.openSubmenus.notice = true;
    } else if (url.includes('exam')) {
      this.openSubmenus.exam = true;
    } else if (url.includes('assignment') || url.includes('material')) {
      this.openSubmenus.academic = true;
    }
  }
  toggleSubmenu(menu: string, event?: Event) {
    if (event) {
      event.preventDefault();
      event.stopPropagation();
    }
    
    const currentState = this.openSubmenus[menu];
    Object.keys(this.openSubmenus).forEach(key => {
      this.openSubmenus[key] = false;
    });
    this.openSubmenus[menu] = !currentState;
  }

  handleLogout() {
    if (confirm("Are you sure you want to logout?")) {
      localStorage.removeItem('adminToken'); // Clear your auth token
      this.router.navigate(['/admin-login']);
    }
  }
}