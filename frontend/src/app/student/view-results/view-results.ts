import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { HttpClient } from '@angular/common/http';
import { StudentSidebarComponent } from '../student-sidebar/student-sidebar';

@Component({
  selector: 'app-view-results',
  standalone: true,
  imports: [CommonModule, StudentSidebarComponent],
  templateUrl: './view-results.html',
  styleUrls: ['./view-results.css']
})
export class ViewResultsComponent implements OnInit {
  results: any[] = [];
  studentName: string = '';
  isLoading: boolean = true;

  constructor(private http: HttpClient) {}

  ngOnInit(): void {
    this.studentName = sessionStorage.getItem("sessionStudentName") || 'Student';
    const sid = sessionStorage.getItem("sessionStudentId");
    if (sid) {
      this.fetchResults(sid);
    }
  }

  fetchResults(sid: string) {
    // We point to your Student controller. 
    // If you don't have a results table yet, you can point this to a dummy JSON or your existing attendance to test layout
    this.http.get<any[]>(`http://localhost:5139/api/Student/my-results/${sid}`)
      .subscribe({
        next: (data) => {
          this.results = data;
          this.isLoading = false;
        },
        error: (err) => {
          console.error("Error fetching results", err);
          this.isLoading = false;
          // Dummy data for demonstration if DB endpoint isn't ready
          this.results = [
            { subject: 'Data Structures', totalMarks: 100, marksObtained: 85, grade: 'A', status: 'Pass' },
            { subject: 'Web Technologies', totalMarks: 100, marksObtained: 92, grade: 'O', status: 'Pass' },
            { subject: 'Database Management', totalMarks: 100, marksObtained: 78, grade: 'B', status: 'Pass' }
          ];
        }
      });
  }
}