import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';

@Component({
    selector: 'app-user-layout',
    standalone: true,
    imports: [CommonModule, RouterModule], // Nhớ import RouterModule để xài router-outlet
    templateUrl: './user-layout.html'
})
export class UserLayoutComponent { }