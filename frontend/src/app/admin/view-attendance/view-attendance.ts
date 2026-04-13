import { Component, OnInit, ChangeDetectorRef, NgZone } from '@angular/core';
import { CommonModule, Location } from '@angular/common';
import { HttpClient } from '@angular/common/http';
import { RouterModule } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { AdminSidebarComponent } from '../admin-sidebar/admin-sidebar';

@Component({
  selector: 'app-view-attendance',
  standalone: true,
  imports: [CommonModule, RouterModule, FormsModule, AdminSidebarComponent],
  templateUrl: './view-attendance.html',
  styleUrls: ['./view-attendance.css']
})
export class ViewAttendanceComponent implements OnInit {
  currentStep: number = 1;
  isLoading: boolean = true;

  departments: any[] = [];
  allFaculties: any[] = [];
  filteredFaculties: any[] = [];
  attendanceRecords: any[] = [];

  selectedDepartment: any = null;
  selectedFaculty: any = null;

  showEditModal: boolean = false;
  recordToEdit: any = null;

  private apiUrl = 'http://localhost:5139/api';

  constructor(
    private http: HttpClient,
    private cdr: ChangeDetectorRef,
    private location: Location,
    private zone: NgZone // 🚀 ULTIMATE FIX FOR THE "CLICK" PROBLEM
  ) {}

  ngOnInit(): void {
    this.fetchInitialData();
  }

  fetchInitialData(): void {
    this.isLoading = true;
    this.http.get<any>(`${this.apiUrl}/Departments/Dall`).subscribe({
      next: (depts) => {
        this.zone.run(() => {
          this.departments = Array.isArray(depts) ? depts : (depts?.data || []);

          this.http.get<any>(`${this.apiUrl}/Faculty/all`).subscribe({
            next: (facs) => {
              this.zone.run(() => {
                this.allFaculties = Array.isArray(facs) ? facs : (facs?.data || []);
                this.isLoading = false;
                this.cdr.detectChanges();
              });
            },
            error: (err) => {
              console.error("Error fetching faculties:", err);
              this.zone.run(() => { this.isLoading = false; this.cdr.detectChanges(); });
            }
          });
        });
      },
      error: (err) => {
        console.error("Error fetching departments:", err);
        this.zone.run(() => { this.isLoading = false; this.cdr.detectChanges(); });
      }
    });
  }

  goToFaculties(dept: any): void {
    this.zone.run(() => {
      this.selectedDepartment = dept;
      const targetDeptName = (dept.departmentName || dept.DepartmentName || dept.deptName || dept.DeptName || dept.name || dept.Name || "").toLowerCase().trim();

      this.filteredFaculties = this.allFaculties.filter(f => {
        const facDeptName = (f.department || f.Department || "").toLowerCase().trim();
        return facDeptName === targetDeptName || targetDeptName.includes(facDeptName) || facDeptName.includes(targetDeptName);
      });

      this.currentStep = 2;
      this.cdr.detectChanges();
    });
  }

  goToAttendance(faculty: any): void {
    this.zone.run(() => {
      this.selectedFaculty = faculty;

      // 🚀 Use 'fid' if available, else fallback to 'id'
      const targetId = (faculty.fid && faculty.fid !== 0) ? faculty.fid.toString() : (faculty.id || faculty.Id || faculty._id);

      this.isLoading = true;
      this.http.get<any[]>(`${this.apiUrl}/Student/history/${targetId}`).subscribe({
        next: (data) => {
          this.zone.run(() => {
            this.attendanceRecords = data;
            this.currentStep = 3;
            this.isLoading = false;
            this.cdr.detectChanges();
          });
        },
        error: (err) => {
          console.error("Error fetching attendance", err);
          this.zone.run(() => { this.isLoading = false; this.cdr.detectChanges(); });
        }
      });
    });
  }

  goBack(): void {
    this.zone.run(() => {
      if (this.currentStep === 3) {
        this.currentStep = 2;
      } else if (this.currentStep === 2) {
        this.currentStep = 1;
        this.filteredFaculties = [];
      } else {
        this.location.back();
      }
      this.cdr.detectChanges();
    });
  }

  openEditModal(record: any): void {
    this.zone.run(() => {
      this.recordToEdit = {
        id: record.id || record.Id,
        studentName: record.studentName || record.StudentName || record.fullname || record.FullName,
        status: record.status || record.Status || 'Present',
        remark: record.remark || record.Remark || '',
        date: record.date || record.Date
      };
      this.showEditModal = true;
      this.cdr.detectChanges();
    });
  }

  closeEditModal(): void {
    this.zone.run(() => {
      this.showEditModal = false;
      this.recordToEdit = null;
      this.cdr.detectChanges();
    });
  }

  updateRecord(): void {
    if (!this.recordToEdit) return;
    const targetId = this.selectedFaculty.id || this.selectedFaculty.Id || this.selectedFaculty.fid;

    const payload = {
      Status: this.recordToEdit.status,
      Remark: this.recordToEdit.remark,
      FacultyId: String(targetId),
      Date: this.recordToEdit.date ? new Date(this.recordToEdit.date).toISOString() : new Date().toISOString()
    };

    this.http.put(`${this.apiUrl}/Admin/history/${this.recordToEdit.id}`, payload).subscribe({
      next: () => {
        this.zone.run(() => {
          alert("✅ Record updated successfully!");
          this.closeEditModal();
          this.goToAttendance(this.selectedFaculty);
        });
      },
      error: (err) => {
        console.error("Update failed", err);
        alert("🚨 UPDATE FAILED");
      }
    });
  }

  deleteRecord(recordId: string): void {
    if (confirm("⚠️ Permanently delete this record?")) {
      this.http.delete(`${this.apiUrl}/Admin/history/${recordId}`).subscribe({
        next: () => {
          this.zone.run(() => {
            alert("✅ Deleted!");
            this.goToAttendance(this.selectedFaculty);
          });
        },
        error: (err) => console.error("Delete failed", err)
      });
    }
  }
}
