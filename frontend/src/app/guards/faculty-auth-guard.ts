import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';

export const facultyAuthGuard: CanActivateFn = (route, state) => {
  const router = inject(Router);
  
  // 🛡️ SSR FIX: Safely check for window and sessionStorage
  if (typeof window === 'undefined' || typeof sessionStorage === 'undefined') {
    return true; // Allow SSR to proceed (browser will handle real check)
  }

  const facultyId = sessionStorage.getItem('sessionFid');

  // 1. Check if user is logged in
  if (!facultyId) {
    router.navigate(['/faculty-login']);
    return false;
  }

  // 2. Define allowed routes for Faculty
 // Inside allowedRoutes array in faculty-auth-guard.ts
// src/app/guards/faculty-auth-guard.ts

// src/app/guards/faculty-auth-guard.ts
const allowedRoutes = [
  '/faculty-home',
  '/take-attendance',
  '/attendance-history',
  '/student-report',
  '/students',
  '/notice',
  '/suggestion',// 👈 Add this line!
  '/profile', // 👈 Add this line for the profile page
  '/faculty-leave',
  '/study-material',
  '/assignment',
  '/exam-creation',
  '/exam-students'
];
  const isAllowed = allowedRoutes.some(path => state.url.includes(path));

  if (isAllowed) {
    return true;
  } else {
    // If they try to go somewhere else, send them home
    router.navigate(['/faculty-home']);
    return false;
  }
};