import { Component, ChangeDetectorRef } from '@angular/core'; // ✅ Added ChangeDetectorRef
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { HttpClient } from '@angular/common/http';
import { AdminSidebarComponent } from '../admin-sidebar/admin-sidebar';

@Component({
  selector: 'app-add-department',
  standalone: true,
  imports: [CommonModule, FormsModule, AdminSidebarComponent],
  templateUrl: './add-department.html' // ✅ Ensure this matches your filename
})
export class AddDepartmentComponent {
  departmentName: string = "";
  message = { type: "", text: "" };

  private apiUrl = "http://localhost:5139/api/Departments/AddDepartment";

  constructor(
    private http: HttpClient, 
    private cdr: ChangeDetectorRef // ✅ Injecting the detector
  ) {}

  handleSubmit() {
    if (!this.departmentName.trim()) {
      this.message = { type: "danger", text: "Department Name cannot be empty!" };
      return;
    }

    const body = { departmentName: this.departmentName };

    this.http.post<any>(this.apiUrl, body).subscribe({
      next: (data) => {
        // ✅ Success logic
        this.message = { 
          type: "success", 
          text: data.message || "Department added successfully!" 
        };
        this.departmentName = ""; 
        
        // 🚀 CRITICAL: This forces the alert to show up immediately!
        this.cdr.detectChanges(); 

        // Optional: Hide the message after 3 seconds
        setTimeout(() => {
          this.message = { type: "", text: "" };
          this.cdr.detectChanges();
        }, 3000);
      },
      error: (error) => {
        console.error("Error:", error);
        this.message = { 
          type: "danger", 
          text: error.error?.message || "Server error while adding department" 
        };
        this.cdr.detectChanges(); // 🚀 Force refresh on error too
      }
    });
  }
}