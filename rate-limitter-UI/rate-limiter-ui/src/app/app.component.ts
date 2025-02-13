import { Component, OnInit } from '@angular/core';
import { ApiService } from './services/api.service';
import { FormsModule } from '@angular/forms';
import { CommonModule, DatePipe } from '@angular/common';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css'],
  imports: [FormsModule, CommonModule], 
})
export class AppComponent implements OnInit {
  accounts: any[] = [];
  selectedAccount: any;
  businessNumbers: any[] = [];
  selectedBusinessNumber: any = null;
  history: any[] = [];
  startDate: string | null = null; 
  endDate: string |null = null;

  constructor(private apiService: ApiService) {}

  ngOnInit(): void {
    this.apiService.getAccounts().subscribe((data: any) => {
      this.accounts = data;
      if (this.accounts.length > 0) {
        this.selectedAccount = this.accounts[0];
        console.log(this.selectedAccount);

        this.loadBusinessNumbers();
      }
    });
  }

  loadBusinessNumbers(): void {
    this.apiService.getBusinessNumbers(this.selectedAccount.id).subscribe((data: any) => {
      this.businessNumbers = data;
      if (this.businessNumbers.length > 0) {
        this.loadHistory();
      }
    });
  }

  loadHistory(): void {
    const businessPhone = this.selectedBusinessNumber ? this.selectedBusinessNumber.phoneNumber : '';
  
    this.apiService.getHistory(
      this.selectedAccount.id,
      businessPhone,
      this.startDate,
      this.endDate
    ).subscribe((data: any) => {
      this.history = data;
      console.log("history", this.history);
    });
  }
  
  resetFilters() {
    this.startDate = null;
    this.endDate = null;
    this.selectedBusinessNumber = null;

    this.loadHistory();
  }
}