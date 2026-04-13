import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router, RouterModule } from '@angular/router';

@Component({
  selector: 'app-faculty-sidebar',
  standalone: true,
  imports: [CommonModule, RouterModule], // RouterModule is required for routerLink
  templateUrl: './faculty-sidebar.html',
  styleUrls: ['./faculty-sidebar.css']
})
export class FacultySidebarComponent implements OnInit {
  username: string = 'Faculty';

  constructor(private router: Router) {}

  ngOnInit(): void {
    // Dynamically grab the name of the logged-in user
    const sessionUser = sessionStorage.getItem('sessionUsername');
    if (sessionUser) {
      this.username = sessionUser;
    }
  }

  handleLogout(): void {
    // Clear session and kick back to login
    sessionStorage.clear();
    this.router.navigate(['/faculty-login']);
  }
}