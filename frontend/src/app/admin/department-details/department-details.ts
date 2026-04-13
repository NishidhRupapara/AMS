import { Component, OnInit, ChangeDetectorRef } from '@angular/core'; // ✅ Import this
import { CommonModule } from '@angular/common';
import { HttpClient } from '@angular/common/http';
import { FormsModule } from '@angular/forms';
import { AdminSidebarComponent } from '../admin-sidebar/admin-sidebar';

@Component({
  selector: 'app-department-details',
  standalone: true,
  imports: [CommonModule, FormsModule, AdminSidebarComponent],
  templateUrl: './department-details.html'
})
export class DepartmentDetailsComponent implements OnInit {
  departments: any[] = [];
  loading: boolean = true;
  showViewModal: boolean = false;
  showEditModal: boolean = false;
  selectedDept: any = {};

  private apiUrl = "http://localhost:5139/api/Departments";

  constructor(
    private http: HttpClient, 
    private cdr: ChangeDetectorRef // ✅ Inject the Detector
  ) {}

  ngOnInit(): void {
    this.fetchDepartments();
  }

  fetchDepartments(): void {
    this.http.get<any[]>(`${this.apiUrl}/Dall`).subscribe({
      next: (data) => {
        this.departments = data;
        this.loading = false;
        this.cdr.detectChanges(); // 🚀 Forces table to show data immediately
      },
      error: () => {
        this.loading = false;
        this.cdr.detectChanges();
      }
    });
  }

  handleDelete(id: any): void {
    if (!confirm("Are you sure you want to delete this department?")) return;

    this.http.delete(`${this.apiUrl}/${id}`).subscribe({
      next: () => {
        this.departments = this.departments.filter(dep => dep.did !== id);
        alert("Department Deleted!");
        this.cdr.detectChanges(); // 🚀 Removes row from table immediately
      },
      error: () => alert("Failed to delete department")
    });
  }

  handleView(id: any): void {
    this.http.get(`${this.apiUrl}/${id}`).subscribe({
      next: (data) => {
        this.selectedDept = data;
        this.showViewModal = true;
        this.cdr.detectChanges(); // 🚀 Opens View Modal immediately
      },
      error: () => alert("Department not found")
    });
  }

  handleEdit(id: any): void {
    this.http.get(`${this.apiUrl}/${id}`).subscribe({
      next: (data) => {
        this.selectedDept = data;
        this.showEditModal = true;
        this.cdr.detectChanges(); // 🚀 Opens Edit Modal immediately
      },
      error: () => alert("Department not found")
    });
  }

  handleUpdate(): void {
    this.http.put(`${this.apiUrl}/${this.selectedDept.did}`, this.selectedDept).subscribe({
      next: () => {
        alert("✅ Department updated successfully");
        this.closeModals(); // Use helper to close and refresh
        this.fetchDepartments();
      },
      error: () => alert("❌ Failed to update department")
    });
  }

  // ✅ Helper to ensure modals close and UI refreshes 
  closeModals() {
    this.showViewModal = false;
    this.showEditModal = false;
    this.cdr.detectChanges(); // 🚀 Clears the backdrop and modal instantly
  }
}