import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';

@Component({
    selector: 'app-admin-layout',
    standalone: true,
    imports: [CommonModule, RouterModule], // Nhớ import RouterModule để xài router-outlet
    templateUrl: './admin-layout.html'
})
export class AdminLayoutComponent { }