import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule, Router } from '@angular/router';

@Component({
  selector: 'app-admin-sidebar',
  standalone: true,
  imports: [CommonModule, RouterModule],
  templateUrl: './admin-sidebar.html',
  styleUrls: ['./admin-sidebar.css']
})
export class AdminSidebarComponent {
  // Object to track which submenus are open
  openSubmenus: any = {
    student: false,
    faculty: false,
    department: false,
    notice: false
  };

  constructor(private router: Router) {}

  toggleSubmenu(menu: string) {
    // Close other submenus and toggle the clicked one
    Object.keys(this.openSubmenus).forEach(key => {
      if (key !== menu) this.openSubmenus[key] = false;
    });
    this.openSubmenus[menu] = !this.openSubmenus[menu];
  }

  handleLogout() {
    if (confirm("Are you sure you want to logout?")) {
      localStorage.removeItem('adminToken'); // Clear your auth token
      this.router.navigate(['/admin-login']);
    }
  }
}