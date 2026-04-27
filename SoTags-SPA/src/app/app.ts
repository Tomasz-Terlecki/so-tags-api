import { Component } from '@angular/core';
import { TagsTableComponent } from './components/tags-table/tags-table.component';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [TagsTableComponent],
  templateUrl: './app.html',
  styleUrl: './app.css'
})
export class App {}
