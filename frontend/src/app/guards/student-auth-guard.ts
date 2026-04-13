import { inject } from '@angular/core';
import { Router, CanActivateFn } from '@angular/router';

export const studentAuthGuard: CanActivateFn = (route, state) => {
  const router = inject(Router);
  
  // 🛡️ SSR FIX: Safely check for window and sessionStorage
  if (typeof window === 'undefined' || typeof sessionStorage === 'undefined') {
    return true; // Allow SSR to proceed (browser will handle real check)
  }

  // 1. Check if Student ID exists in session
  const studentId = sessionStorage.getItem("sessionStudentId");

  // 2. Logic: If ID exists, allow access
  if (studentId) {
    return true;
  } else {
    // 3. If not logged in, redirect to student login page
    // Using a non-blocking UI notice is better for UX than alert(), 
    // but we'll keep the alert if you prefer it for debugging.
    console.warn("🔒 Access Denied! Redirecting to Student Login.");
    router.navigate(['/student-login']);
    return false;
  }
};