import { CanActivateFn, Router } from '@angular/router';
import { inject } from '@angular/core';

export const adminAuthGuard: CanActivateFn = (route, state) => {
  const router = inject(Router);

  // Check if we are running in the browser to safely access localStorage
  if (typeof window !== 'undefined' && typeof window.localStorage !== 'undefined') {
    
    // Look for the exact key set by the AdminLoginComponent
    const token = localStorage.getItem('adminToken');

    if (!token) {
      // Token is missing, kick back to login
      router.navigate(['/admin-login']);
      return false;
    }
    
    // Token exists, allow routing to proceed
    return true; 
  }
  
  return true; 
};