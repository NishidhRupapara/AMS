import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';

@Component({
  selector: 'app-landing-page',
  standalone: true,
  imports: [CommonModule, RouterModule],
  templateUrl: './landing-page.html',
  styleUrls: ['./landing-page.css']
})
export class LandingPageComponent {
  roles = [
    {
      title: 'Administrator',
      icon: '🛡️',
      description: 'System oversight, user management, and department coordination.',
      route: '/admin-login',
      color: '#0d6efd'
    },
    {
      title: 'Faculty Member',
      icon: '👨‍🏫',
      description: 'Manage attendance, share resources, and create examinations.',
      route: '/faculty-login',
      color: '#198754'
    },
    {
      title: 'Student Portal',
      icon: '🎓',
      description: 'Access study materials, check attendance, and take exams.',
      route: '/student-login',
      color: '#6f42c1'
    }
  ];
}
