import { Component } from '@angular/core';
import { RouterLink } from '@angular/router';

@Component({
  selector: 'app-unauthorized',
  standalone: true,
  imports: [RouterLink],
  template: `
    <div class="unauthorized">
      <h1>403 - Access Denied</h1>
      <p>You don't have permission to access this page.</p>
      <a routerLink="/dashboard" class="link">Go to your Dashboard</a>
    </div>
  `,
  styles: [`
    .unauthorized { text-align: center; padding: 80px 20px; }
    h1 { color: #e53e3e; font-size: 36px; }
    p { color: #718096; font-size: 18px; }
    .link { color: #667eea; text-decoration: none; font-weight: 600; }
  `],
})
export class UnauthorizedComponent {}
