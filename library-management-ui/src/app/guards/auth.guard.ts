import { inject } from '@angular/core';
import { Router, CanActivateFn } from '@angular/router';
import { AuthService } from '../services/auth.service';

export const authGuard: CanActivateFn = (route, state) => {
  const authService = inject(AuthService);
  const router = inject(Router);
  
  if (authService.isLoggedIn) {
    // Check if route requires specific role
    const requiredRole = route.data['role'] as string;
    
    if (requiredRole) {
      const user = authService.getUserFromStorage();
      const hasRole = user?.role === requiredRole;
      
      if (!hasRole) {
        router.navigate(['/books']);
        return false;
      }
    }
    
    return true;
  }
  
  router.navigate(['/login'], { queryParams: { returnUrl: state.url } });
  return false;
};