import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { HttpClient } from '@angular/common/http';
import { FormsModule } from '@angular/forms';
import { AdminSidebarComponent } from '../../admin-sidebar/admin-sidebar';

@Component({
  selector: 'app-faculty-details',
  standalone: true,
  imports: [CommonModule, FormsModule, AdminSidebarComponent],
  templateUrl: './faculty-details.html'
})
export class FacultyDetailsComponent implements OnInit {
  faculties: any[] = [];
  loading: boolean = true;

  // Modal visibility states
  showViewModal: boolean = false;
  showEditModal: boolean = false;

  selectedFaculty: any = null;
  editFaculty: any = {
    fid: 0, fname: "", mname: "", lname: "", gender: "",
    dob: "", doj: "", department: "", username: "",
    email: "", mobile: "", address: ""
  };

  private apiUrl = "http://127.0.0.1:5139/api/Faculty";

  constructor(private http: HttpClient, private cdr: ChangeDetectorRef) { }

  ngOnInit() {
    this.fetchFaculties();
  }

  fetchFaculties() {
    this.http.get<any[]>(`${this.apiUrl}/all`).subscribe({
      next: (data) => {
        this.faculties = data;
        this.loading = false;
        this.cdr.detectChanges();
      },
      error: (err) => {
        console.error("Error fetching faculty list:", err);
        this.loading = false;
      }
    });
  }

  // view-faculty.ts snippets

  handleView(fid: number) {
    // Check the console (F12) to see if the URL is correct: http://127.0.0.1:5139/api/Faculty/101
    this.http.get(`${this.apiUrl}/${fid}`).subscribe({
      next: (data) => {
        this.selectedFaculty = data;
        this.showViewModal = true;
        this.cdr.detectChanges();
      },
      error: (err) => {
        console.error("View Error:", err);
        alert("Faculty not found.");
      }
    });
  }

  handleEdit(fid: number) {
    this.http.get(`${this.apiUrl}/${fid}`).subscribe({
      next: (data: any) => {
        this.editFaculty = { ...data };
        // Format dates for HTML input type="date"
        if (this.editFaculty.doj) this.editFaculty.doj = this.editFaculty.doj.split('T')[0];
        if (this.editFaculty.dob) this.editFaculty.dob = this.editFaculty.dob.split('T')[0];

        this.showEditModal = true;
        this.cdr.detectChanges();
      },
      error: (err) => {
        console.error("Edit Fetch Error:", err);
        alert("Could not load faculty for editing.");
      }
    });
  }

  // Update Logic - Submit to DB
  handleUpdateFaculty() {
    this.http.put(`${this.apiUrl}/${this.editFaculty.fid}`, this.editFaculty).subscribe({
      next: () => {
        alert("✅ Faculty updated successfully!");
        this.showEditModal = false;
        this.fetchFaculties(); // Refresh the main table
      },
      error: (err) => {
        console.error(err);
        alert("❌ Failed to update faculty.");
      }
    });
  }

  handleDelete(id: number) {
    if (confirm("Are you sure you want to delete this faculty?")) {
      this.http.delete(`${this.apiUrl}/${id}`).subscribe({
        next: () => {
          this.faculties = this.faculties.filter(f => f.fid !== id);
          this.cdr.detectChanges();
        },
        error: () => alert("Delete failed.")
      });
    }
  }
}