import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';

@Injectable({
  providedIn: 'root'
})
export class ApiService {
  private apiUrl = 'http://localhost:5125/api';

  constructor(private http: HttpClient) { }

  getAccounts() {
    var res =  this.http.get(`${this.apiUrl}/monitoring/accounts`);
    console.log("accounts", res);
    return res;
  }

  getBusinessNumbers(accountId: string) {
    var res= this.http.get(`${this.apiUrl}/monitoring/business-numbers`, {
      params: { accountId }
    });
    console.log("buisiness", res);
    return res;
  }

  getHistory(accountId: string, businessPhone: string, start: string | null, end: string | null) {
    const params: any = { accountId, businessPhone };
  
    if (start) params.start = start;
    if (end) params.end = end;
  
    return this.http.get(`${this.apiUrl}/monitoring/history`, { params });
  }
  
}